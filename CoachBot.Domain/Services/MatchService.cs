using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
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

        #region Get Match Data
        public Match GetMatch(int matchId)
        {
            return _coachBotContext.GetMatchById(matchId);
        }

        public PagedResult<Match> GetMatches(int regionId, int page, int pageSize, string sortOrder, int? playerId = null, int? teamId = null, bool upcomingOnly = false)
        {
            var queryable = _coachBotContext.Matches
                .Include(m => m.LineupHome)
                    .ThenInclude(th => th.Channel)
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.Channel)
                .Include(m => m.TeamHome)
                .Include(m => m.TeamAway)
                .Include(m => m.MatchStatistics)
                .Include(m => m.Tournament)
                .Include(s => s.Server)
                    .ThenInclude(s => s.Country)
                .Where(m => upcomingOnly || m.ReadiedDate != null)
                .Where(m => m.Server.RegionId == regionId)
                .Where(m => !upcomingOnly || m.ScheduledKickOff > DateTime.Now)
                .Where(m => playerId == null || m.PlayerMatchStatistics.Any(p => p.PlayerId == playerId))
                .Where(m => teamId == null || m.TeamMatchStatistics.Any(t => t.TeamId == teamId));

            return queryable.GetPaged(page, pageSize, sortOrder);
        }

        public void UpdateMatch(Match match)
        {
            var existingMatch = _coachBotContext.Matches.Single(m => m.Id == match.Id);
            existingMatch.ScheduledKickOff = match.ScheduledKickOff;
            existingMatch.ServerId = match.ServerId;
            _coachBotContext.Update(existingMatch);
            _coachBotContext.SaveChanges();
        }

        public Match GetCurrentMatchForChannel(ulong channelId)
        {
            if (!_coachBotContext.Matches.Any(m => (m.LineupHome.Channel.DiscordChannelId == channelId || (m.LineupAway != null && m.LineupAway.Channel.DiscordChannelId == channelId)) && m.ReadiedDate == null))
            {
                var channel = _coachBotContext.Channels.First(c => c.DiscordChannelId == channelId);
                CreateMatch(channel.Id);
            }

            return _coachBotContext.GetCurrentMatchForChannel(channelId);
        }

        public List<Match> GetMatchesForChannel(ulong channelId, bool readiedMatchesOnly = false, int limit = 10)
        {
            return _coachBotContext.Matches
               .Include(m => m.LineupHome)
                    .ThenInclude(th => th.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.LineupHome)
                    .ThenInclude(th => th.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Position)
                .Include(m => m.LineupHome)
                   .ThenInclude(th => th.PlayerLineupPositions)
                   .ThenInclude(ptp => ptp.Lineup)
                .Include(m => m.LineupHome)
                    .ThenInclude(th => th.PlayerSubstitutes)
                    .ThenInclude(ps => ps.Player)
                .Include(m => m.LineupHome)
                    .ThenInclude(th => th.Channel)
                    .ThenInclude(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Position)
                .Include(m => m.LineupAway)
                   .ThenInclude(th => th.PlayerLineupPositions)
                   .ThenInclude(ptp => ptp.Lineup)
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.PlayerSubstitutes)
                    .ThenInclude(ps => ps.Player)
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.Channel)
                    .ThenInclude(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
               .Where(m => m.LineupHome.Channel != null && m.LineupAway.Channel != null)
               .Where(m => m.LineupHome.Channel.DiscordChannelId == channelId || m.LineupAway.Channel.DiscordChannelId == channelId)
               .Where(m => !readiedMatchesOnly || m.ReadiedDate != null)
               .OrderByDescending(m => m.CreatedDate)
               .Take(limit)
               .ToList();
        }

        public List<MatchOutcomeType> GetFormForChannel(ulong channelId, int limit = 5)
        {
            var recentMatches = _coachBotContext.Matches
                .Where(m => m.LineupHome.Channel.DiscordChannelId == channelId || m.LineupAway.Channel.DiscordChannelId == channelId)
                .Where(m => m.IsMixMatch == false)
                .Where(m => m.MatchStatistics != null)
                .Include(m => m.MatchStatistics)
                .Include(m => m.LineupHome)
                    .ThenInclude(t => t.Channel)
                .Include(m => m.LineupAway)
                    .ThenInclude(t => t.Channel)
                .OrderByDescending(m => m.ReadiedDate)
                .Take(5);

            var formList = new List<MatchOutcomeType>();
            foreach (var recentMatch in recentMatches)
            {
                var matchDataTeamType = recentMatch.LineupHome.Channel.DiscordChannelId == channelId ? MatchDataTeamType.Home : MatchDataTeamType.Away;
                var matchOutcomeType = recentMatch.MatchStatistics.GetMatchOutcomeTypeForTeam(matchDataTeamType);
                formList.Add(matchOutcomeType);
            }

            return formList;
        }
        #endregion

        #region Manage Match Data
        public ServiceResponse CreateMatch(int channelId, Lineup existingTeam = null)
        {
            var channel = _coachBotContext.Channels.First(c => c.Id == channelId);
            var existingMatch = _coachBotContext.Matches.FirstOrDefault(m => m.LineupHome.ChannelId == channelId && (m.LineupAway == null || m.LineupAway.Channel.Id == channelId) && m.ReadiedDate == null);
            var match = existingMatch ?? new Match();

            // TODO: Make sure we don't keep creating new teams every time
            match.LineupHome = existingTeam ?? new Lineup()
            {
                ChannelId = channelId
            };
            match.CreatedDate = DateTime.UtcNow;
            match.LineupHome.TeamType = MatchTeamType.Home; // This is necessary to ensure that if a team is unchallenged that they become the Home team after

            if (channel.IsMixChannel)
            {
                match.LineupAway = new Lineup()
                {
                    TeamType = MatchTeamType.Away,
                    ChannelId = channelId
                };
            }
            else
            {
                match.LineupAway = null;
            }

            if (existingMatch == null)
            {
                _coachBotContext.Matches.Add(match);
            }

            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, "Teamsheet successfully reset");
        }

        public ServiceResponse AddPlayerToLineup(ulong channelId, Player player, string positionName = null, MatchTeamType teamType = MatchTeamType.Home)
        {
            const string GK_POSITION = "GK";
            var match = GetCurrentMatchForChannel(channelId);
            if (match.LineupHome.PlayerLineupPositions is null) match.LineupHome.PlayerLineupPositions = new List<PlayerLineupPosition>();
            var channel = _coachBotContext.Channels.Include(c => c.ChannelPositions).ThenInclude(cp => cp.Position).First(c => c.DiscordChannelId == channelId);
            var team = teamType == MatchTeamType.Home ? match.LineupHome : match.LineupAway;

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

            var playerTeamPosition = new PlayerLineupPosition()
            {
                PlayerId = player.Id,
                LineupId = team.Id,
                PositionId = position.Id
            };
            _coachBotContext.PlayerLineupPositions.Add(playerTeamPosition);
            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.Success, $"Signed **{player.DisplayName}** to **{position.Name}** for **{team.Channel.Team.Name}**");
        }

        public ServiceResponse AddSubsitutePlayerToLineup(ulong channelId, Player player, MatchTeamType teamType = MatchTeamType.Home)
        {
            var match = GetCurrentMatchForChannel(channelId);
            if (match.LineupHome.PlayerSubstitutes is null) match.LineupHome.PlayerSubstitutes = new List<PlayerLineupSubstitute>();
            if (match.SignedPlayersAndSubs.Any(sp => sp.DiscordUserId == player.DiscordUserId)) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{player.DisplayName}** is already signed");
            if (!match.SignedPlayersAndSubs.Any()) return new ServiceResponse(ServiceResponseStatus.Failure, $"All outfield positions are still available");

            var playerTeamSubstitute = new PlayerLineupSubstitute()
            {
                PlayerId = player.Id,
                LineupId = teamType == MatchTeamType.Home ? (int)match.LineupHomeId : (int)match.LineupAwayId
            };
            _coachBotContext.PlayerLineupSubstitutes.Add(playerTeamSubstitute);
            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.Success, $"Added **{player.DisplayName}** to the subs bench");
        }

        public ServiceResponse RemoveSubstitutePlayerFromMatch(ulong channelId, Player player)
        {
            var match = GetCurrentMatchForChannel(channelId);
            if (match.LineupHome.PlayerSubstitutes.Any(sp => sp.Player.DiscordUserId == player.DiscordUserId) 
                || match.LineupAway.PlayerSubstitutes.Any(sp => sp.Player.DiscordUserId == player.DiscordUserId))
            {
                RemovePlayerSubstituteFromTeam(match.LineupHome, player);
                RemovePlayerSubstituteFromTeam(match.LineupAway, player);

                return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, $"Removed **{player.DisplayName}** from the subs bench");
            }

            return new ServiceResponse(ServiceResponseStatus.Failure, $"**{player.DisplayName}** is not on the subs bench and therefore cannot be removed");
        }
               
        public ServiceResponse ClearPosition(ulong channelId, string position, MatchTeamType teamType = MatchTeamType.Home)
        {
            var match = GetCurrentMatchForChannel(channelId);
            var team = match.GetTeam(teamType);

            if (team == null) new ServiceResponse(ServiceResponseStatus.Failure, $"Cannot clear position for a team that does not exist");
            if (!team.Channel.ChannelPositions.Any(cp => cp.Position.Name.ToUpper() == position.ToUpper())) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{position.ToUpper()}** is not a valid position");

            var playerPosition = team.PlayerLineupPositions.FirstOrDefault(ptp => ptp.Position.Name.ToUpper() == position.ToUpper());
            if (playerPosition != null) team.PlayerLineupPositions.Remove(playerPosition);

            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, $"Cleared position **{position.ToUpper()}**");
        }

        public ServiceResponse RemovePlayerFromMatch(ulong channelId, Player player)
        {
            var match = GetCurrentMatchForChannel(channelId);

            if (match.LineupHome.PlayerLineupPositions.Any(ptp => ptp.Player.Id == player.Id))
            {
                var removedPlayer = RemovePlayerFromTeam(match.LineupHome, player);
                if (match.LineupHome.PlayerSubstitutes.Any())
                {
                    var substitute = ReplaceWithSubstitute(removedPlayer.Position, match.LineupHome);

                    return new ServiceResponse(ServiceResponseStatus.Success, $":arrows_counterclockwise:  **Substitution** {Environment.NewLine} {substitute.DisplayName} comes off the bench to replace **{player.DisplayName}**");
                }
            }
            else if (match.LineupAway != null && match.LineupAway.PlayerLineupPositions.Any(ptp => ptp.Player.Id == player.Id))
            {
                var removedPlayer = RemovePlayerFromTeam(match.LineupAway, player);
                if (match.LineupAway.PlayerSubstitutes.Any())
                {
                    var substitute = ReplaceWithSubstitute(removedPlayer.Position, match.LineupAway);
                    return new ServiceResponse(ServiceResponseStatus.Success, $":arrows_counterclockwise:  **Substitution** {Environment.NewLine} {substitute.DisplayName} comes off the bench to replace **{player.DisplayName}**");
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
            var channel = match.LineupHome.Channel;
            var server = _serverService.GetServer(serverId);
            matchId = match.Id;

            if (match.LineupAway == null || match.LineupHome == null) return new ServiceResponse(ServiceResponseStatus.Failure, $"There is no opposition set");
            if (match.SignedPlayersAndSubs.Count < channel.ChannelPositions.Count) return new ServiceResponse(ServiceResponseStatus.Failure, $"All positions must be filled"); ;
            if (server == null) return new ServiceResponse(ServiceResponseStatus.Failure, $"A valid server must be provided");
            if (server.RegionId != channel.Team.RegionId) return new ServiceResponse(ServiceResponseStatus.Failure, $"The selected server is not in this channels region");
            if (match.SignedPlayers.GroupBy(p => p.Id).Where(p => p.Count() > 1).Any()) return new ServiceResponse(ServiceResponseStatus.Failure, $"One or more players is signed for both teams");

            match.ServerId = server.Id;
            match.ReadiedDate = DateTime.UtcNow;
            _coachBotContext.SaveChanges();

            CreateMatch((int)match.LineupHome.ChannelId);
            if (match.LineupHome.ChannelId != match.LineupAway.ChannelId) CreateMatch((int)match.LineupAway.ChannelId);

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
            if (!challenger.IsMixChannel && challengerMatch.LineupAway != null && challengerMatch.LineupAway.ChannelId != challengerMatch.LineupHome.ChannelId) return new ServiceResponse(ServiceResponseStatus.Failure, $"You are already challenging **{challengerMatch.LineupAway.Channel.Team.Name}**");
            if (opposition.IsMixChannel && !oppositionMatch.IsMixMatch) return new ServiceResponse(ServiceResponseStatus.Failure, $"{opposition.Team.Name} already has opposition challenging");
            if (!_searchService.GetSearches().Any(s => s.ChannelId == opposition.Id) && !opposition.IsMixChannel) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{opposition.Team.Name}** aren't searching for a team to face");
            if (challengerChannelId == oppositionId) return new ServiceResponse(ServiceResponseStatus.Failure, $"You can't face yourself. Don't waste my time.");
            if (challenger.ChannelPositions.Count() != opposition.ChannelPositions.Count()) return new ServiceResponse(ServiceResponseStatus.Failure, $"Sorry, **{opposition.Team.Name}** are looking for an **{opposition.ChannelPositions.Count()}v{opposition.ChannelPositions.Count()}** match");
            if (Math.Round(challenger.ChannelPositions.Count() * 0.7) > GetCurrentMatchForChannel(challengerChannelId).LineupHome.OccupiedPositions.Count()) return new ServiceResponse(ServiceResponseStatus.Failure, $"At least **{Math.Round(challenger.ChannelPositions.Count() * 0.7)}** positions must be filled");
            if (challenger.Team.RegionId != opposition.Team.RegionId) return new ServiceResponse(ServiceResponseStatus.Failure, $"You can't challenge opponents from other regions");

            if (opposition.IsMixChannel)
            {
                CombineMixLineups(oppositionMatch, opposition);
            }

            _searchService.StopSearch(challenger.Id);
            _searchService.StopSearch(opposition.Id);
            challengerMatch.LineupAway = oppositionMatch.LineupHome;
            challengerMatch.LineupAway.TeamType = MatchTeamType.Away;
            _coachBotContext.Matches.Remove(oppositionMatch);
            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.Success, "Team sucessfully challenged");
        }

        public ServiceResponse Unchallenge(ulong unchallengerChannelId)
        {
            var challenger = _channelService.GetChannelByDiscordId(unchallengerChannelId);
            var match = GetCurrentMatchForChannel(unchallengerChannelId);

            if (match.LineupAway.ChannelId == null || match.LineupAway.ChannelId == 0 || match.LineupHome == null || match.LineupHome.ChannelId == 0) return new ServiceResponse(ServiceResponseStatus.Failure, "There is no opposition set to unchallenge");
            if (match.LineupAway.ChannelId == match.LineupHome.ChannelId) return new ServiceResponse(ServiceResponseStatus.Failure, "There is no opposition set to unchallenge");

            CreateMatch((int)match.LineupAway.ChannelId, match.LineupAway);
            match.LineupAwayId = null;
            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, "Team successfully unchallenged");
        }

        #endregion

        #region Private methods
        private PlayerLineupPosition RemovePlayerFromTeam(Lineup team, Player player)
        {
            var playerTeamPosition = team?.PlayerLineupPositions.FirstOrDefault(ptp => ptp.Player.Id == player.Id);
            if (playerTeamPosition != null)
            {
                team.PlayerLineupPositions.Remove(playerTeamPosition);
                _coachBotContext.SaveChanges();
            }

            return playerTeamPosition;
        }

        private void RemovePlayerSubstituteFromTeam(Lineup team, Player player)
        {
            if (team == null) return;

            var playerSubstitute = team.PlayerSubstitutes.FirstOrDefault(ps => ps.Player.Id == player.Id);
            if (playerSubstitute != null)
            {
                team.PlayerSubstitutes.Remove(playerSubstitute);
                _coachBotContext.SaveChanges();
            }
        }

        private Player ReplaceWithSubstitute(Position position, Lineup team)
        {
            var sub = team.PlayerSubstitutes.Select(p => p.Player).FirstOrDefault();

            if (sub != null)
            {
                RemovePlayerSubstituteFromTeam(team, sub);
                AddPlayerToLineup(team.Channel.DiscordChannelId, sub, position.Name, team.TeamType);
            }

            return sub;
        }

        private async void RemovePlayersFromOtherMatches(Match readiedMatch, bool respectDuplicityProtection = true)
        {
            var otherPlayerSignings = _coachBotContext.PlayerLineupPositions
                .Where(ptp => ptp.Lineup.Match.ReadiedDate == null)
                .Where(ptp => ptp.Lineup.Channel.DuplicityProtection == true || !respectDuplicityProtection)
                .Where(ptp => readiedMatch.SignedPlayers.Any(sp => sp.Id == ptp.PlayerId));

            _coachBotContext.PlayerLineupPositions.RemoveRange(otherPlayerSignings);
            _coachBotContext.SaveChanges();

            foreach (var otherPlayerSigning in otherPlayerSignings)
            {
                var message = $":stadium: **{otherPlayerSigning.Player.DisplayName}** has gone to play another match (**{readiedMatch.LineupHome.Channel.Team.TeamCode}** vs **{readiedMatch.LineupAway.Channel.Team.TeamCode}**)";
                await _discordNotificationService.SendChannelMessage(otherPlayerSigning.Lineup.Channel.DiscordChannelId, message);
            }
        }

        private void CombineMixLineups(Match match, Channel channel)
        {
            var unoccupiedHomePositions = channel.ChannelPositions.Where(cp => !match.LineupHome.OccupiedPositions.Any(op => cp.PositionId == op.Id));
            var transferrablePositions = unoccupiedHomePositions.Where(up => match.LineupAway.OccupiedPositions.Any(op => op.Id == up.PositionId));

            if (transferrablePositions.Any())
            {
                foreach(var position in transferrablePositions)
                {
                    var awayPlayer = match.LineupAway.PlayerLineupPositions.FirstOrDefault(ap => ap.PositionId == position.PositionId);
                    if (awayPlayer != null)
                    {
                        var playerTeamPosition = new PlayerLineupPosition()
                        {
                            PlayerId = awayPlayer.PlayerId,
                            LineupId = (int)match.LineupHomeId,
                            PositionId = awayPlayer.PositionId
                        };
                        _coachBotContext.PlayerLineupPositions.Add(playerTeamPosition);
                    }
                }
            }
        }
        #endregion
    }
}
