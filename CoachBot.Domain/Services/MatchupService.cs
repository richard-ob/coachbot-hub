using CoachBot.Database;
using CoachBot.Domain.Model;
using CoachBot.Factories;
using CoachBot.Model;
using CoachBot.Tools;
using Discord;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoachBot.Domain.Services
{
    public class MatchupService
    {
        private readonly CoachBotContext _coachBotContext;
        private readonly MatchService _matchService;
        private readonly ChannelService _channelService;
        private readonly ServerService _serverService;
        private readonly SearchService _searchService;
        private readonly DiscordNotificationService _discordNotificationService;

        public MatchupService(
            CoachBotContext coachBotContext,
            MatchService matchService,
            ChannelService channelService,
            ServerService serverService,
            SearchService searchService,
            DiscordNotificationService discordNotificationService
        )
        {
            _coachBotContext = coachBotContext;
            _matchService = matchService;
            _channelService = channelService;
            _serverService = serverService;
            _searchService = searchService;
            _discordNotificationService = discordNotificationService;
        }

        public ServiceResponse CreateMatchup(int channelId, Lineup existingTeam = null)
        {
            var channel = _coachBotContext.Channels.First(c => c.Id == channelId);
            var existingMatchup = _coachBotContext.Matchups.FirstOrDefault(m => m.LineupHome.ChannelId == channelId && (m.LineupAway == null || m.LineupAway.Channel.Id == channelId) && m.ReadiedDate == null);
            var matchup = existingMatchup ?? new Matchup();

            // TODO: Make sure we don't keep creating new lineups every time
            matchup.LineupHome = existingTeam ?? new Lineup()
            {
                ChannelId = channelId
            };
            matchup.CreatedDate = DateTime.UtcNow;
            matchup.LineupHome.TeamType = MatchTeamType.Home; // This is necessary to ensure that if a team is unchallenged that they become the Home team after

            if (channel.IsMixChannel)
            {
                matchup.LineupAway = new Lineup()
                {
                    TeamType = MatchTeamType.Away,
                    ChannelId = channelId
                };
            }
            else
            {
                matchup.LineupAway = null;
            }

            if (existingMatchup == null)
            {
                _coachBotContext.Matchups.Add(matchup);
            }

            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, "Teamsheet successfully reset");
        }

        public Matchup GetCurrentMatchupForChannel(ulong channelId)
        {
            if (!_coachBotContext.Matchups.Any(m => (m.LineupHome.Channel.DiscordChannelId == channelId || (m.LineupAway != null && m.LineupAway.Channel.DiscordChannelId == channelId)) && m.ReadiedDate == null))
            {
                var channel = _coachBotContext.Channels.First(c => c.DiscordChannelId == channelId);
                CreateMatchup(channel.Id);
            }

            return GetMatchupQueryable()
                .Where(m => m.ReadiedDate == null)
                .OrderByDescending(m => m.CreatedDate)
                .First(m => m.LineupHome.Channel.DiscordChannelId == channelId || (m.LineupAway != null && m.LineupAway.Channel.DiscordChannelId == channelId));
        }

        public List<Matchup> GetMatchupsForChannel(ulong channelId, bool readiedMatchesOnly = false, int limit = 10)
        {
            return GetMatchupQueryable()
                .Include(m => m.Match)
                    .ThenInclude(m => m.MatchStatistics)
                .Where(m => m.LineupHome.Channel != null && m.LineupAway.Channel != null)
                .Where(m => m.LineupHome.Channel.DiscordChannelId == channelId || m.LineupAway.Channel.DiscordChannelId == channelId)
                .Where(m => !readiedMatchesOnly || m.ReadiedDate != null)
                .OrderByDescending(m => m.CreatedDate)
                .Take(limit)
                .ToList();
        }

        public Matchup GetMatchup(int matchupId)
        {
            return GetMatchupQueryable().Single(m => m.Id == matchupId);
        }

        public List<MatchOutcomeType> GetFormForChannel(ulong channelId, int limit = 5)
        {
            var recentMatches = _coachBotContext.Matchups
                .AsQueryable()
                .Where(m => m.LineupHome.Channel.DiscordChannelId == channelId || m.LineupAway.Channel.DiscordChannelId == channelId)                
                .Where(m => m.IsMixMatch == false)
                .OrderByDescending(m => m.ReadiedDate)
                .Select(m => m.Match)
                .Where(m => m.MatchStatistics != null)
                .Include(m => m.MatchStatistics)
                .Take(5);

            var formList = new List<MatchOutcomeType>();
            foreach (var recentMatch in recentMatches)
            {
                var matchDataTeamType = recentMatch.Matchup.LineupHome.Channel.DiscordChannelId == channelId ? MatchDataTeamType.Home : MatchDataTeamType.Away;
                var matchOutcomeType = recentMatch.MatchStatistics.GetMatchOutcomeTypeForTeam(matchDataTeamType);
                formList.Add(matchOutcomeType);
            }

            return formList;
        }

        public ServiceResponse AddPlayerToLineup(ulong channelId, Player player, string positionName = null, MatchTeamType teamType = MatchTeamType.Home)
        {
            const string GK_POSITION = "GK";
            var matchup = GetCurrentMatchForChannel(channelId);
            if (matchup.LineupHome.PlayerLineupPositions is null) matchup.LineupHome.PlayerLineupPositions = new List<PlayerLineupPosition>();
            var channel = _coachBotContext.Channels.Include(c => c.ChannelPositions).ThenInclude(cp => cp.Position).First(c => c.DiscordChannelId == channelId);
            var team = teamType == MatchTeamType.Home ? matchup.LineupHome : matchup.LineupAway;

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

            // INFO: Allows a player to type !lw in a mix channel and if the home team is full, it will add them to the away team
            if (matchup.IsMixMatch && team != null && team.OccupiedPositions.Any(op => op == position) && teamType == MatchTeamType.Home && matchup.LineupAway != null && matchup.LineupAway.ChannelId == channel.Id)
            {
                team = matchup.LineupAway;
            }

            if (player.DiscordUserId != null && matchup.SignedPlayersAndSubs.Any(sp => sp.DiscordUserId == player.DiscordUserId)) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{player.DisplayName}** is already signed for **{matchup.GetSignedTeam((ulong)player.DiscordUserId)?.Name}**");
            if (matchup.SignedPlayersAndSubs.Any(sp => sp.Name == player.Name)) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{player.DisplayName}** is already signed for **{matchup.GetSignedTeam(player.Name)?.Name}**");
            if (team != null && team.OccupiedPositions.Any(op => op == position)) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{positionName.ToUpper()}** is already filled");
            if (position is null && positionName != null) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{positionName.ToUpper()}** is not a valid position for this team");
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
                || (match.LineupAway != null && match.LineupAway.PlayerSubstitutes.Any(sp => sp.Player.DiscordUserId == player.DiscordUserId)))
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
            var team = match.GetLineup(teamType);

            if (team == null) new ServiceResponse(ServiceResponseStatus.Failure, $"Cannot clear position for a team that does not exist");
            if (!team.Channel.ChannelPositions.Any(cp => cp.Position.Name.ToUpper() == position.ToUpper())) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{position.ToUpper()}** is not a valid position");

            var playerPosition = team.PlayerLineupPositions
                .AsQueryable()
                .Include(p => p.Player)
                .Include(p => p.Lineup)
                    .ThenInclude(p => p.PlayerSubstitutes)
                    .ThenInclude(p => p.Player)
                .Include(p => p.Position)
                .FirstOrDefault(ptp => ptp.Position.Name.ToUpper() == position.ToUpper());

            if (playerPosition != null) team.PlayerLineupPositions.Remove(playerPosition);

            _coachBotContext.SaveChanges();

            if (playerPosition != null && playerPosition.Lineup != null && playerPosition.Lineup.PlayerSubstitutes.Any())
            {
                var substitute = ReplaceWithSubstitute(playerPosition.Position, playerPosition.Lineup);
                return GenerateSubReplacementServiceResponse(substitute, playerPosition.Player);
            }

            return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, $"Cleared position **{position.ToUpper()}**");
        }

        public ServiceResponse RemovePlayerFromMatch(ulong channelId, Player player)
        {
            var match = GetCurrentMatchForChannel(channelId);

            if (match.LineupHome.PlayerLineupPositions.Any(ptp => ptp.Player.Id == player.Id && ptp.Lineup.Channel.DiscordChannelId == channelId))
            {
                var removedPlayer = RemovePlayerFromTeam(match.LineupHome, player);
                if (match.LineupHome.PlayerSubstitutes.Any())
                {
                    var substitute = ReplaceWithSubstitute(removedPlayer.Position, match.LineupHome);
                    return GenerateSubReplacementServiceResponse(substitute, player);
                }
            }
            else if (match.LineupAway != null && match.LineupAway.PlayerLineupPositions.Any(ptp => ptp.Player.Id == player.Id && ptp.Lineup.Channel.DiscordChannelId == channelId))
            {
                var removedPlayer = RemovePlayerFromTeam(match.LineupAway, player);
                if (match.LineupAway.PlayerSubstitutes.Any())
                {
                    var substitute = ReplaceWithSubstitute(removedPlayer.Position, match.LineupAway);
                    return GenerateSubReplacementServiceResponse(substitute, player);
                }
            }
            else
            {
                return new ServiceResponse(ServiceResponseStatus.Failure, $"**{player.DisplayName}** is not signed");
            }

            return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, $"Removed **{player.DisplayName}**");
        }

        public ServiceResponse RemovePlayerGlobally(ulong discordUserId, bool offlineMessage = false, bool respectDuplicityProtection = false)
        {
            var player = _coachBotContext.Players.Single(p => p.DiscordUserId == discordUserId);

            var playerSignings = _coachBotContext.PlayerLineupPositions
                .AsSplitQuery()
                .Include(plp => plp.Lineup)
                    .ThenInclude(l => l.Channel)
                .Include(plp => plp.Lineup)
                    .ThenInclude(plp => plp.PlayerSubstitutes)
                    .ThenInclude(plp => plp.Player)
                .Include(plp => plp.Lineup)
                    .ThenInclude(l => l.HomeMatchup)
                .Include(plp => plp.Lineup)
                    .ThenInclude(l => l.AwayMatchup)
                .Include(plp => plp.Position)
                .Include(plp => plp.Player)
                .Where(plp => plp.Player != null && plp.Player.DiscordUserId != null && (ulong)plp.Player.DiscordUserId == discordUserId)
                .Where(plp => (plp.Lineup.HomeMatchup != null && plp.Lineup.HomeMatchup.ReadiedDate == null) || (plp.Lineup.AwayMatchup != null && plp.Lineup.AwayMatchup.ReadiedDate == null))
                .Where(plp => respectDuplicityProtection == false || plp.Lineup.Channel.DuplicityProtection == true);

            foreach (var signing in playerSignings)
            {
                var channelId = signing.Lineup.Channel.DiscordChannelId;
                var currentChannelMatchup = GetCurrentMatchForChannel(channelId);
                if (currentChannelMatchup != null && signing.Lineup != null && signing.Lineup.Matchup != null && currentChannelMatchup.Id == signing.Lineup.Matchup.Id)
                {
                    _coachBotContext.PlayerLineupPositions.Remove(signing);
                    _coachBotContext.SaveChanges();

                    var embed = offlineMessage ? DiscordEmbedHelper.GenerateEmbed($"Removed {player.DisplayName} from the line-up as they have gone offline", ServiceResponseStatus.Warning)
                        : DiscordEmbedHelper.GenerateEmbed($"Removed **{player.DisplayName}**", ServiceResponseStatus.NegativeSuccess);
                    _discordNotificationService.SendChannelMessage(channelId, embed).Wait();

                    if (signing.Lineup.PlayerSubstitutes.Any())
                    {
                        var sub = ReplaceWithSubstitute(signing.Position, signing.Lineup);
                        _discordNotificationService.SendChannelMessage(channelId, DiscordEmbedHelper.GenerateEmbedFromServiceResponse(GenerateSubReplacementServiceResponse(sub, signing.Player))).Wait();
                    }
                    
                    foreach (var teamEmbed in GenerateTeamList(channelId))
                    {
                        _discordNotificationService.SendChannelMessage(channelId, teamEmbed).Wait();
                    }
                }
            }

            var playerSubSignings = _coachBotContext.PlayerLineupSubstitutes
                .AsSplitQuery()
                .Include(plp => plp.Lineup)
                .ThenInclude(l => l.Channel)
                .Include(plp => plp.Lineup)
                .ThenInclude(l => l.HomeMatchup)
                .Include(plp => plp.Lineup)
                .ThenInclude(l => l.AwayMatchup)
                .Where(plp => plp.Player != null && plp.Player.DiscordUserId != null && (ulong)plp.Player.DiscordUserId == discordUserId)
                .Where(plp => (plp.Lineup.HomeMatchup != null && plp.Lineup.HomeMatchup.ReadiedDate == null) || (plp.Lineup.AwayMatchup != null && plp.Lineup.AwayMatchup.ReadiedDate == null))
                .Where(plp => respectDuplicityProtection == false || plp.Lineup.Channel.DuplicityProtection == true);

            foreach (var signing in playerSubSignings)
            {
                var channelId = signing.Lineup.Channel.DiscordChannelId;
                var currentChannelMatchup = GetCurrentMatchForChannel(channelId);
                if (currentChannelMatchup != null && signing.Lineup != null && signing.Lineup.Matchup != null && currentChannelMatchup.Id == signing.Lineup.Matchup.Id)
                {
                    _coachBotContext.PlayerLineupSubstitutes.Remove(signing);
                    _coachBotContext.SaveChanges();
                    var embed = offlineMessage ? DiscordEmbedHelper.GenerateEmbed($"Removed {player.DisplayName} from the subs bench as they have gone offline", ServiceResponseStatus.Warning)
                        : DiscordEmbedHelper.GenerateEmbed($"Removed **{player.DisplayName}** from the subs bench", ServiceResponseStatus.NegativeSuccess);
                    _discordNotificationService.SendChannelMessage(channelId, embed).Wait();
                    foreach (var teamEmbed in GenerateTeamList(channelId))
                    {
                        _discordNotificationService.SendChannelMessage(channelId, teamEmbed).Wait();
                    }
                }

            }

            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, $"Removed **{player.DisplayName}** from all lineups");
        }

        public ServiceResponse ReadyMatch(ulong channelId, int serverId, out int matchId)
        {
            var matchup = GetCurrentMatchupForChannel(channelId);
            var channel = matchup.LineupHome.Channel;
            var server = _serverService.GetServer(serverId);
            matchId = -1;

            if (matchup.LineupAway == null || matchup.LineupHome == null) return new ServiceResponse(ServiceResponseStatus.Failure, $"There is no opposition set");
            if (matchup.SignedPlayersAndSubs.Count < channel.ChannelPositions.Count) return new ServiceResponse(ServiceResponseStatus.Failure, $"All positions must be filled");
            if (server == null) return new ServiceResponse(ServiceResponseStatus.Failure, $"A valid server must be provided");
            if (server.RegionId != channel.Team.RegionId) return new ServiceResponse(ServiceResponseStatus.Failure, $"The selected server is not in this channels region");
            if (matchup.SignedPlayers.GroupBy(p => p.Id).Where(p => p.Count() > 1).Any()) return new ServiceResponse(ServiceResponseStatus.Failure, $"One or more players is signed for both teams");
            if (_coachBotContext.Matches.Any(m => m.ServerId == serverId && m.KickOff != null && m.KickOff > DateTime.UtcNow && m.KickOff.Value.AddMinutes(-90) < DateTime.UtcNow)) return new ServiceResponse(ServiceResponseStatus.Failure, $"The selected server has a scheduled match within the next 90 minutes");

            var match = new Match()
            {
                ServerId = server.Id,
                TeamHomeId = matchup.LineupHome.Channel.TeamId,
                TeamAwayId = matchup.LineupAway.Channel.TeamId,
                Format = (MatchFormat)channel.ChannelPositions.Count,
                MatchType = server.HasRconPassword ? MatchType.RankedFriendly : MatchType.UnrankedFriendly
            };
            _coachBotContext.Matches.Add(match);
            var savedMatchId = _coachBotContext.Entry(match).Property(m => m.Id).CurrentValue;
            matchup.ReadiedDate = DateTime.UtcNow;
            matchup.MatchId = savedMatchId;
            _coachBotContext.SaveChanges();

            CreateMatchup((int)matchup.LineupHome.ChannelId);
            if (matchup.LineupHome.ChannelId != matchup.LineupAway.ChannelId) CreateMatchup((int)matchup.LineupAway.ChannelId);

            RemovePlayersFromOtherMatchups(matchup);

            matchId = savedMatchId;

            return new ServiceResponse(ServiceResponseStatus.Success, $"Match successfully readied");
        }

        public ServiceResponse Challenge(ulong challengerChannelId, ulong oppositionId, string challengerMention)
        {
            var challenger = _channelService.GetChannelByDiscordId(challengerChannelId);
            var opposition = _channelService.GetChannelByDiscordId(oppositionId);
            var oppositionMatchup = GetCurrentMatchupForChannel(oppositionId);
            var challengerMatchup = GetCurrentMatchupForChannel(challengerChannelId);

            if (challenger.IsMixChannel) return new ServiceResponse(ServiceResponseStatus.Failure, $"Mix channels cannot challenge teams");
            if (!challenger.IsMixChannel && challengerMatchup.LineupAway != null && challengerMatchup.LineupAway.ChannelId != challengerMatchup.LineupHome.ChannelId) return new ServiceResponse(ServiceResponseStatus.Failure, $"You are already challenging **{challengerMatchup.LineupAway.Channel.Team.Name}**");
            if (opposition.IsMixChannel && !oppositionMatchup.IsMixMatch) return new ServiceResponse(ServiceResponseStatus.Failure, $"{opposition.Team.Name} already has opposition challenging");
            if (opposition.IsMixChannel && oppositionMatchup.IsMixMatch && oppositionMatchup.SignedPlayers.Count >= (opposition.ChannelPositions.Count() * 1.75)) return new ServiceResponse(ServiceResponseStatus.Failure, $"{opposition.Team.Name} has an active mix vs mix challenge underway and cannot be challenged presently");
            if (!opposition.IsMixChannel && oppositionMatchup.LineupAwayId != null) return new ServiceResponse(ServiceResponseStatus.Failure, $"{opposition.Team.Name} already has opposition challenging");
            if (!_searchService.GetSearches().Any(s => s.ChannelId == opposition.Id) && !opposition.IsMixChannel) return new ServiceResponse(ServiceResponseStatus.Failure, $"**{opposition.Team.Name}** aren't searching for a team to face");
            if (challengerChannelId == oppositionId) return new ServiceResponse(ServiceResponseStatus.Failure, $"You can't face yourself. Don't waste my time.");
            if (challenger.ChannelPositions.Count() != opposition.ChannelPositions.Count()) return new ServiceResponse(ServiceResponseStatus.Failure, $"Sorry, **{opposition.Team.Name}** are looking for an **{opposition.ChannelPositions.Count()}v{opposition.ChannelPositions.Count()}** match");
            if (Math.Round(challenger.ChannelPositions.Count() * 0.7) > challengerMatchup.LineupHome.OccupiedPositions.Count()) return new ServiceResponse(ServiceResponseStatus.Failure, $"At least **{Math.Round(challenger.ChannelPositions.Count() * 0.7)}** positions must be filled");
            if (challenger.Team.RegionId != opposition.Team.RegionId) return new ServiceResponse(ServiceResponseStatus.Failure, $"You can't challenge opponents from other regions");

            if (opposition.IsMixChannel)
            {
                CombineMixLineups(oppositionMatchup, opposition);
            }

            _searchService.StopSearch(challenger.Id).Wait();
            _searchService.StopSearch(opposition.Id).Wait();
            challengerMatchup.LineupAway = oppositionMatchup.LineupHome;
            challengerMatchup.LineupAway.TeamType = MatchTeamType.Away;
            _coachBotContext.Matchups.Remove(oppositionMatchup);
            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.Success, "Team sucessfully challenged");
        }

        public ServiceResponse Unchallenge(ulong unchallengerChannelId)
        {
            var challenger = _channelService.GetChannelByDiscordId(unchallengerChannelId);
            var matchup = GetCurrentMatchForChannel(unchallengerChannelId);

            if (matchup.LineupAway.ChannelId == null || matchup.LineupAway.ChannelId == 0 || matchup.LineupHome == null || matchup.LineupHome.ChannelId == 0) return new ServiceResponse(ServiceResponseStatus.Failure, "There is no opposition set to unchallenge");
            if (matchup.LineupAway.ChannelId == matchup.LineupHome.ChannelId) return new ServiceResponse(ServiceResponseStatus.Failure, "There is no opposition set to unchallenge");

            CreateMatchup((int)matchup.LineupAway.ChannelId, matchup.LineupAway);
            matchup.LineupAwayId = null;
            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, "Team successfully unchallenged");
        }

        public void SendTeamListToChannel(ulong channelId)
        {
            foreach(var teamEmbed in GenerateTeamList(channelId))
            {
                _discordNotificationService.SendChannelMessage(channelId, teamEmbed).Wait();
            }
        }

        public List<Embed> GenerateTeamList(ulong channelId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var matchup = GetCurrentMatchupForChannel(channelId);

            if (!channel.UseClassicLineup) return GenerateTeamSheet(channelId);

            var teamType = _channelService.GetTeamTypeForChannelTeamType(ChannelTeamType.TeamOne, channelId);
            var teamLists = new List<Embed>() {
                TeamListEmbedFactory.GenerateEmbed(channel, matchup, teamType)
            };

            if (matchup.IsMixMatch)
            {
                var awayTeamList = TeamListEmbedFactory.GenerateEmbed(channel, matchup, MatchTeamType.Away);
                teamLists.Add(awayTeamList);
            }

            return teamLists;
        }

        private List<Embed> GenerateTeamSheet(ulong channelId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var matchup = GetCurrentMatchupForChannel(channelId);

            if (channel.UseClassicLineup) return GenerateTeamList(channelId);

            var teamType = _channelService.GetTeamTypeForChannelTeamType(ChannelTeamType.TeamOne, channelId);
            var teamSheets = new List<Embed>() {
                TeamSheetEmbedFactory.GenerateEmbed(channel, matchup, teamType)
            };

            if (matchup.IsMixMatch)
            {
                var awayTeamSheet = TeamSheetEmbedFactory.GenerateEmbed(channel, matchup, MatchTeamType.Away);
                teamSheets.Add(awayTeamSheet);
            }

            return teamSheets;
        }

        public bool IsPlayerSigned(ulong discordUserId)
        {
            return _coachBotContext.Players.AsQueryable().Where(p => p.DiscordUserId == discordUserId && _coachBotContext.IsPlayerSigned((ulong)p.DiscordUserId)).Any();
        }

        public List<Channel> GetSignedChannelsForPlayer(ulong discordUserId)
        {
            var recentSigningBuffer = DateTime.UtcNow.AddDays(-7);

            return _coachBotContext.PlayerLineupPositions
                .AsQueryable()
                .Where(plp => plp.Player.DiscordUserId == discordUserId)
                .Where(plp => (plp.Lineup.HomeMatchup != null && plp.Lineup.HomeMatchup.ReadiedDate == null) || (plp.Lineup.AwayMatchup != null && plp.Lineup.AwayMatchup.ReadiedDate == null))
                .Where(plp => plp.CreatedDate > recentSigningBuffer)
                .Select(plp => plp.Lineup.Channel)
                .ToList();
        }

        public async void RemovePlayersFromOtherMatchups(Matchup readiedMatchup, bool respectDuplicityProtection = true)
        {
            var signedPlayerIds = readiedMatchup.SignedPlayers.Select(p => p.Id).ToList();
            var otherPlayerSignings = _coachBotContext.PlayerLineupPositions
                .AsQueryable()
                .AsSplitQuery()
                .Include(plp => plp.Lineup)
                    .ThenInclude(l => l.Channel)
                .Include(plp => plp.Lineup)
                    .ThenInclude(plp => plp.PlayerSubstitutes)
                    .ThenInclude(plp => plp.Player)
                .Include(plp => plp.Lineup)
                    .ThenInclude(l => l.HomeMatchup)
                .Include(plp => plp.Lineup)
                    .ThenInclude(l => l.AwayMatchup)
                .Include(plp => plp.Position)
                .Include(plp => plp.Player)
                .Where(ptp => (ptp.Lineup.HomeMatchup != null && ptp.Lineup.HomeMatchup.ReadiedDate == null) || (ptp.Lineup.AwayMatchup != null && ptp.Lineup.AwayMatchup.ReadiedDate == null))
                .Where(ptp => signedPlayerIds.Any(sp => sp == ptp.PlayerId))
                .ToList();

            foreach (var otherPlayerSigning in otherPlayerSignings)
            {
                if (otherPlayerSigning.Lineup.Channel.DuplicityProtection || !respectDuplicityProtection)
                {
                    var channelId = otherPlayerSigning.Lineup.Channel.DiscordChannelId;
                    _coachBotContext.PlayerLineupPositions.Remove(otherPlayerSigning);
                    _coachBotContext.SaveChanges();
                    var message = $":stadium: **{otherPlayerSigning.Player.DisplayName}** has gone to play another match (**{readiedMatchup.LineupHome.Channel.Team.Name} {readiedMatchup.LineupHome.Channel.Team.BadgeEmote}** vs **{readiedMatchup.LineupAway.Channel.Team.BadgeEmote} {readiedMatchup.LineupAway.Channel.Team.Name}**) and has been removed from the lineup.";
                    await _discordNotificationService.SendChannelMessage(channelId, message);

                    if (otherPlayerSigning.Lineup.PlayerSubstitutes.Any())
                    {
                        var sub = ReplaceWithSubstitute(otherPlayerSigning.Position, otherPlayerSigning.Lineup);
                        _discordNotificationService.SendChannelMessage(channelId, DiscordEmbedHelper.GenerateEmbedFromServiceResponse(GenerateSubReplacementServiceResponse(sub, otherPlayerSigning.Player))).Wait();
                    }

                    SendTeamListToChannel(channelId);
                }
                else
                {
                    var message = $":stadium: **{otherPlayerSigning.Player.DisplayName}** has gone to play another match (**{readiedMatchup.LineupHome.Channel.Team.Name} {readiedMatchup.LineupHome.Channel.Team.BadgeEmote}** vs **{readiedMatchup.LineupAway.Channel.Team.BadgeEmote} {readiedMatchup.LineupAway.Channel.Team.Name}**)";
                    await _discordNotificationService.SendChannelMessage(otherPlayerSigning.Lineup.Channel.DiscordChannelId, message);
                }
            }
        }

        #region Private methods
        private Matchup GetCurrentMatchForChannel(ulong channelId)
        {
            return GetMatchupQueryable()
                .Where(m => m.ReadiedDate == null)
                .OrderByDescending(m => m.CreatedDate)
                .First(m => m.LineupHome.Channel.DiscordChannelId == channelId || (m.LineupAway != null && m.LineupAway.Channel.DiscordChannelId == channelId));
        }

        private IQueryable<Matchup> GetMatchupQueryable()
        {
            return _coachBotContext.Matchups
                .AsSplitQuery()
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
                .Include(m => m.LineupHome)
                    .ThenInclude(th => th.Channel)
                    .ThenInclude(c => c.Team)
                    .ThenInclude(t => t.Guild)
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
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.Channel)
                    .ThenInclude(c => c.Team)
                    .ThenInclude(t => t.Guild)
                .Include(m => m.Match);
        }

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
            var sub = team.PlayerSubstitutes.OrderBy(s => s.CreatedDate).Select(p => p.Player).FirstOrDefault();

            if (sub != null)
            {
                RemovePlayerSubstituteFromTeam(team, sub);
                AddPlayerToLineup(team.Channel.DiscordChannelId, sub, position.Name, team.TeamType);
            }

            return sub;
        }

        private ServiceResponse GenerateSubReplacementServiceResponse(Player substitute, Player player)
        {
            return new ServiceResponse(ServiceResponseStatus.Success, $":arrows_counterclockwise:  **Substitution** {Environment.NewLine} {substitute.DisplayName} comes off the bench to replace **{player.DisplayName}**");
        }

        private void CombineMixLineups(Matchup matchup, Channel channel)
        {
            var unoccupiedHomePositions = channel.ChannelPositions.Where(cp => !matchup.LineupHome.OccupiedPositions.Any(op => cp.PositionId == op.Id));
            var transferrablePositions = unoccupiedHomePositions.Where(up => matchup.LineupAway.OccupiedPositions.Any(op => op.Id == up.PositionId));

            if (transferrablePositions.Any())
            {
                foreach (var position in transferrablePositions)
                {
                    var awayPlayer = matchup.LineupAway.PlayerLineupPositions.FirstOrDefault(ap => ap.PositionId == position.PositionId);
                    if (awayPlayer != null)
                    {
                        var playerTeamPosition = new PlayerLineupPosition()
                        {
                            PlayerId = awayPlayer.PlayerId,
                            LineupId = (int)matchup.LineupHomeId,
                            PositionId = awayPlayer.PositionId
                        };
                        _coachBotContext.PlayerLineupPositions.Add(playerTeamPosition);
                    }
                }
            }
        }

        #endregion Private methods

    }
}
