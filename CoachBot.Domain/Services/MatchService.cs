using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using CoachBot.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class MatchService
    {
        private readonly CoachBotContext _coachBotContext;
        private readonly ChannelService _channelService;
        private readonly ServerService _serverService;
        private readonly SearchService _searchService;
        private readonly DiscordNotificationService _discordNotificationService;

        public MatchService(CoachBotContext coachBotContext, ChannelService channelService, ServerService serverService, SearchService searchService, DiscordNotificationService discordNotificationService)
        {
            _coachBotContext = coachBotContext;
            _channelService = channelService;
            _serverService = serverService;
            _searchService = searchService;
            _discordNotificationService = discordNotificationService;
        }

        public ServiceResponse CreateMatch(int channelId, Team existingTeam = null)
        {
            var channel = _coachBotContext.Channels.First(c => c.Id == channelId);
            var existingMatch = _coachBotContext.Matches.FirstOrDefault(m => m.TeamHome.ChannelId == channelId && (m.TeamAway == null || m.TeamAway.Channel.Id == channelId) && m.ReadiedDate == null);
            var match = existingMatch ?? new Match();

            // TODO: Make sure we don't keep creating new teams every time
            match.TeamHome = existingTeam ?? new Team()
            {
                ChannelId = channelId
            };
            match.CreatedDate = DateTime.UtcNow;
            match.TeamHome.TeamType = TeamType.Home; // This is necessary to ensure that if a team is unchallenged that they become the Home team after

            if (channel.IsMixChannel)
            {
                match.TeamAway = new Team()
                {
                    TeamType = TeamType.Away,
                    ChannelId = channelId
                };
            }
            else
            {
                match.TeamAway = null;
            }

            if (existingMatch == null)
            {
                _coachBotContext.Matches.Add(match);
            }

            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, "Teamsheet successfully reset");
        }

        public ServiceResponse AddPlayerToTeam(ulong channelId, Player player, string positionName = null, TeamType teamType = TeamType.Home)
        {
            const string GK_POSITION = "GK";
            var match = GetCurrentMatchForChannel(channelId);
            if (match.TeamHome.PlayerTeamPositions is null) match.TeamHome.PlayerTeamPositions = new List<PlayerTeamPosition>();
            var channel = _coachBotContext.Channels.Include(c => c.ChannelPositions).ThenInclude(cp => cp.Position).First(c => c.DiscordChannelId == channelId);
            var team = teamType == TeamType.Home ? match.TeamHome : match.TeamAway;

            Position position;
            if (positionName == null)
            {
                var channelPositions = channel.ChannelPositions.Select(cp => cp.Position);
                position = channelPositions.FirstOrDefault(cp => !team.OccupiedPositions.Any(op => cp.Name == op.Name) && cp.Name != GK_POSITION);
            }
            else
            {
                position = channel.ChannelPositions.Select(cp => cp.Position).FirstOrDefault(p => p.Name.ToUpper() == positionName.ToUpper());
            }

            if (player.DiscordUserId != null && match.SignedPlayersAndSubs.Any(sp => sp.DiscordUserId == player.DiscordUserId)) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{player.DisplayName}** is already signed");
            if (match.SignedPlayersAndSubs.Any(sp => sp.Name == player.Name)) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{player.DisplayName}** is already signed");
            if (position is null && positionName != null) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{positionName}** is already filled");
            if (team != null && team.OccupiedPositions.Any(op => op == position)) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{positionName}** is already filled");
            if (position is null && positionName == null) return new ServiceResponse(ServiceResponseStatus.Failure, $"There are no outfield positions available");

            var playerTeamPosition = new PlayerTeamPosition()
            {
                PlayerId = player.Id,
                TeamId = team.Id,
                PositionId = position.Id
            };                                 
            _coachBotContext.PlayerTeamPositions.Add(playerTeamPosition);
            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.Success, $"Signed **{player.DisplayName}** to **{position.Name}** for **{team.Channel.Name}**");
        }

        public ServiceResponse AddSubsitutePlayerToTeam(ulong channelId, Player player, TeamType teamType = TeamType.Home)
        {
            var match = GetCurrentMatchForChannel(channelId);
            if (match.TeamHome.PlayerSubstitutes is null) match.TeamHome.PlayerSubstitutes = new List<PlayerTeamSubstitute>();
            if (match.SignedPlayersAndSubs.Any(sp => sp.DiscordUserId == player.DiscordUserId)) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{player.DisplayName}** is already signed");
            if (!match.SignedPlayersAndSubs.Any()) return new ServiceResponse(ServiceResponseStatus.Failure, $"All outfield positions are still available");

            var playerTeamSubstitute = new PlayerTeamSubstitute()
            {
                PlayerId = player.Id,
                TeamId = teamType == TeamType.Home ? (int)match.TeamHomeId : (int)match.TeamAwayId
            };
            _coachBotContext.PlayerTeamSubstitute.Add(playerTeamSubstitute);
            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.Success, $"Added **{player.DisplayName}** to the subs bench");
        }

        public ServiceResponse RemoveSubstitutePlayerFromMatch(ulong channelId, Player player)
        {
            var match = GetCurrentMatchForChannel(channelId);
            if (match.TeamHome.PlayerSubstitutes.Any(sp => sp.Player.DiscordUserId == player.DiscordUserId) 
                || match.TeamAway.PlayerSubstitutes.Any(sp => sp.Player.DiscordUserId == player.DiscordUserId))
            {
                RemovePlayerSubstituteFromTeam(match.TeamHome, player);
                RemovePlayerSubstituteFromTeam(match.TeamAway, player);
            }

            return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, $"Removed **{player.DisplayName}** from the subs bench");
        }
               
        public ServiceResponse ClearPosition(ulong channelId, string position, TeamType teamType = TeamType.Home)
        {
            var match = GetCurrentMatchForChannel(channelId);
            var team = match.GetTeam(teamType);

            if (team == null) new ServiceResponse(ServiceResponseStatus.Failure, $"Cannot clear position for a team that does not exist");
            if (!team.Channel.ChannelPositions.Any(cp => cp.Position.Name.ToUpper() == position.ToUpper())) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{position.ToUpper()}** is not a valid position");

            var playerPosition = team.PlayerTeamPositions.FirstOrDefault(ptp => ptp.Position.Name.ToUpper() == position.ToUpper());
            if (playerPosition != null) team.PlayerTeamPositions.Remove(playerPosition);

            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, $"Cleared position **{position.ToUpper()}**");
        }

        public ServiceResponse RemovePlayerFromMatch(ulong channelId, Player player)
        {
            var match = GetCurrentMatchForChannel(channelId);

            if (match.TeamHome.PlayerTeamPositions.Any(ptp => ptp.Player.Id == player.Id))
            {
                var removedPlayer = RemovePlayerFromTeam(match.TeamHome, player);
                if (match.TeamHome.PlayerSubstitutes.Any())
                {
                    var substitute = ReplaceWithSubstitute(removedPlayer.Position, match.TeamHome);
                    return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, $"Substituted **{player.DisplayName}** with **{substitute.DisplayName}**");
                }
            }
            else if (match.TeamAway != null && match.TeamAway.PlayerTeamPositions.Any(ptp => ptp.Player.Id == player.Id))
            {
                var removedPlayer = RemovePlayerFromTeam(match.TeamHome, player);
                if (match.TeamAway.PlayerSubstitutes.Any())
                {
                    var substitute = ReplaceWithSubstitute(removedPlayer.Position, match.TeamAway);
                    return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, $"Substituted **{player.DisplayName}** with **{substitute.DisplayName}**");
                }
            }
            else
            {
                return new ServiceResponse(ServiceResponseStatus.Failure, $"**{player.DisplayName}** is not signed");
            }

            return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, $"Removed **{player.DisplayName}**");
        }

        public ServiceResponse ReadyMatch(ulong channelId, int serverId, out int matchId)
        {
            var match = GetCurrentMatchForChannel(channelId);
            var channel = match.TeamHome.Channel;
            var server = _serverService.GetServer(serverId);
            matchId = match.Id;

            if (match.TeamAway == null || match.TeamHome == null) return new ServiceResponse(ServiceResponseStatus.Failure, $"There is no opposition set");
            if (match.SignedPlayersAndSubs.Count < channel.ChannelPositions.Count) return new ServiceResponse(ServiceResponseStatus.Failure, $"All positions must be filled"); ;
            if (server == null) return new ServiceResponse(ServiceResponseStatus.Failure, $"A valid server must be provided");
            if (server.RegionId != channel.RegionId) return new ServiceResponse(ServiceResponseStatus.Failure, $"The selected server is not in this channels region");
            if (match.SignedPlayers.GroupBy(p => p.Id).Where(p => p.Count() > 1).Any()) return new ServiceResponse(ServiceResponseStatus.Failure, $"One or more players is signed for both teams");

            match.ServerId = server.Id;
            match.ReadiedDate = DateTime.UtcNow;
            _coachBotContext.SaveChanges();

            CreateMatch((int)match.TeamHome.ChannelId);
            if (match.TeamHome.ChannelId != match.TeamAway.ChannelId) CreateMatch((int)match.TeamAway.ChannelId);

            RemovePlayersFromOtherMatches(match);

            return new ServiceResponse(ServiceResponseStatus.Success, $"Match successfully readied");
        }

        public ServiceResponse Challenge(ulong challengerChannelId, ulong oppositionId, string challengerMention)
        {
            var challenger = _channelService.GetChannelByDiscordId(challengerChannelId);
            var opposition = _channelService.GetChannelByDiscordId(oppositionId);
            var oppositionMatch = GetCurrentMatchForChannel(oppositionId);
            var challengerMatch = GetCurrentMatchForChannel(challengerChannelId);

            if (challenger.IsMixChannel) return new ServiceResponse(ServiceResponseStatus.Failure, $"Mix channels cannot challenge teams");
            if (!challenger.IsMixChannel && challengerMatch.TeamAway != null && challengerMatch.TeamAway.ChannelId != challengerMatch.TeamHome.ChannelId) return new ServiceResponse(ServiceResponseStatus.Failure, $"You are already challenging **{challengerMatch.TeamAway.Channel.Name}**");
            if (opposition.IsMixChannel && !oppositionMatch.IsMixMatch) return new ServiceResponse(ServiceResponseStatus.Failure, $"{opposition.Name} already has opposition challenging");
            if (!_searchService.GetSearches().Any(s => s.ChannelId == opposition.Id) && !opposition.IsMixChannel) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{opposition.Name}** are no longer searching for a team to face");
            if (challengerChannelId == oppositionId) return new ServiceResponse(ServiceResponseStatus.Failure, $"You can't face yourself. Don't waste my time.");
            if (challenger.ChannelPositions.Count() != opposition.ChannelPositions.Count()) return new ServiceResponse(ServiceResponseStatus.Failure, $"Sorry, **{opposition.Name}** are looking for an **{opposition.ChannelPositions.Count()}v{opposition.ChannelPositions.Count()}** match");
            if (Math.Round(challenger.ChannelPositions.Count() * 0.7) > GetCurrentMatchForChannel(challengerChannelId).TeamHome.OccupiedPositions.Count()) return new ServiceResponse(ServiceResponseStatus.Failure, $"At least **{Math.Round(challenger.ChannelPositions.Count() * 0.7)}** positions must be filled");
            if (challenger.RegionId != opposition.RegionId) return new ServiceResponse(ServiceResponseStatus.Failure, $"You can't challenge opponents from other regions");

            _searchService.StopSearch(challenger.Id);
            _searchService.StopSearch(opposition.Id);
            challengerMatch.TeamAway = oppositionMatch.TeamHome;
            challengerMatch.TeamAway.TeamType = TeamType.Away;
            _coachBotContext.Matches.Remove(oppositionMatch);
            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.Success, "Team sucessfully challenged");
        }

        public ServiceResponse Unchallenge(ulong unchallengerChannelId)
        {
            var challenger = _channelService.GetChannelByDiscordId(unchallengerChannelId);
            var match = GetCurrentMatchForChannel(unchallengerChannelId);

            if (match.TeamAway.ChannelId == null || match.TeamAway.ChannelId == 0 || match.TeamHome == null || match.TeamHome.ChannelId == 0) return new ServiceResponse(ServiceResponseStatus.Failure, "There is no opposition set to unchallenge");
            if (match.TeamAway.ChannelId == match.TeamHome.ChannelId) return new ServiceResponse(ServiceResponseStatus.Failure, "There is no opposition set to unchallenge");

            CreateMatch((int)match.TeamAway.ChannelId, match.TeamAway);
            match.TeamAwayId = null;
            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, "Team successfully unchallenged");
        }

        public Match GetMatch(int matchId)
        {
            return _coachBotContext.GetMatchById(matchId);
        }

        public Match GetCurrentMatchForChannel(ulong channelId)
        {
            if (!_coachBotContext.Matches.Any(m => (m.TeamHome.Channel.DiscordChannelId == channelId || (m.TeamAway != null && m.TeamAway.Channel.DiscordChannelId == channelId)) && m.ReadiedDate == null))
            {
                var channel = _coachBotContext.Channels.First(c => c.DiscordChannelId == channelId);
                CreateMatch(channel.Id);
            }

            return _coachBotContext.GetCurrentMatchForChannel(channelId);
        }

        public List<Match> GetMatchesForChannel(ulong channelId, bool readiedMatchesOnly = false, int limit = 10)
        {
            return _coachBotContext.Matches
               .Include(m => m.TeamHome)
                    .ThenInclude(th => th.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.TeamHome)
                    .ThenInclude(th => th.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Position)
                .Include(m => m.TeamHome)
                   .ThenInclude(th => th.PlayerTeamPositions)
                   .ThenInclude(ptp => ptp.Team)
                .Include(m => m.TeamHome)
                    .ThenInclude(th => th.PlayerSubstitutes)
                    .ThenInclude(ps => ps.Player)
                .Include(m => m.TeamHome)
                    .ThenInclude(th => th.Channel)
                    .ThenInclude(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
                .Include(m => m.TeamAway)
                    .ThenInclude(ta => ta.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.TeamAway)
                    .ThenInclude(ta => ta.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Position)
                .Include(m => m.TeamAway)
                   .ThenInclude(th => th.PlayerTeamPositions)
                   .ThenInclude(ptp => ptp.Team)
                .Include(m => m.TeamAway)
                    .ThenInclude(ta => ta.PlayerSubstitutes)
                    .ThenInclude(ps => ps.Player)
                .Include(m => m.TeamAway)
                    .ThenInclude(ta => ta.Channel)
                    .ThenInclude(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
               .Where(m => m.TeamHome.Channel != null && m.TeamAway.Channel != null)
               .Where(m => m.TeamHome.Channel.DiscordChannelId == channelId || m.TeamAway.Channel.DiscordChannelId == channelId)
               .Where(m => !readiedMatchesOnly || m.ReadiedDate != null)
               .OrderByDescending(m => m.CreatedDate)
               .Take(limit)
               .ToList();
        }

        private PlayerTeamPosition RemovePlayerFromTeam(Team team, Player player)
        {
            var playerTeamPosition = team?.PlayerTeamPositions.FirstOrDefault(ptp => ptp.Player.Id == player.Id);
            if (playerTeamPosition != null)
            {
                team.PlayerTeamPositions.Remove(playerTeamPosition);
                _coachBotContext.SaveChanges();
            }

            return playerTeamPosition;
        }

        private void RemovePlayerSubstituteFromTeam(Team team, Player player)
        {
            if (team == null) return;

            var playerSubstitute = team.PlayerSubstitutes.First(ps => ps.Player.Id == player.Id);
            if (playerSubstitute != null)
            {
                team.PlayerSubstitutes.Remove(playerSubstitute);
                _coachBotContext.SaveChanges();
            }
        }

        private Player ReplaceWithSubstitute(Position position, Team team)
        {
            var sub = team.PlayerSubstitutes.Select(p => p.Player).FirstOrDefault();

            if (sub != null)
            {
                RemovePlayerSubstituteFromTeam(team, sub);
                AddPlayerToTeam(team.Channel.DiscordChannelId, sub, position.Name, team.TeamType);
            }

            return sub;
        }

        private void RemovePlayersFromOtherMatches(Match readiedMatch, bool respectDuplicityProtection = true)
        {
            var otherPlayerSignings = _coachBotContext.PlayerTeamPositions
                .Where(ptp => ptp.Team.Match.ReadiedDate == null)
                .Where(ptp => ptp.Team.Channel.DuplicityProtection == true || !respectDuplicityProtection)
                .Where(ptp => readiedMatch.SignedPlayers.Any(sp => sp.Id == ptp.PlayerId));

            _coachBotContext.PlayerTeamPositions.RemoveRange(otherPlayerSignings);
            _coachBotContext.SaveChanges();

            foreach (var otherPlayerSigning in otherPlayerSignings)
            {
                var message = $":stadium: **{otherPlayerSigning.Player.DisplayName}** has gone to play another match (**{readiedMatch.TeamHome.Channel.TeamCode}** vs **{readiedMatch.TeamAway.Channel.TeamCode}**)";
                _discordNotificationService.SendChannelMessage(otherPlayerSigning.Team.Channel.DiscordChannelId, message);
            }
        }
    }
}
