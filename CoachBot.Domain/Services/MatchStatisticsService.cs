using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Helpers;
using CoachBot.Domain.Model;
using CoachBot.Model;
using CoachBot.Shared.Model;
using Discord;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace CoachBot.Domain.Services
{
    public class MatchStatisticsService
    {
        private readonly CoachBotContext _coachBotContext;
        private readonly FantasyService _fantasyService;
        private readonly DiscordNotificationService _discordNotificationService;
        private readonly TournamentService _tournamentService;
        private readonly Config _config;

        public MatchStatisticsService(
            CoachBotContext coachBotContext,
            FantasyService fantasyService,
            DiscordNotificationService discordNotificationService, 
            TournamentService tournamentService,
            Config config
        )
        {
            _coachBotContext = coachBotContext;
            _fantasyService = fantasyService;
            _discordNotificationService = discordNotificationService;
            _tournamentService = tournamentService;
            _config = config;
        }

        public void SaveMatchData(MatchData matchData, string token, string sourceAddress, int matchId, bool manualSave = false)
        {
            var match = _coachBotContext.Matches.Single(m => m.Id == matchId);
            match.MatchStatistics = new MatchStatistics()
            {
                MatchData = matchData,
                Token = token,
                SourceAddress = sourceAddress
            };

            if (matchData.IsValid(match, manualSave))
            {
                match.KickOff = match.KickOff ?? match.MatchStatistics.KickOff;
                match.MapId = GetOrCreateMap(matchData.MatchInfo.MapName);
                _coachBotContext.SaveChanges();
                GenerateStatsForMatch(match.Id);
            }
        }

        public int SaveUnlinkedMatchData(MatchData matchData, string token, string sourceAddress)
        {
            var matchStatistics = new MatchStatistics()
            {
                MatchData = matchData,
                Token = token,
                SourceAddress = sourceAddress
            };

            _coachBotContext.MatchStatistics.Add(matchStatistics);
            _coachBotContext.SaveChanges();

            return matchStatistics.Id;
        }

        public List<MatchStatistics> GetUnlinkedMatchData()
        {
            var unlinkedMatchStatistics = _coachBotContext.MatchStatistics.AsQueryable().Where(ms => !_coachBotContext.Matches.Any(m => m.MatchStatisticsId == ms.Id)).ToList();

            return unlinkedMatchStatistics.Where(ms => ms?.MatchData?.Players.Count > 5).ToList();
        }

        public void UnlinkMatchStatistics(int matchStatisticsId)
        {
            var match = _coachBotContext.Matches.Single(m => m.MatchStatisticsId == matchStatisticsId);

            match.MatchStatisticsId = null;
            match.KickOff = null;

            _coachBotContext.FantasyPlayerPhases.RemoveRange(_coachBotContext.FantasyPlayerPhases.AsQueryable().Where(m => m.PlayerMatchStatistics.Match.Id == match.Id));
            _coachBotContext.PlayerMatchStatistics.RemoveRange(_coachBotContext.PlayerMatchStatistics.AsQueryable().Where(m => m.MatchId == match.Id));
            _coachBotContext.PlayerPositionMatchStatistics.RemoveRange(_coachBotContext.PlayerPositionMatchStatistics.AsQueryable().Where(m => m.MatchId == match.Id));
            _coachBotContext.TeamMatchStatistics.RemoveRange(_coachBotContext.TeamMatchStatistics.AsQueryable().Where(m => m.MatchId == match.Id));

            _coachBotContext.SaveChanges();
        }

        public void SwapTeams(int matchStatisticsId)
        {
            var match = _coachBotContext.Matches.Single(m => m.MatchStatisticsId == matchStatisticsId);

            var playerPositionMatchStatistics = _coachBotContext.PlayerPositionMatchStatistics.AsQueryable().Where(p => p.MatchId == match.Id);
            foreach(var playerPosition in playerPositionMatchStatistics)
            {
                playerPosition.TeamId = playerPosition.TeamId == match.TeamHomeId ? match.TeamAwayId : match.TeamHomeId;
                playerPosition.MatchOutcome = playerPosition.MatchOutcome == MatchOutcomeType.Draw ? MatchOutcomeType.Draw : playerPosition.MatchOutcome == MatchOutcomeType.Loss ? MatchOutcomeType.Win : MatchOutcomeType.Loss;
            }

            var playerMatchStatistics = _coachBotContext.PlayerMatchStatistics.AsQueryable().Where(p => p.MatchId == match.Id);
            foreach (var player in playerMatchStatistics)
            {
                player.TeamId = player.TeamId == match.TeamHomeId ? match.TeamAwayId : match.TeamHomeId;
                player.MatchOutcome = player.MatchOutcome == MatchOutcomeType.Draw ? MatchOutcomeType.Draw : player.MatchOutcome == MatchOutcomeType.Loss ? MatchOutcomeType.Win : MatchOutcomeType.Loss;
            }

            var teamMatchStatistics = _coachBotContext.TeamMatchStatistics.AsQueryable().Where(t => t.Match.Id == match.Id);
            foreach(var team in teamMatchStatistics)
            {
                team.TeamId = team.TeamId == match.TeamHomeId ? match.TeamAwayId : match.TeamAwayId;
                team.MatchOutcome = team.MatchOutcome == MatchOutcomeType.Draw ? MatchOutcomeType.Draw : team.MatchOutcome == MatchOutcomeType.Loss ? MatchOutcomeType.Win : MatchOutcomeType.Loss;
            }

            var currentHomeTeamId = match.TeamHomeId;
            var currentAwayTeamId = match.TeamAwayId;

            match.TeamHomeId = currentAwayTeamId;
            match.TeamAwayId = currentHomeTeamId;

            _coachBotContext.SaveChanges();

            GenerateTeamForm();
        }

        public void CreateMatchFromMatchData(int matchStatisticsId)
        {
            var matchStatistics = _coachBotContext.MatchStatistics.Single(m => m.Id == matchStatisticsId);
            var base64EncodedBytes = Convert.FromBase64String(matchStatistics.Token);
            var token = Encoding.UTF8.GetString(base64EncodedBytes);
            var serverAddress = token.Split("_")[0];
            var matchId = int.Parse(token.Split("_")[1]);
            var homeTeamCode = token.Split("_")[2];
            var awayTeamCode = token.Split("_")[3];

            var server = _coachBotContext.Servers.FirstOrDefault(s => s.Address == serverAddress);

            var match = new Match()
            {
                ServerId = server?.Id,
                MatchStatisticsId = matchStatisticsId,
                KickOff = matchStatistics.KickOff,
                Format = GetMatchFormatFromMapName(matchStatistics.MatchData.MatchInfo.MapName),
                MatchType = MatchType.RankedFriendly,
                MapId = GetOrCreateMap(matchStatistics.MatchData.MatchInfo.MapName),
                TeamHomeId = _coachBotContext.Teams.First(t => t.TeamCode == homeTeamCode && t.RegionId == server.RegionId).Id,
                TeamAwayId = _coachBotContext.Teams.First(t => t.TeamCode == awayTeamCode && t.RegionId == server.RegionId).Id
            };

            _coachBotContext.Matches.Add(match);
            _coachBotContext.SaveChanges();

            GenerateStatsForMatch(match.Id);

            if (_config.BotConfig.EnableBotHubIntegration)
            {
                var originalMatch = _coachBotContext.Matches
                    .Include(m => m.Matchup)
                        .ThenInclude(m => m.LineupHome)
                        .ThenInclude(l => l.Channel)
                    .Include(m => m.Matchup)
                        .ThenInclude(m => m.LineupAway)
                        .ThenInclude(l => l.Channel)
                    .Include(m => m.Server)
                    .SingleOrDefault(m => m.Id == matchId);

                if (originalMatch != null && originalMatch.Matchup != null)
                {
                    var resultMatch = _coachBotContext.Matches
                        .Include(m => m.MatchStatistics)
                        .Include(m => m.TeamHome)
                        .Include(m => m.TeamAway)
                        .Single(m => m.Id == match.Id);
                    var resultEmbed = GetResultEmbed(match);
                    var homeChannelId = originalMatch.Matchup.LineupHome.Channel.DiscordChannelId;
                    var awayChannelId = originalMatch.Matchup.LineupAway.Channel.DiscordChannelId;
                    _discordNotificationService.SendChannelMessage(homeChannelId, resultEmbed).Wait();
                    if (awayChannelId != homeChannelId)
                    {
                        _discordNotificationService.SendChannelMessage(awayChannelId, resultEmbed).Wait();
                    }
                }
            }
        }

        public void CreateMatchResultOverride(int matchId, int homeGoals, int awayGoals)
        {
            var match = _coachBotContext.Matches.Include(m => m.MatchStatistics).Single(m => m.Id == matchId);

            match.MatchStatistics = match.MatchStatistics ?? new MatchStatistics();
            match.MatchStatistics.HomeGoals = homeGoals;
            match.MatchStatistics.AwayGoals = awayGoals;

            _coachBotContext.SaveChanges();
        }

        private MatchFormat GetMatchFormatFromMapName(string mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName) || !mapName.Contains('v'))
            {
                return MatchFormat.EightVsEight;
            }

            return (MatchFormat)int.Parse(mapName.Split('v').First());
        }

        public void GenerateStatsForMatch(int matchId)
        {
            var match = _coachBotContext.Matches
                .Include(m => m.MatchStatistics)
                .Include(m => m.TeamHome)
                .Include(m => m.TeamAway)
                .Include(m => m.Matchup)
                    .ThenInclude(m => m.LineupHome)
                    .ThenInclude(l => l.Channel)
                .Include(m => m.Matchup)
                    .ThenInclude(m => m.LineupAway)
                    .ThenInclude(l => l.Channel)
                .Include(m => m.Server)
                .Single(m => m.Id == matchId);

            GeneratePlayerMatchStatistics(match);
            GenerateTeamMatchStatistics(match);
            GeneratePlayerOfTheMatch(match);
            GenerateTeamForm();
            if (match.TournamentId.HasValue && match.TournamentId > 0)
            {
                var tournamentPhaseId = _coachBotContext.TournamentGroupMatches.Single(m => m.MatchId == match.Id).TournamentPhaseId;
                _fantasyService.GenerateFantasyPhaseSnapshots(tournamentPhaseId);
                _tournamentService.ManageTournamentProgress((int)match.TournamentId, match.Id);
            }

            _discordNotificationService.SendAuditChannelMessage($"New match statistics uploaded for {match.TeamHome.Name} vs {match.TeamAway.Name}").Wait();

            if (_config.BotConfig.EnableBotHubIntegration)
            {
                var resultEmbed = GetResultEmbed(match);

                _discordNotificationService.SendChannelMessage(_config.DiscordConfig.AuditChannelId, GetResultDetailedEmbed(match)).Wait();
                _discordNotificationService.SendChannelMessage(_config.DiscordConfig.ResultStreamChannelId, GetResultDetailedEmbed(match)).Wait();

                if (match.Matchup != null)
                {
                    var homeChannelId = match.Matchup.LineupHome.Channel.DiscordChannelId;
                    var awayChannelId = match.Matchup.LineupAway.Channel.DiscordChannelId;
                    _discordNotificationService.SendChannelMessage(homeChannelId, resultEmbed).Wait();
                    if (awayChannelId != homeChannelId)
                    {
                        _discordNotificationService.SendChannelMessage(awayChannelId, resultEmbed).Wait();
                    }
                }
            }
        }

        private Embed GetResultEmbed(Match match)
        {
            return new EmbedBuilder()
                    .WithTitle($"FULL TIME: {match.TeamHome.Name} {match.TeamHome.BadgeEmote} {match.MatchStatistics.MatchGoalsHome} - {match.MatchStatistics.MatchGoalsAway} {match.TeamAway.BadgeEmote} {match.TeamAway.Name}")
                    .WithDescription($"The match overview is now available to view: https://{_config.WebServerConfig.ClientUrl}match-overview/{match.Id}")
                    .WithColor(new Color(254, 254, 254))
                    .Build();
        }

        private Embed GetResultDetailedEmbed(Match match)
        {
            return new EmbedBuilder()
                    .WithTitle($"FULL TIME: {match.TeamHome.Name} {match.TeamHome.BadgeEmote} {match.MatchStatistics.MatchGoalsHome} - {match.MatchStatistics.MatchGoalsAway} {match.TeamAway.BadgeEmote} {match.TeamAway.Name}")
                    .WithColor(new Color(254, 254, 254))
                    .AddField(new EmbedFieldBuilder().WithIsInline(true).WithName("Server").WithValue($"{match.Server.Name}"))
                    .AddField(new EmbedFieldBuilder().WithIsInline(true).WithName("Format").WithValue($"{(int)match.Format}v{(int)match.Format}"))
                    .AddField(new EmbedFieldBuilder().WithIsInline(true).WithName("Type").WithValue($"{GetMatchType(match.MatchType)}"))
                    .AddField(new EmbedFieldBuilder().WithIsInline(false).WithName("Overview").WithValue($"https://{_config.WebServerConfig.ClientUrl}match-overview/{match.Id}"))
                    .WithCurrentTimestamp()
                    .Build();

            string GetMatchType(MatchType matchType)
            {
                switch(matchType)
                {
                    case MatchType.Competition:
                        return "Competition";
                    default:
                        return "Friendly";
                }
            }
        }

        public Model.Dtos.PagedResult<TeamStatisticTotals> GetTeamStatistics(int page, int pageSize, string sortOrder, TeamStatisticsFilters filters)
        {
            return GetTeamStatisticTotals(filters).GetPaged(page, pageSize, sortOrder);
        }

        public Model.Dtos.PagedResult<PlayerStatisticTotals> GetPlayerStatistics(int page, int pageSize, string sortOrder, PlayerStatisticFilters filters)
        {
            return GetPlayerStatisticTotals(filters).GetPaged(page, pageSize, sortOrder);
        }

        public Model.Dtos.PagedResult<PlayerStatisticTotals> GetPlayerMatchStatisticsTotals(int page, int pageSize, string sortOrder, PlayerStatisticFilters filters)
        {
            return GetPlayerMatchStatisticTotals(filters).GetPaged(page, pageSize, sortOrder);
        }

        public Model.Dtos.PagedResult<PlayerPositionMatchStatistics> GetPlayerPositionMatchStatistics(int page, int pageSize, string sortOrder, PlayerStatisticFilters filters)
        {
            return GetPlayerPositionMatchStatisticsQueryable(filters).GetPaged(page, pageSize, sortOrder);
        }

        public Model.Dtos.PagedResult<PlayerMatchStatistics> GetPlayerMatchStatistics(int page, int pageSize, string sortOrder, PlayerStatisticFilters filters)
        {
            return GetPlayerMatchStatisticsQueryable(filters).GetPaged(page, pageSize, sortOrder);
        }

        public Model.Dtos.PagedResult<TeamMatchStatistics> GeTeamMatchStatistics(int page, int pageSize, string sortOrder, TeamStatisticsFilters filters)
        {
            return GetTeamMatchStatisticsQueryable(filters).GetPaged(page, pageSize, sortOrder);
        }

        public List<PlayerTeamStatisticsTotals> GetPlayerTeamStatistics(int? playerId = null, int? teamId = null, bool activeOnly = false)
        {
            var playerTeams = _coachBotContext.PlayerTeams
                .AsNoTracking()
                .Include(p => p.Team)
                    .ThenInclude(p => p.BadgeImage)
                .Include(p => p.Player)
                    .ThenInclude(p => p.Country)
                .Where(p => playerId == null || p.PlayerId == playerId)
                .Where(p => teamId == null || p.TeamId == teamId)
                .Where(p => !p.IsPending)
                .Where(p => !activeOnly || p.LeaveDate == null)
                .OrderBy(p => p.JoinDate);

            if (activeOnly)
            {
                playerTeams = playerTeams.OrderByDescending(pt => pt.TeamRole).ThenBy(pt => pt.JoinDate);
            }

            var allPlayerTeamStatisticTotals = new List<PlayerTeamStatisticsTotals>();

            foreach (var playerTeam in playerTeams.Take(40))
            {
                var filter = new PlayerStatisticFilters()
                {
                    PlayerId = playerTeam.PlayerId,
                    TeamId = playerTeam.TeamId,
                    DateFrom = playerTeam.JoinDate,
                    DateTo = playerTeam.LeaveDate
                };
                var statistics = GetPlayerStatisticTotals(filter).FirstOrDefault();
                var playerTeamStatisticsTotals = new PlayerTeamStatisticsTotals()
                {
                    PlayerTeam = MapToSimplePlayerTeamInstance(playerTeam),
                    Position = GetMostCommonPosition(playerTeam)
                };

                if (statistics != null)
                {
                    playerTeamStatisticsTotals.Appearances = statistics.Appearances;
                    playerTeamStatisticsTotals.Goals = statistics.Goals;
                    playerTeamStatisticsTotals.Assists = statistics.Assists;
                    playerTeamStatisticsTotals.YellowCards = statistics.YellowCards;
                    playerTeamStatisticsTotals.RedCards = statistics.RedCards;
                }

                allPlayerTeamStatisticTotals.Add(playerTeamStatisticsTotals);
            }

            return allPlayerTeamStatisticTotals;
        }

        public void GenerateStatistics()
        {
            var matches = _coachBotContext.Matches
                .Include(m => m.MatchStatistics)
                .Include(m => m.TeamHome)
                .Include(m => m.TeamAway)
                .Where(m => m.MatchStatistics != null && _coachBotContext.TeamMatchStatistics.Any(tm => tm.MatchId == m.Id && tm.PossessionPercentage == 0));

            foreach (var match in matches)
            {
                GeneratePlayerMatchStatisticsGoalsConceded(match);
                GenerateTeamMatchStatisticsPossession(match);
            }
        }

        public List<MatchDayTotals> GetPlayerMatchDayTotals(int playerId)
        {
            return _coachBotContext.PlayerMatchStatistics
                .AsNoTracking()
                .Where(p => p.PlayerId == playerId)
                .Where(p => p.Match.KickOff != null && p.Match.KickOff.Value > DateTime.UtcNow.AddYears(-1))
                .GroupBy(p => p.Match.KickOff.Value.Date)
                .Select(s => new MatchDayTotals()
                {
                    Matches = s.Count(),
                    MatchDayDate = s.Key
                }).ToList();
        }

        public List<MatchDayTotals> GetTeamMatchDayTotals(int teamId)
        {
            return _coachBotContext.TeamMatchStatistics
                .AsNoTracking()
                .Where(t => t.TeamId == teamId)
                .Where(t => t.Match.KickOff != null && t.Match.KickOff.Value > DateTime.UtcNow.AddYears(-1))
                .GroupBy(t => t.Match.KickOff.Value.Date)
                .Select(s => new MatchDayTotals()
                {
                    Matches = s.Count(),
                    MatchDayDate = s.Key
                }).ToList();
        }

        public PlayerOfTheMatchStatistics GetPlayerOfTheMatch(int matchId)
        {
            var match = _coachBotContext.Matches.Single(m => m.Id == matchId);
            var playerMatchPositionStatistics = _coachBotContext.PlayerPositionMatchStatistics.Include(p => p.Position).Where(p => p.MatchId == matchId && p.PlayerId == match.PlayerOfTheMatchId);
            var mainPositionGroup = PositionGroupHelper.DeterminePositionGroup(playerMatchPositionStatistics);

            return _coachBotContext.PlayerMatchStatistics
                .AsQueryable()
                .OrderByDescending(p => p.SecondsPlayed)
                .Where(p => p.PlayerId == match.PlayerOfTheMatchId && p.MatchId == matchId)
                .Select(p => new PlayerOfTheMatchStatistics()
                {
                    PlayerId = p.PlayerId,
                    PlayerName = p.Player.Name,
                    Goals = p.Goals,
                    Assists = p.Assists,
                    GoalsConceded = p.GoalsConceded,
                    Interceptions = p.Interceptions,
                    KeeperSaves = p.KeeperSaves,
                    PassCompletion = (int)(Convert.ToDouble(p.Passes) > 0 ? (Convert.ToDouble(p.PassesCompleted) / Convert.ToDouble(p.Passes)) * 100 : 0),
                    PositionGroup = mainPositionGroup
                }).FirstOrDefault();
        }

        public void GenerateTeamForm()
        {
            foreach (var teamId in _coachBotContext.TeamMatchStatistics.AsQueryable().Select(t => t.TeamId).Distinct())
            {
                var team = _coachBotContext.Teams.Include(t => t.TeamMatchStatistics).Single(t => t.Id == teamId);
                team.Form = team.TeamMatchStatistics.OrderByDescending(t => t.CreatedDate).Take(5).Select(m => m.MatchOutcome).ToList();
                team.Form.Reverse();
            }
            _coachBotContext.SaveChanges();
        }

        #region Private Methods
        private void GeneratePlayerMatchStatisticsGoalsConceded(Match match)
        {
            foreach (var matchDataPlayer in match.MatchStatistics.MatchData.Players.Where(p => p.MatchPeriodData != null && p.MatchPeriodData.Any()))
            {
                if (matchDataPlayer.Info.SteamId == "BOT") continue;

                var player = GetOrCreatePlayer(matchDataPlayer);
                foreach (var teamType in matchDataPlayer.MatchPeriodData.Select(m => m.Info.Team).Distinct())
                {
                    var isHomeTeam = teamType == MatchDataSideConstants.Home;
                    var team = isHomeTeam ? match.TeamHome : match.TeamAway;
                    foreach (var position in matchDataPlayer.MatchPeriodData.Where(m => m.Info.Team == teamType).Select(m => m.Info.Position).Distinct())
                    {
                        var positionId = _coachBotContext.Positions.AsQueryable().Where(p => p.Name == position).Select(p => p.Id).FirstOrDefault();
                        var playerPositionMatchStatistics = new PlayerPositionMatchStatistics()
                        {
                            PlayerId = player.Id,
                            MatchId = match.Id,
                            MatchTeamType = isHomeTeam ? MatchTeamType.Home : MatchTeamType.Away,
                            TeamId = team.Id,
                            PositionId = positionId > 0 ? positionId : _coachBotContext.Positions.FirstOrDefault().Id,
                            MatchOutcome = match.MatchStatistics.GetMatchOutcomeTypeForTeam(isHomeTeam ? MatchDataTeamType.Home : MatchDataTeamType.Away),
                            SecondsPlayed = matchDataPlayer.GetPlayerPositionSeconds(teamType, position),
                            PossessionPercentage = match.MatchStatistics.MatchData.GetPlayerPositionPossession(matchDataPlayer, teamType, position),
                            Nickname = matchDataPlayer.Info.Name,
                            Substitute = matchDataPlayer.WasPlayerSubstitute(teamType, position)
                        };
                        playerPositionMatchStatistics.AddMatchDataStatistics(matchDataPlayer.GetPlayerTotals(teamType, position));
                        if (position.ToUpper() != "GK")
                        {
                            playerPositionMatchStatistics.GoalsConceded = match.MatchStatistics.MatchData.GetPlayerPositionGoalsConceded(matchDataPlayer, teamType, position);
                        }
                        if (playerPositionMatchStatistics.SecondsPlayed > 0)
                        {
                            var foundStats = _coachBotContext.PlayerPositionMatchStatistics.FirstOrDefault(
                                p => p.MatchId == playerPositionMatchStatistics.MatchId
                                && p.PlayerId == playerPositionMatchStatistics.PlayerId
                                && p.TeamId == playerPositionMatchStatistics.TeamId
                                && p.PositionId == playerPositionMatchStatistics.PositionId
                                && p.MatchTeamType == playerPositionMatchStatistics.MatchTeamType
                            );
                            if (foundStats != null)
                            {
                                foundStats.GoalsConceded = playerPositionMatchStatistics.GoalsConceded;
                                _coachBotContext.SaveChanges();
                            }
                        }
                    }
                    var playerMatchStatistics = new PlayerMatchStatistics()
                    {
                        PlayerId = player.Id,
                        MatchId = match.Id,
                        TeamId = team.Id,
                        MatchTeamType = isHomeTeam ? MatchTeamType.Home : MatchTeamType.Away,
                        MatchOutcome = match.MatchStatistics.GetMatchOutcomeTypeForTeam(isHomeTeam ? MatchDataTeamType.Home : MatchDataTeamType.Away),
                        SecondsPlayed = matchDataPlayer.GetPlayerSeconds(teamType),
                        PossessionPercentage = match.MatchStatistics.MatchData.GetPlayerPossession(matchDataPlayer),
                        Nickname = matchDataPlayer.Info.Name,
                        Substitute = matchDataPlayer.WasPlayerSubstitute(teamType)
                    };
                    playerMatchStatistics.AddMatchDataStatistics(matchDataPlayer.GetMatchStatisticsPlayerTotal());
                    playerMatchStatistics.GoalsConceded = match.MatchStatistics.MatchData.GetPlayerGoalsConceded(matchDataPlayer, teamType);
                    if (playerMatchStatistics.SecondsPlayed > 0)
                    {
                        var foundStats = _coachBotContext.PlayerMatchStatistics.FirstOrDefault(
                            p => p.MatchId == playerMatchStatistics.MatchId
                            && p.PlayerId == playerMatchStatistics.PlayerId
                            && p.TeamId == playerMatchStatistics.TeamId
                            && p.MatchTeamType == playerMatchStatistics.MatchTeamType
                        );
                        if (foundStats != null)
                        {
                            foundStats.GoalsConceded = playerMatchStatistics.GoalsConceded;
                            _coachBotContext.SaveChanges();
                        }
                    }
                }
            }
        }

        private void GenerateTeamMatchStatisticsPossession(Match match)
        {
            foreach (var matchDataTeam in match.MatchStatistics.MatchData.Teams)
            {
                var isHomeTeam = matchDataTeam.MatchTotal.Side == MatchDataSideConstants.Home;
                var team = isHomeTeam ? match.TeamHome : match.TeamAway;
                var teamMatchStatistics = new TeamMatchStatistics()
                {
                    MatchId = match.Id,
                    MatchTeamType = isHomeTeam ? MatchTeamType.Home : MatchTeamType.Away,
                    TeamId = team.Id,
                    MatchOutcome = match.MatchStatistics.GetMatchOutcomeTypeForTeam(isHomeTeam ? MatchDataTeamType.Home : MatchDataTeamType.Away),
                    PossessionPercentage = match.MatchStatistics.MatchData.GetTeamPosession(isHomeTeam ? MatchDataTeamType.Home : MatchDataTeamType.Away),
                    TeamName = matchDataTeam.MatchTotal.Name
                };
                teamMatchStatistics.AddMatchDataStatistics(matchDataTeam.MatchTotal.Statistics);
                var foundStats = _coachBotContext.TeamMatchStatistics.FirstOrDefault(p => 
                    p.MatchId == teamMatchStatistics.MatchId
                    && p.TeamId == teamMatchStatistics.TeamId
                    && p.MatchTeamType == teamMatchStatistics.MatchTeamType
                );
                if (foundStats != null)
                {
                    foundStats.PossessionPercentage = teamMatchStatistics.PossessionPercentage;
                    _coachBotContext.SaveChanges();
                }
            }
        }

        private void GeneratePlayerMatchStatistics(Match match)
        {
            foreach (var matchDataPlayer in match.MatchStatistics.MatchData.Players.Where(p => p.MatchPeriodData != null && p.MatchPeriodData.Any()))
            {
                if (matchDataPlayer.Info.SteamId == "BOT") continue;

                var player = GetOrCreatePlayer(matchDataPlayer);
                foreach (var teamType in matchDataPlayer.MatchPeriodData.Select(m => m.Info.Team).Distinct())
                {
                    var isHomeTeam = teamType == MatchDataSideConstants.Home;
                    var team = isHomeTeam ? match.TeamHome : match.TeamAway;
                    foreach (var position in matchDataPlayer.MatchPeriodData.Where(m => m.Info.Team == teamType).Select(m => m.Info.Position).Distinct())
                    {
                        var positionId = _coachBotContext.Positions.AsQueryable().Where(p => p.Name == position).Select(p => p.Id).FirstOrDefault();
                        var playerPositionMatchStatistics = new PlayerPositionMatchStatistics()
                        {
                            PlayerId = player.Id,
                            MatchId = match.Id,
                            MatchTeamType = isHomeTeam ? MatchTeamType.Home : MatchTeamType.Away,
                            TeamId = team.Id,
                            PositionId = positionId > 0 ? positionId : _coachBotContext.Positions.FirstOrDefault().Id,
                            MatchOutcome = match.MatchStatistics.GetMatchOutcomeTypeForTeam(isHomeTeam ? MatchDataTeamType.Home : MatchDataTeamType.Away),
                            SecondsPlayed = matchDataPlayer.GetPlayerPositionSeconds(teamType, position),
                            PossessionPercentage = match.MatchStatistics.MatchData.GetPlayerPositionPossession(matchDataPlayer, teamType, position),
                            Nickname = matchDataPlayer.Info.Name,
                            Substitute = matchDataPlayer.WasPlayerSubstitute(teamType, position)
                        };
                        playerPositionMatchStatistics.AddMatchDataStatistics(matchDataPlayer.GetPlayerTotals(teamType, position));
                        if (position.ToUpper() != "GK")
                        {
                            playerPositionMatchStatistics.GoalsConceded = match.MatchStatistics.MatchData.GetPlayerPositionGoalsConceded(matchDataPlayer, teamType, position);
                        }
                        if (playerPositionMatchStatistics.SecondsPlayed > 0)
                        {
                            _coachBotContext.PlayerPositionMatchStatistics.Add(playerPositionMatchStatistics);
                        }
                    }
                    var playerMatchStatistics = new PlayerMatchStatistics()
                    {
                        PlayerId = player.Id,
                        MatchId = match.Id,
                        TeamId = team.Id,
                        MatchTeamType = isHomeTeam ? MatchTeamType.Home : MatchTeamType.Away,
                        MatchOutcome = match.MatchStatistics.GetMatchOutcomeTypeForTeam(isHomeTeam ? MatchDataTeamType.Home : MatchDataTeamType.Away),
                        SecondsPlayed = matchDataPlayer.GetPlayerSeconds(teamType),
                        PossessionPercentage = match.MatchStatistics.MatchData.GetPlayerPossession(matchDataPlayer),
                        Nickname = matchDataPlayer.Info.Name,
                        Substitute = matchDataPlayer.WasPlayerSubstitute(teamType)
                    };
                    playerMatchStatistics.AddMatchDataStatistics(matchDataPlayer.GetMatchStatisticsPlayerTotal());
                    playerMatchStatistics.GoalsConceded = match.MatchStatistics.MatchData.GetPlayerGoalsConceded(matchDataPlayer, teamType);
                    if (playerMatchStatistics.SecondsPlayed > 0)
                    {
                        _coachBotContext.PlayerMatchStatistics.Add(playerMatchStatistics);
                    }
                    _coachBotContext.SaveChanges();
                }
            }
        }

        private void GenerateTeamMatchStatistics(Match match)
        {
            foreach (var matchDataTeam in match.MatchStatistics.MatchData.Teams)
            {
                var isHomeTeam = matchDataTeam.MatchTotal.Side == MatchDataSideConstants.Home;
                var team = isHomeTeam ? match.TeamHome : match.TeamAway;
                var teamMatchStatistics = new TeamMatchStatistics()
                {
                    MatchId = match.Id,
                    MatchTeamType = isHomeTeam ? MatchTeamType.Home : MatchTeamType.Away,
                    TeamId = team.Id,
                    MatchOutcome = match.MatchStatistics.GetMatchOutcomeTypeForTeam(isHomeTeam ? MatchDataTeamType.Home : MatchDataTeamType.Away),
                    PossessionPercentage = match.MatchStatistics.MatchData.GetTeamPosession(isHomeTeam ? MatchDataTeamType.Home : MatchDataTeamType.Away),
                    TeamName = matchDataTeam.MatchTotal.Name
                };
                teamMatchStatistics.AddMatchDataStatistics(matchDataTeam.MatchTotal.Statistics);
                _coachBotContext.TeamMatchStatistics.Add(teamMatchStatistics);
                _coachBotContext.SaveChanges();
            }
        }

        private void GeneratePlayerOfTheMatch(Match match)
        {
            var playerMatchStatistics = _coachBotContext.PlayerMatchStatistics.Include(p => p.Player).Where(p => p.MatchId == match.Id);
            var playerPoints = new List<Tuple<Player, int>>();
            foreach(var player in playerMatchStatistics)
            {
                var playerPositionMatchStatistics = _coachBotContext.PlayerPositionMatchStatistics.Include(p => p.Position).Where(p => p.PlayerId == player.PlayerId && p.MatchId == match.Id);
                var fantasyPoints = _fantasyService.CalculateFantasyPoints(player, playerPositionMatchStatistics);
                var mainPositionGroup = PositionGroupHelper.DeterminePositionGroup(playerPositionMatchStatistics);

                playerPoints.Add(new Tuple<Player, int>(player.Player, fantasyPoints));
            }

            var playerOfTheMatch = playerPoints.OrderByDescending(p => p.Item2).Select(p => p.Item1).FirstOrDefault();

            if (playerOfTheMatch != null)
            {
                match.PlayerOfTheMatchId = playerOfTheMatch.Id;
                _coachBotContext.SaveChanges();
            }
        }

        private IQueryable<PlayerPositionMatchStatistics> GetPlayerPositionMatchStatisticsQueryable(PlayerStatisticFilters filters)
        {
            return _coachBotContext.PlayerPositionMatchStatistics
                .AsNoTracking()
                .Where(p => filters.MatchId == null || p.MatchId == filters.MatchId)
                .Where(p => filters.IncludeSubstituteAppearances || !p.Substitute)
                .Where(p => filters.PlayerId == null || p.PlayerId == filters.PlayerId)
                .Where(p => filters.TeamId == null || p.TeamId == filters.TeamId)
                .Where(p => filters.MatchTeamType == null || p.MatchTeamType == filters.MatchTeamType)
                .Where(p => filters.PositionId == null || p.PositionId == filters.PositionId)
                .Where(p => filters.RegionId == null || p.Team.RegionId == filters.RegionId)
                .Where(p => filters.TournamentId == null || p.Match.TournamentId == filters.TournamentId)
                .Where(p => filters.MatchOutcome == null || p.MatchOutcome == filters.MatchOutcome)
                .Where(p => filters.MatchFormat == null || p.Match.Format == filters.MatchFormat)
                .Where(p => string.IsNullOrWhiteSpace(filters.PlayerName) || p.Player.Name.Contains(filters.PlayerName))
                .Where(p => filters.TimePeriod != StatisticsTimePeriod.Week || p.Match.KickOff > DateTime.UtcNow.AddDays(-7))
                .Where(p => filters.TimePeriod != StatisticsTimePeriod.Month || p.Match.KickOff > DateTime.UtcNow.AddMonths(-1))
                .Where(p => filters.TimePeriod != StatisticsTimePeriod.Year || p.Match.KickOff > DateTime.UtcNow.AddYears(-1))
                .Include(p => p.Match)
                    .ThenInclude(p => p.TeamHome)
                    .ThenInclude(t => t.BadgeImage)
                .Include(p => p.Match)
                    .ThenInclude(p => p.TeamAway)
                    .ThenInclude(t => t.BadgeImage)
                .Include(p => p.Team)
                    .ThenInclude(p => p.BadgeImage)
                .Include(p => p.Position)
                .Where(p => filters.PlayerId == null || p.PlayerId == filters.PlayerId);
        }

        private IQueryable<PlayerMatchStatistics> GetPlayerMatchStatisticsQueryable(PlayerStatisticFilters filters)
        {
            return _coachBotContext.PlayerMatchStatistics
                .AsNoTracking()
                .Where(p => filters.MatchId == null || p.MatchId == filters.MatchId)
                .Where(p => filters.IncludeSubstituteAppearances || !p.Substitute)
                .Where(p => filters.PlayerId == null || p.PlayerId == filters.PlayerId)
                .Where(p => filters.TeamId == null || p.TeamId == filters.TeamId)
                .Where(p => filters.MatchTeamType == null || p.MatchTeamType == filters.MatchTeamType)
                .Where(p => filters.RegionId == null || p.Team.RegionId == filters.RegionId)
                .Where(p => filters.TournamentId == null || p.Match.TournamentId == filters.TournamentId)
                .Where(p => filters.MatchOutcome == null || p.MatchOutcome == filters.MatchOutcome)
                .Where(p => filters.MatchFormat == null || p.Match.Format == filters.MatchFormat)
                .Where(p => string.IsNullOrWhiteSpace(filters.PlayerName) || p.Player.Name.Contains(filters.PlayerName))
                .Where(p => filters.TimePeriod != StatisticsTimePeriod.Week || p.Match.KickOff > DateTime.UtcNow.AddDays(-7))
                .Where(p => filters.TimePeriod != StatisticsTimePeriod.Month || p.Match.KickOff > DateTime.UtcNow.AddMonths(-1))
                .Where(p => filters.TimePeriod != StatisticsTimePeriod.Year || p.Match.KickOff > DateTime.UtcNow.AddYears(-1))
                .Include(p => p.Match)
                    .ThenInclude(p => p.TeamHome)
                .Include(p => p.Match)
                    .ThenInclude(p => p.TeamAway)
                .Include(p => p.Team)
                .Where(p => filters.PlayerId == null || p.PlayerId == filters.PlayerId);
        }

        private IQueryable<TeamMatchStatistics> GetTeamMatchStatisticsQueryable(TeamStatisticsFilters filters)
        {
            var results = _coachBotContext.TeamMatchStatistics
                 .AsNoTracking()
                 .Where(t => filters.TournamentId == null || t.Match.TournamentId == filters.TournamentId)
                 .Where(t => filters.TeamId == null || filters.HeadToHead || t.TeamId == filters.TeamId)
                 .Where(t => filters.OppositionTeamId == null || filters.HeadToHead || t.Match.TeamAwayId == filters.OppositionTeamId || t.Match.TeamHomeId == filters.OppositionTeamId)
                 .Where(p => filters.OppositionTeamId == null || !filters.HeadToHead || (p.Match.TeamHomeId == filters.TeamId && p.Match.TeamAwayId == filters.OppositionTeamId) || (p.Match.TeamAwayId == filters.TeamId && p.Match.TeamHomeId == filters.OppositionTeamId))
                 .Where(p => filters.MatchTeamType == null || p.MatchTeamType == filters.MatchTeamType)
                 .Where(t => filters.RegionId == null || t.Team.RegionId == filters.RegionId)
                 .Where(t => filters.IncludeInactive || t.Team.Inactive == false)
                 .Where(t => filters.TeamType == null || t.Team.TeamType == filters.TeamType)
                 .Where(t => filters.MatchType == null | t.Match.MatchType == filters.MatchType)
                 .Where(p => filters.MatchOutcome == null || p.MatchOutcome == filters.MatchOutcome)
                 .Where(p => filters.MatchFormat == null || p.Match.Format == filters.MatchFormat)
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Week || p.Match.KickOff > DateTime.UtcNow.AddDays(-7))
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Month || p.Match.KickOff > DateTime.UtcNow.AddMonths(-1))
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Year || p.Match.KickOff > DateTime.UtcNow.AddYears(-1))
                 .Include(p => p.Match)
                    .ThenInclude(p => p.TeamHome)
                .Include(p => p.Match)
                    .ThenInclude(p => p.TeamAway);

            if (filters.HeadToHead)
            {
                return results
                    .Include(p => p.Match).ThenInclude(m => m.MatchStatistics)
                    .Include(p => p.Match).ThenInclude(m => m.Tournament);
            }

            return results;
        }

        private IQueryable<PlayerStatisticTotals> GetPlayerStatisticTotals(PlayerStatisticFilters filters)
        {
            return _coachBotContext
                 .PlayerPositionMatchStatistics
                 .AsNoTracking()
                 .Where(p => filters.IncludeSubstituteAppearances || !p.Substitute)
                 .Where(p => filters.MatchId == null || p.MatchId == filters.MatchId)
                 .Where(p => filters.PlayerId == null || p.PlayerId == filters.PlayerId)
                 .Where(p => filters.TeamId == null || filters.OppositionTeamId != null || p.TeamId == filters.TeamId && p.Player.Teams.Any(pt => pt.TeamId == filters.TeamId))
                 .Where(p => filters.OppositionTeamId == null || (p.Match.TeamHomeId == filters.TeamId && p.Match.TeamAwayId == filters.OppositionTeamId) || (p.Match.TeamAwayId == filters.TeamId && p.Match.TeamHomeId == filters.OppositionTeamId))
                 .Where(p => filters.PositionId == null || p.PositionId == filters.PositionId)
                 .Where(p => string.IsNullOrWhiteSpace(filters.PositionName) || p.Position.Name == filters.PositionName)
                 .Where(p => filters.MinimumSecondsPlayed == null || p.SecondsPlayed > filters.MinimumSecondsPlayed)
                 .Where(p => filters.RegionId == null || p.Team.RegionId == filters.RegionId)
                 .Where(p => filters.TournamentId == null || p.Match.TournamentId == filters.TournamentId)
                 .Where(p => filters.MatchFormat == null || p.Match.Format == filters.MatchFormat)
                 .Where(p => filters.MatchType == null || p.Match.MatchType == filters.MatchType)
                 .Where(p => string.IsNullOrWhiteSpace(filters.PlayerName) || p.Player.Name.Contains(filters.PlayerName))
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Week || p.Match.KickOff > DateTime.UtcNow.AddDays(-7))
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Month || p.Match.KickOff > DateTime.UtcNow.AddMonths(-1))
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Year || p.Match.KickOff > DateTime.UtcNow.AddYears(-1))
                 .Select(m => new
                 {
                     m.PlayerId,
                     m.Player.DiscordUserId,
                     m.Player.SteamID,
                     m.Player.Name,
                     m.RedCards,
                     m.YellowCards,
                     m.Fouls,
                     m.FoulsSuffered,
                     m.SlidingTackles,
                     m.SlidingTacklesCompleted,
                     m.GoalsConceded,
                     m.Shots,
                     m.ShotsOnGoal,
                     m.Passes,
                     m.PassesCompleted,
                     m.Interceptions,
                     m.Offsides,
                     m.GoalKicks,
                     m.OwnGoals,
                     m.DistanceCovered,
                     m.FreeKicks,
                     m.KeeperSaves,
                     m.KeeperSavesCaught,
                     m.Penalties,
                     m.Possession,
                     m.PossessionPercentage,
                     m.ThrowIns,
                     m.Corners,
                     m.Goals,
                     m.Assists,
                     m.MatchOutcome,
                     m.Substitute,
                     m.MatchId,
                     m.Player.Rating,
                     m.PositionId
                 })
                 .GroupBy(p => new { p.PlayerId, p.SteamID, p.Name, p.Rating }, (key, s) => new PlayerStatisticTotals()
                 {
                     Goals = s.Sum(p => p.Goals),
                     GoalsAverage = s.Average(p => p.Goals),
                     Assists = s.Sum(p => p.Assists),
                     AssistsAverage = s.Average(p => p.Assists),
                     Fouls = s.Sum(p => p.Fouls),
                     FoulsAverage = s.Average(p => p.Fouls),
                     FoulsSuffered = s.Sum(p => p.FoulsSuffered),
                     FoulsSufferedAverage = s.Average(p => p.FoulsSuffered),
                     SlidingTacklesAverage = s.Average(p => p.SlidingTackles),
                     SlidingTacklesCompletedAverage = s.Average(p => p.SlidingTacklesCompleted),
                     GoalsConceded = s.Sum(p => p.GoalsConceded),
                     GoalsConcededAverage = s.Average(p => p.GoalsConceded),
                     Shots = s.Sum(p => p.Shots),
                     ShotsAverage = s.Average(p => p.Shots),
                     ShotsOnGoal = s.Sum(p => p.ShotsOnGoal),
                     ShotsOnGoalAverage = s.Average(p => p.ShotsOnGoal),
                     ShotAccuracyPercentage = s.Sum(p => Convert.ToDouble(p.Shots)) > 0 ? s.Sum(p => Convert.ToDouble(p.ShotsOnGoal)) / s.Sum(p => Convert.ToDouble(p.Shots)) : 0,
                     Passes = s.Sum(p => p.Passes),
                     PassesAverage = s.Average(p => p.Passes),
                     PassesCompleted = s.Sum(p => p.PassesCompleted),
                     PassesCompletedAverage = s.Average(p => p.PassesCompleted),
                     PassCompletionPercentageAverage = s.Sum(p => Convert.ToDouble(p.Passes)) > 0 ? s.Sum(p => Convert.ToDouble(p.PassesCompleted)) / s.Sum(p => Convert.ToDouble(p.Passes)) : 0,
                     Interceptions = s.Sum(p => p.Interceptions),
                     InterceptionsAverage = s.Average(p => p.Interceptions),
                     Offsides = s.Sum(p => p.Offsides),
                     OffsidesAverage = s.Average(p => p.Offsides),
                     GoalKicksAverage = s.Average(p => p.GoalKicks),
                     OwnGoals = s.Sum(p => p.OwnGoals),
                     OwnGoalsAverage = s.Average(p => p.OwnGoals),
                     DistanceCoveredAverage = s.Average(p => p.DistanceCovered),
                     FreeKicks = s.Sum(p => p.FreeKicks),
                     FreeKicksAverage = s.Average(p => p.FreeKicks),
                     KeeperSaves = s.Sum(p => p.KeeperSaves),
                     KeeperSavesAverage = s.Average(p => p.KeeperSaves),
                     KeeperSavesCaughtAverage = s.Average(p => p.KeeperSavesCaught),
                     Penalties = s.Sum(p => p.Penalties),
                     PenaltiesAverage = s.Average(p => p.Penalties),
                     PossessionAverage = s.Average(p => p.Possession),
                     PossessionPercentageAverage = s.Average(p => p.PossessionPercentage),
                     RedCards = s.Sum(p => p.RedCards),
                     RedCardsAverage = s.Average(p => p.RedCards),
                     YellowCards = s.Sum(p => p.YellowCards),
                     YellowCardsAverage = s.Average(p => p.YellowCards),
                     ThrowIns = s.Sum(p => p.ThrowIns),
                     Corners = s.Sum(p => p.Corners),
                     PlayerId = key.PlayerId,
                     Appearances = s.Count(),
                     KeeperSavePercentage = s.Sum(p => Convert.ToDouble(p.KeeperSaves)) > 0 ? s.Sum(p => Convert.ToDouble(p.KeeperSaves)) / (s.Sum(p => Convert.ToDouble(p.KeeperSaves)) + s.Sum(p => Convert.ToDouble(p.GoalsConceded))) : 0,
                     ShotConversionPercentage = s.Sum(p => Convert.ToDouble(p.Shots)) > 0 ? s.Sum(p => Convert.ToDouble(p.Goals)) / s.Sum(p => Convert.ToDouble(p.Shots)) : 0,
                     SubstituteAppearances = s.Sum(p => p.Substitute ? 1 : 0),
                     Wins = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Win ? 1 : 0),
                     WinPercentage = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Win ? 1 : 0) > 0 ? Convert.ToDouble(s.Sum(p => p.MatchOutcome == MatchOutcomeType.Win ? 1 : 0)) / Convert.ToDouble(s.Count()) : 0,
                     Losses = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Loss ? 1 : 0),
                     LossPercentage = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Loss ? 1 : 0) > 0 ? Convert.ToDouble(s.Sum(p => p.MatchOutcome == MatchOutcomeType.Loss ? 1 : 0)) / Convert.ToDouble(s.Count()) : 0,
                     Draws = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Draw ? 1 : 0),
                     DrawPercentage = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Draw ? 1 : 0) > 0 ? Convert.ToDouble(s.Sum(p => p.MatchOutcome == MatchOutcomeType.Draw ? 1 : 0)) / Convert.ToDouble(s.Count()) : 0,
                     Name = key.Name,
                     SteamID = key.SteamID,
                     Rating = key.Rating
                 })
                 .Where(p => filters.MinimumAppearances == null || p.Appearances >= filters.MinimumAppearances);
        }

        private IQueryable<PlayerStatisticTotals> GetPlayerMatchStatisticTotals(PlayerStatisticFilters filters)
        {
            return _coachBotContext
                 .PlayerMatchStatistics
                 .AsNoTracking()
                 .Where(p => filters.IncludeSubstituteAppearances || !p.Substitute)
                 .Where(p => filters.MatchId == null || p.MatchId == filters.MatchId)
                 .Where(p => filters.PlayerId == null || p.PlayerId == filters.PlayerId)
                 .Where(p => filters.TeamId == null || filters.OppositionTeamId != null || p.TeamId == filters.TeamId && p.Player.Teams.Any(pt => pt.TeamId == filters.TeamId))
                 .Where(p => filters.OppositionTeamId == null || (p.Match.TeamHomeId == filters.TeamId && p.Match.TeamAwayId == filters.OppositionTeamId) || (p.Match.TeamAwayId == filters.TeamId && p.Match.TeamHomeId == filters.OppositionTeamId))
                 .Where(p => filters.MinimumSecondsPlayed == null || p.SecondsPlayed > filters.MinimumSecondsPlayed)
                 .Where(p => filters.RegionId == null || p.Team.RegionId == filters.RegionId)
                 .Where(p => filters.TournamentId == null || p.Match.TournamentId == filters.TournamentId)
                 .Where(p => filters.MatchFormat == null || p.Match.Format == filters.MatchFormat)
                 .Where(p => filters.MatchType == null || p.Match.MatchType == filters.MatchType)
                 .Where(p => string.IsNullOrWhiteSpace(filters.PlayerName) || p.Player.Name.Contains(filters.PlayerName))
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Week || p.Match.KickOff > DateTime.UtcNow.AddDays(-7))
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Month || p.Match.KickOff > DateTime.UtcNow.AddMonths(-1))
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Year || p.Match.KickOff > DateTime.UtcNow.AddYears(-1))
                 .Select(m => new
                 {
                     m.PlayerId,
                     m.Player.DiscordUserId,
                     m.Player.SteamID,
                     m.Player.Name,
                     m.RedCards,
                     m.YellowCards,
                     m.Fouls,
                     m.FoulsSuffered,
                     m.SlidingTackles,
                     m.SlidingTacklesCompleted,
                     m.GoalsConceded,
                     m.Shots,
                     m.ShotsOnGoal,
                     m.Passes,
                     m.PassesCompleted,
                     m.Interceptions,
                     m.Offsides,
                     m.GoalKicks,
                     m.OwnGoals,
                     m.DistanceCovered,
                     m.FreeKicks,
                     m.KeeperSaves,
                     m.KeeperSavesCaught,
                     m.Penalties,
                     m.Possession,
                     m.PossessionPercentage,
                     m.ThrowIns,
                     m.Corners,
                     m.Goals,
                     m.Assists,
                     m.MatchOutcome,
                     m.Substitute,
                     m.MatchId,
                     m.Player.Rating
                 })
                 .GroupBy(p => new { p.PlayerId, p.SteamID, p.Name, p.Rating }, (key, s) => new PlayerStatisticTotals()
                 {
                     Goals = s.Sum(p => p.Goals),
                     GoalsAverage = s.Average(p => p.Goals),
                     Assists = s.Sum(p => p.Assists),
                     AssistsAverage = s.Average(p => p.Assists),
                     Fouls = s.Sum(p => p.Fouls),
                     FoulsAverage = s.Average(p => p.Fouls),
                     FoulsSuffered = s.Sum(p => p.FoulsSuffered),
                     FoulsSufferedAverage = s.Average(p => p.FoulsSuffered),
                     SlidingTacklesAverage = s.Average(p => p.SlidingTackles),
                     SlidingTacklesCompletedAverage = s.Average(p => p.SlidingTacklesCompleted),
                     GoalsConceded = s.Sum(p => p.GoalsConceded),
                     GoalsConcededAverage = s.Average(p => p.GoalsConceded),
                     Shots = s.Sum(p => p.Shots),
                     ShotsAverage = s.Average(p => p.Shots),
                     ShotsOnGoal = s.Sum(p => p.ShotsOnGoal),
                     ShotsOnGoalAverage = s.Average(p => p.ShotsOnGoal),
                     ShotAccuracyPercentage = s.Sum(p => Convert.ToDouble(p.Shots)) > 0 ? s.Sum(p => Convert.ToDouble(p.ShotsOnGoal)) / s.Sum(p => Convert.ToDouble(p.Shots)) : 0,
                     Passes = s.Sum(p => p.Passes),
                     PassesAverage = s.Average(p => p.Passes),
                     PassesCompleted = s.Sum(p => p.PassesCompleted),
                     PassesCompletedAverage = s.Average(p => p.PassesCompleted),
                     PassCompletionPercentageAverage = s.Sum(p => Convert.ToDouble(p.Passes)) > 0 ? s.Sum(p => Convert.ToDouble(p.PassesCompleted)) / s.Sum(p => Convert.ToDouble(p.Passes)) : 0,
                     Interceptions = s.Sum(p => p.Interceptions),
                     InterceptionsAverage = s.Average(p => p.Interceptions),
                     Offsides = s.Sum(p => p.Offsides),
                     OffsidesAverage = s.Average(p => p.Offsides),
                     GoalKicksAverage = s.Average(p => p.GoalKicks),
                     OwnGoals = s.Sum(p => p.OwnGoals),
                     OwnGoalsAverage = s.Average(p => p.OwnGoals),
                     DistanceCoveredAverage = s.Average(p => p.DistanceCovered),
                     FreeKicks = s.Sum(p => p.FreeKicks),
                     FreeKicksAverage = s.Average(p => p.FreeKicks),
                     KeeperSaves = s.Sum(p => p.KeeperSaves),
                     KeeperSavesAverage = s.Average(p => p.KeeperSaves),
                     KeeperSavesCaughtAverage = s.Average(p => p.KeeperSavesCaught),
                     Penalties = s.Sum(p => p.Penalties),
                     PenaltiesAverage = s.Average(p => p.Penalties),
                     PossessionAverage = s.Average(p => p.Possession),
                     PossessionPercentageAverage = s.Average(p => p.PossessionPercentage),
                     RedCards = s.Sum(p => p.RedCards),
                     RedCardsAverage = s.Average(p => p.RedCards),
                     YellowCards = s.Sum(p => p.YellowCards),
                     YellowCardsAverage = s.Average(p => p.YellowCards),
                     ThrowIns = s.Sum(p => p.ThrowIns),
                     Corners = s.Sum(p => p.Corners),
                     PlayerId = key.PlayerId,
                     Appearances = s.Count(),
                     KeeperSavePercentage = s.Sum(p => Convert.ToDouble(p.KeeperSaves)) > 0 ? s.Sum(p => Convert.ToDouble(p.KeeperSaves)) / (s.Sum(p => Convert.ToDouble(p.KeeperSaves)) + s.Sum(p => Convert.ToDouble(p.GoalsConceded))) : 0,
                     ShotConversionPercentage = s.Sum(p => Convert.ToDouble(p.Shots)) > 0 ? s.Sum(p => Convert.ToDouble(p.Goals)) / s.Sum(p => Convert.ToDouble(p.Shots)) : 0,
                     SubstituteAppearances = s.Sum(p => p.Substitute ? 1 : 0),
                     Wins = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Win ? 1 : 0),
                     WinPercentage = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Win ? 1 : 0) > 0 ? Convert.ToDouble(s.Sum(p => p.MatchOutcome == MatchOutcomeType.Win ? 1 : 0)) / Convert.ToDouble(s.Count()) : 0,
                     Losses = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Loss ? 1 : 0),
                     LossPercentage = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Loss ? 1 : 0) > 0 ? Convert.ToDouble(s.Sum(p => p.MatchOutcome == MatchOutcomeType.Loss ? 1 : 0)) / Convert.ToDouble(s.Count()) : 0,
                     Draws = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Draw ? 1 : 0),
                     DrawPercentage = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Draw ? 1 : 0) > 0 ? Convert.ToDouble(s.Sum(p => p.MatchOutcome == MatchOutcomeType.Draw ? 1 : 0)) / Convert.ToDouble(s.Count()) : 0,
                     Name = key.Name,
                     SteamID = key.SteamID,
                     Rating = key.Rating
                 })
                 .Where(p => filters.MinimumAppearances == null || p.Appearances >= filters.MinimumAppearances);
        }

        private IQueryable<TeamStatisticTotals> GetTeamStatisticTotals(TeamStatisticsFilters filters)
        {
            return _coachBotContext
                 .TeamMatchStatistics
                 .AsQueryable()
                 .Where(t => filters.TournamentId == null || t.Match.TournamentId == filters.TournamentId)
                 .Where(t => filters.TournamentGroupId == null || _coachBotContext.TournamentGroupMatches.Any(tg => tg.MatchId == t.MatchId && tg.TournamentGroupId == filters.TournamentGroupId))
                 .Where(t => filters.TeamId == null || filters.HeadToHead || t.TeamId == filters.TeamId)
                 .Where(t => filters.OppositionTeamId == null || filters.HeadToHead || t.Match.TeamAwayId == filters.OppositionTeamId || t.Match.TeamHomeId == filters.OppositionTeamId)
                 .Where(p => filters.OppositionTeamId == null || !filters.HeadToHead || (p.Match.TeamHomeId == filters.TeamId && p.Match.TeamAwayId == filters.OppositionTeamId) || (p.Match.TeamAwayId == filters.TeamId && p.Match.TeamHomeId == filters.OppositionTeamId))
                 .Where(t => filters.RegionId == null || t.Team.RegionId == filters.RegionId)
                 .Where(t => filters.IncludeInactive || t.Team.Inactive == false)
                 .Where(t => filters.TeamType == null || t.Team.TeamType == filters.TeamType)
                 .Where(p => filters.MatchFormat == null || p.Match.Format == filters.MatchFormat)
                 .Where(t => filters.MatchType == null || t.Match.MatchType == filters.MatchType)
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Week || p.Match.KickOff > DateTime.UtcNow.AddDays(-7))
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Month || p.Match.KickOff > DateTime.UtcNow.AddMonths(-1))
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Year || p.Match.KickOff > DateTime.UtcNow.AddYears(-1))
                 .Where(p => filters.DateFrom == null || p.Match.KickOff > filters.DateFrom)
                 .Where(p => filters.DateTo == null || p.Match.KickOff < filters.DateTo)
                 .AsNoTracking()
                 .Select(m => new
                 {
                     m.TeamId,
                     m.RedCards,
                     m.YellowCards,
                     m.Fouls,
                     m.FoulsSuffered,
                     m.SlidingTackles,
                     m.SlidingTacklesCompleted,
                     m.GoalsConceded,
                     m.Shots,
                     m.ShotsOnGoal,
                     m.Passes,
                     m.PassesCompleted,
                     m.Interceptions,
                     m.Offsides,
                     m.GoalKicks,
                     m.OwnGoals,
                     m.DistanceCovered,
                     m.FreeKicks,
                     m.KeeperSaves,
                     m.KeeperSavesCaught,
                     m.Penalties,
                     m.Possession,
                     m.PossessionPercentage,
                     m.ThrowIns,
                     m.Corners,
                     m.Goals,
                     m.Assists,
                     m.MatchOutcome,
                     m.Team.BadgeImage.Url,
                     m.Team.Form,
                     m.Team.Name
                 })
                 .GroupBy(p => new { p.TeamId, p.Name, p.Url, p.Form }, (key, s) => new TeamStatisticTotals()
                 {
                     Goals = s.Sum(p => p.Goals),
                     GoalsAverage = s.Average(p => p.Goals),
                     Assists = s.Sum(p => p.Assists),
                     AssistsAverage = s.Average(p => p.Assists),
                     Fouls = s.Sum(p => p.Fouls),
                     FoulsAverage = s.Average(p => p.Fouls),
                     FoulsSuffered = s.Sum(p => p.FoulsSuffered),
                     FoulsSufferedAverage = s.Average(p => p.FoulsSuffered),
                     SlidingTacklesAverage = s.Average(p => p.SlidingTackles),
                     SlidingTacklesCompletedAverage = s.Average(p => p.SlidingTacklesCompleted),
                     GoalsConceded = s.Sum(p => p.GoalsConceded),
                     GoalsConcededAverage = s.Average(p => p.GoalsConceded),
                     Shots = s.Sum(p => p.Shots),
                     ShotsAverage = s.Average(p => p.Shots),
                     ShotsOnGoal = s.Sum(p => p.ShotsOnGoal),
                     ShotsOnGoalAverage = s.Average(p => p.ShotsOnGoal),
                     ShotAccuracyPercentage = s.Sum(p => Convert.ToDouble(p.Shots)) > 0 ? s.Sum(p => Convert.ToDouble(p.ShotsOnGoal)) / s.Sum(p => Convert.ToDouble(p.Shots)) : 0,
                     ShotConversionPercentage = s.Sum(p => Convert.ToDouble(p.Shots)) > 0 ? s.Sum(p => Convert.ToDouble(p.Goals)) / s.Sum(p => Convert.ToDouble(p.Shots)) : 0,
                     Passes = s.Sum(p => p.Passes),
                     PassesAverage = s.Average(p => p.Passes),
                     PassesCompleted = s.Sum(p => p.PassesCompleted),
                     PassesCompletedAverage = s.Average(p => p.PassesCompleted),
                     PassCompletionPercentageAverage = s.Sum(p => Convert.ToDouble(p.Passes)) > 0 ? s.Sum(p => Convert.ToDouble(p.PassesCompleted)) / s.Sum(p => Convert.ToDouble(p.Passes)) : 0,
                     Interceptions = s.Sum(p => p.Interceptions),
                     InterceptionsAverage = s.Average(p => p.Interceptions),
                     Offsides = s.Sum(p => p.Offsides),
                     OffsidesAverage = s.Average(p => p.Offsides),
                     GoalKicksAverage = s.Average(p => p.GoalKicks),
                     OwnGoals = s.Sum(p => p.OwnGoals),
                     OwnGoalsAverage = s.Average(p => p.OwnGoals),
                     DistanceCoveredAverage = s.Average(p => p.DistanceCovered),
                     FreeKicks = s.Sum(p => p.FreeKicks),
                     FreeKicksAverage = s.Average(p => p.FreeKicks),
                     KeeperSaves = s.Sum(p => p.KeeperSaves),
                     KeeperSavesAverage = s.Average(p => p.KeeperSaves),
                     KeeperSavesCaughtAverage = s.Average(p => p.KeeperSavesCaught),
                     Penalties = s.Sum(p => p.Penalties),
                     PenaltiesAverage = s.Average(p => p.Penalties),
                     PossessionAverage = s.Average(p => p.Possession),
                     PossessionPercentageAverage = s.Average(p => p.PossessionPercentage),
                     RedCards = s.Sum(p => p.RedCards),
                     RedCardsAverage = s.Average(p => p.RedCards),
                     YellowCards = s.Sum(p => p.YellowCards),
                     YellowCardsAverage = s.Average(p => p.YellowCards),
                     ThrowIns = s.Sum(p => p.ThrowIns),
                     Corners = s.Sum(p => p.Corners),
                     Appearances = s.Count(),
                     Wins = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Win ? 1 : 0),
                     WinPercentage = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Win ? 1 : 0) > 0 ? Convert.ToDouble(s.Sum(p => p.MatchOutcome == MatchOutcomeType.Win ? 1 : 0)) / Convert.ToDouble(s.Count()) : 0,
                     Losses = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Loss ? 1 : 0),
                     LossPercentage = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Loss ? 1 : 0) > 0 ? Convert.ToDouble(s.Sum(p => p.MatchOutcome == MatchOutcomeType.Loss ? 1 : 0)) / Convert.ToDouble(s.Count()) : 0,
                     Draws = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Draw ? 1 : 0),
                     DrawPercentage = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Draw ? 1 : 0) > 0 ? Convert.ToDouble(s.Sum(p => p.MatchOutcome == MatchOutcomeType.Draw ? 1 : 0)) / Convert.ToDouble(s.Count()) : 0,
                     TeamId = key.TeamId,
                     TeamName = key.Name,
                     BadgeImageUrl = key.Url,
                     Form = key.Form,
                     GoalDifference = s.Sum(p => p.Goals) - s.Sum(p => p.GoalsConceded),
                     Points = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Win ? 3 : p.MatchOutcome == MatchOutcomeType.Draw ? 1 : 0)
                 })
                 .Where(p => filters.MinimumMatches == null || p.Appearances >= filters.MinimumMatches);
        }

        public List<PlayerPerformanceSnapshot> GetMonthlyPlayerPerformance(int playerId)
        {
            return _coachBotContext
                 .PlayerPerformanceSnapshots
                 .FromSqlInterpolated($@"SELECT PlayerMatchStatistics.PlayerId,
                                    NULL AS Day,
                                    NULL AS Week,
                                    DATEPART(month, PlayerMatchStatistics.CreatedDate) AS Month,
                                    DATEPART(year, PlayerMatchStatistics.CreatedDate) AS Year,
                                    ROUND(AVG(CAST(PlayerMatchStatistics.Goals AS FLOAT)), 2) AS AverageGoals,
                                    ROUND(AVG(CAST(PlayerMatchStatistics.Assists AS FLOAT)), 2) AS AverageAssists,
                                    ROUND(AVG(CAST(PlayerMatchStatistics.GoalsConceded AS FLOAT)), 2) AS AverageGoalsConceded,
                                    COUNT(CASE WHEN GoalsConceded = 0 THEN 1 ELSE 0 END) As CleanSheets,
                                    SUM(CASE WHEN PlayerMatchStatistics.MatchOutcome = {MatchOutcomeType.Win} THEN 1 ELSE 0 END) As Wins,
                                    SUM(CASE WHEN PlayerMatchStatistics.MatchOutcome = {MatchOutcomeType.Draw} THEN 1 ELSE 0 END) As Draws,
                                    SUM(CASE WHEN PlayerMatchStatistics.MatchOutcome = {MatchOutcomeType.Loss} THEN 1 ELSE 0 END) As Losses,
                                    COUNT(*) As Appearances
                    FROM dbo.PlayerMatchStatistics PlayerMatchStatistics
                    WHERE PlayerMatchStatistics.PlayerId = {playerId} AND PlayerMatchStatistics.CreatedDate > DATEADD(year, -1, GETDATE())
                    GROUP BY PlayerMatchStatistics.PlayerId,
                            DATEPART(month, PlayerMatchStatistics.CreatedDate),
                            DATEPART(year, PlayerMatchStatistics.CreatedDate)
                    ORDER BY DATEPART(year, PlayerMatchStatistics.CreatedDate), DATEPART(month, PlayerMatchStatistics.CreatedDate)")
                .ToList();
        }

        public List<TeamPerformanceSnapshot> GetMonthlyTeamPerformance(int teamId)
        {
            return _coachBotContext
                 .TeamPerformanceSnapshots
                 .FromSqlInterpolated($@"SELECT {teamId} AS TeamId,
                            NULL AS Day,
                            NULL AS Week,
                            DATEPART(month, TeamMatchStatistics.CreatedDate) AS Month,
                            DATEPART(year, TeamMatchStatistics.CreatedDate) AS Year,
                            ISNULL(ROUND(AVG(CAST(TeamMatchStatistics.Goals AS FLOAT)), 2), 0) AS AverageGoals,
                            ISNULL(ROUND(AVG(CAST(TeamMatchStatistics.Assists AS FLOAT)), 2), 0) AS AverageAssists,
                            ISNULL(ROUND(AVG(CAST(TeamMatchStatistics.GoalsConceded AS FLOAT)), 2), 0) AS AverageGoalsConceded,
                            SUM(CASE WHEN TeamMatchStatistics.GoalsConceded = 0 THEN 1 ELSE 0 END) As CleanSheets,
                            SUM(CASE WHEN TeamMatchStatistics.MatchOutcome = {MatchOutcomeType.Win} THEN 1 ELSE 0 END) As Wins,
                            SUM(CASE WHEN TeamMatchStatistics.MatchOutcome = {MatchOutcomeType.Draw} THEN 1 ELSE 0 END) As Draws,
                            SUM(CASE WHEN TeamMatchStatistics.MatchOutcome = {MatchOutcomeType.Loss} THEN 1 ELSE 0 END) As Losses,
                            SUM(CASE WHEN CreatedDate IS NOT NULL THEN 1 ELSE 0 END) As Matches
                        FROM dbo.TeamMatchStatistics TeamMatchStatistics
                        WHERE TeamMatchStatistics.TeamId = {teamId} AND TeamMatchStatistics.CreatedDate > DATEADD(year, -1, GETDATE())
                        GROUP BY TeamMatchStatistics.TeamId,
                                DATEPART(week, TeamMatchStatistics.CreatedDate),
                                DATEPART(month, TeamMatchStatistics.CreatedDate),
                                DATEPART(year, TeamMatchStatistics.CreatedDate)
                        ORDER BY DATEPART(year, TeamMatchStatistics.CreatedDate), DATEPART(month, TeamMatchStatistics.CreatedDate)")
                .ToList();
        }

        public List<PlayerPerformanceSnapshot> GetWeeklyPlayerPerformance(int playerId)
        {
            return _coachBotContext
                 .PlayerPerformanceSnapshots
                 .FromSqlInterpolated($@"SELECT PlayerMatchStatistics.PlayerId,
                                    NULL AS Day,
                                    DATEPART(week, PlayerMatchStatistics.CreatedDate) AS Week,
                                    DATEPART(month, PlayerMatchStatistics.CreatedDate) AS Month,
                                    DATEPART(year, PlayerMatchStatistics.CreatedDate) AS Year,
                                    ROUND(AVG(CAST(PlayerMatchStatistics.Goals AS FLOAT)), 2) AS AverageGoals,
                                    ROUND(AVG(CAST(PlayerMatchStatistics.Assists AS FLOAT)), 2) AS AverageAssists,
                                    ROUND(AVG(CAST(PlayerMatchStatistics.GoalsConceded AS FLOAT)), 2) AS AverageGoalsConceded,
                                    COUNT(CASE WHEN GoalsConceded = 0 THEN 1 ELSE 0 END) As CleanSheets,
                                    SUM(CASE WHEN PlayerMatchStatistics.MatchOutcome = {MatchOutcomeType.Win} THEN 1 ELSE 0 END) As Wins,
                                    SUM(CASE WHEN PlayerMatchStatistics.MatchOutcome = {MatchOutcomeType.Draw} THEN 1 ELSE 0 END) As Draws,
                                    SUM(CASE WHEN PlayerMatchStatistics.MatchOutcome = {MatchOutcomeType.Loss} THEN 1 ELSE 0 END) As Losses,
                                    COUNT(*) As Appearances
                    FROM dbo.PlayerMatchStatistics PlayerMatchStatistics
                    WHERE PlayerMatchStatistics.PlayerId = {playerId} AND PlayerMatchStatistics.CreatedDate > DATEADD(month, -12, GETDATE())
                    GROUP BY PlayerMatchStatistics.PlayerId,
                            DATEPART(week, PlayerMatchStatistics.CreatedDate),
                            DATEPART(month, PlayerMatchStatistics.CreatedDate),
                            DATEPART(year, PlayerMatchStatistics.CreatedDate)
                    ORDER BY DATEPART(year, PlayerMatchStatistics.CreatedDate), DATEPART(month, PlayerMatchStatistics.CreatedDate), DATEPART(week, PlayerMatchStatistics.CreatedDate)")
                .ToList();
        }

        public List<TeamPerformanceSnapshot> GetWeeklyTeamPerformance(int teamId)
        {
            return _coachBotContext
                 .TeamPerformanceSnapshots
                 .FromSqlInterpolated($@"SELECT {teamId} AS TeamId,
                            NULL AS Day,
                            DATEPART(week, TeamMatchStatistics.CreatedDate) AS Week,
                            DATEPART(month, TeamMatchStatistics.CreatedDate) AS Month,
                            DATEPART(year, TeamMatchStatistics.CreatedDate) AS Year,
                            ISNULL(ROUND(AVG(CAST(TeamMatchStatistics.Goals AS FLOAT)), 2), 0) AS AverageGoals,
                            ISNULL(ROUND(AVG(CAST(TeamMatchStatistics.Assists AS FLOAT)), 2), 0) AS AverageAssists,
                            ISNULL(ROUND(AVG(CAST(TeamMatchStatistics.GoalsConceded AS FLOAT)), 2), 0) AS AverageGoalsConceded,
                            SUM(CASE WHEN TeamMatchStatistics.GoalsConceded = 0 THEN 1 ELSE 0 END) As CleanSheets,
                            SUM(CASE WHEN TeamMatchStatistics.MatchOutcome = {MatchOutcomeType.Win} THEN 1 ELSE 0 END) As Wins,
                            SUM(CASE WHEN TeamMatchStatistics.MatchOutcome = {MatchOutcomeType.Draw} THEN 1 ELSE 0 END) As Draws,
                            SUM(CASE WHEN TeamMatchStatistics.MatchOutcome = {MatchOutcomeType.Loss} THEN 1 ELSE 0 END) As Losses,
                            SUM(CASE WHEN CreatedDate IS NOT NULL THEN 1 ELSE 0 END) As Matches
                        FROM dbo.TeamMatchStatistics TeamMatchStatistics
                        WHERE TeamMatchStatistics.TeamId = {teamId} AND TeamMatchStatistics.CreatedDate > DATEADD(month, -12, GETDATE())
                        GROUP BY TeamMatchStatistics.TeamId,
                                DATEPART(week, TeamMatchStatistics.CreatedDate),
                                DATEPART(month, TeamMatchStatistics.CreatedDate),
                                DATEPART(year, TeamMatchStatistics.CreatedDate)
                        ORDER BY DATEPART(year, TeamMatchStatistics.CreatedDate), DATEPART(month, TeamMatchStatistics.CreatedDate), DATEPART(week, TeamMatchStatistics.CreatedDate)")
                .ToList();
        }

        public List<PlayerPerformanceSnapshot> GetDailyPlayerPerformance(int playerId)
        {
            return _coachBotContext
                 .PlayerPerformanceSnapshots
                 .FromSqlInterpolated($@"SELECT {playerId} AS PlayerId,
                            DATEPART(day, DateRange.DateValue) AS Day,
                            DATEPART(week, DateRange.DateValue) AS Week,
                            DATEPART(month, DateRange.DateValue) AS Month,
                            DATEPART(year, DateRange.DateValue) AS Year,
                            ISNULL(ROUND(AVG(CAST(PlayerMatchStatistics.Goals AS FLOAT)), 2), 0) AS AverageGoals,
                            ISNULL(ROUND(AVG(CAST(PlayerMatchStatistics.Assists AS FLOAT)), 2), 0) AS AverageAssists,
                            ISNULL(ROUND(AVG(CAST(PlayerMatchStatistics.GoalsConceded AS FLOAT)), 2), 0) AS AverageGoalsConceded,
                            SUM(CASE WHEN GoalsConceded = 0 THEN 1 ELSE 0 END) As CleanSheets,
                            SUM(CASE WHEN PlayerMatchStatistics.MatchOutcome = {MatchOutcomeType.Win} THEN 1 ELSE 0 END) As Wins,
                            SUM(CASE WHEN PlayerMatchStatistics.MatchOutcome = {MatchOutcomeType.Draw} THEN 1 ELSE 0 END) As Draws,
                            SUM(CASE WHEN PlayerMatchStatistics.MatchOutcome = {MatchOutcomeType.Loss} THEN 1 ELSE 0 END) As Losses,
                            SUM(CASE WHEN CreatedDate IS NOT NULL THEN 1 ELSE 0 END) As Appearances
                        FROM dbo.GenerateDateRange(DATEADD(month, -1, GETDATE()), GETDATE(), 1) DateRange
                        LEFT JOIN dbo.PlayerMatchStatistics PlayerMatchStatistics
                            ON PlayerMatchStatistics.PlayerId = {playerId} AND CAST(PlayerMatchStatistics.CreatedDate AS DATE) = DateRange.DateValue
                        GROUP BY PlayerMatchStatistics.PlayerId,
                                DATEPART(day, DateRange.DateValue),
                                DATEPART(week, DateRange.DateValue),
                                DATEPART(month, DateRange.DateValue),
                                DATEPART(year, DateRange.DateValue)
                        ORDER BY DATEPART(year, DateRange.DateValue), DATEPART(month, DateRange.DateValue), DATEPART(week, DateRange.DateValue), DATEPART(day, DateRange.DateValue)")
                .ToList();
        }

        public List<TeamPerformanceSnapshot> GetDailyTeamPerformance(int teamId)
        {
            return _coachBotContext
                 .TeamPerformanceSnapshots
                 .FromSqlInterpolated($@"SELECT {teamId} AS TeamId,
                                    DATEPART(day, DateRange.DateValue) AS Day,
                                    DATEPART(week, DateRange.DateValue) AS Week,
                                    DATEPART(month, DateRange.DateValue) AS Month,
                                    DATEPART(year, DateRange.DateValue) AS Year,
                                    ISNULL(ROUND(AVG(CAST(TeamMatchStatistics.Goals AS FLOAT)), 2), 0) AS AverageGoals,
                                    ISNULL(ROUND(AVG(CAST(TeamMatchStatistics.Assists AS FLOAT)), 2), 0) AS AverageAssists,
                                    ISNULL(ROUND(AVG(CAST(TeamMatchStatistics.GoalsConceded AS FLOAT)), 2), 0) AS AverageGoalsConceded,
                                    SUM(CASE WHEN TeamMatchStatistics.GoalsConceded = 0 THEN 1 ELSE 0 END) As CleanSheets,
                                    SUM(CASE WHEN TeamMatchStatistics.MatchOutcome = {MatchOutcomeType.Win} THEN 1 ELSE 0 END) As Wins,
                                    SUM(CASE WHEN TeamMatchStatistics.MatchOutcome = {MatchOutcomeType.Draw} THEN 1 ELSE 0 END) As Draws,
                                    SUM(CASE WHEN TeamMatchStatistics.MatchOutcome = {MatchOutcomeType.Loss} THEN 1 ELSE 0 END) As Losses,
                                    SUM(CASE WHEN CreatedDate IS NOT NULL THEN 1 ELSE 0 END) As Matches
                        FROM dbo.GenerateDateRange(DATEADD(month, -1, GETDATE()), GETDATE(), 1) DateRange
                        LEFT JOIN dbo.TeamMatchStatistics TeamMatchStatistics
                            ON TeamMatchStatistics.TeamId = {teamId} AND CAST(TeamMatchStatistics.CreatedDate AS DATE) = DateRange.DateValue
                        GROUP BY TeamMatchStatistics.TeamId,
                                DATEPART(day, DateRange.DateValue),
                                DATEPART(week, DateRange.DateValue),
                                DATEPART(month, DateRange.DateValue),
                                DATEPART(year, DateRange.DateValue)
                        ORDER BY DATEPART(year, DateRange.DateValue), DATEPART(month, DateRange.DateValue), DATEPART(week, DateRange.DateValue), DATEPART(day, DateRange.DateValue)")
                .ToList();
        }

        public List<PlayerPerformanceSnapshot> GetContinuousPlayerPerformance(int playerId)
        {
            return _coachBotContext
                 .PlayerPerformanceSnapshots
                 .FromSqlInterpolated($@"SELECT TOP 30
                                    PlayerMatchStatistics.PlayerId,
                                    DATEPART(day, PlayerMatchStatistics.CreatedDate) AS Day,
                                    DATEPART(week, PlayerMatchStatistics.CreatedDate) AS Week,
                                    DATEPART(month, PlayerMatchStatistics.CreatedDate) AS Month,
                                    DATEPART(year, PlayerMatchStatistics.CreatedDate) AS Year,
                                    ROUND(AVG(CAST(PlayerMatchStatistics.Goals AS FLOAT)), 2) AS AverageGoals,
                                    ROUND(AVG(CAST(PlayerMatchStatistics.Assists AS FLOAT)), 2) AS AverageAssists,
                                    ROUND(AVG(CAST(PlayerMatchStatistics.GoalsConceded AS FLOAT)), 2) AS AverageGoalsConceded,
                                    COUNT(CASE WHEN GoalsConceded = 0 THEN 1 ELSE 0 END) As CleanSheets,
                                    SUM(CASE WHEN PlayerMatchStatistics.MatchOutcome = {MatchOutcomeType.Win} THEN 1 ELSE 0 END) As Wins,
                                    SUM(CASE WHEN PlayerMatchStatistics.MatchOutcome = {MatchOutcomeType.Draw} THEN 1 ELSE 0 END) As Draws,
                                    SUM(CASE WHEN PlayerMatchStatistics.MatchOutcome = {MatchOutcomeType.Loss} THEN 1 ELSE 0 END) As Losses,
                                    COUNT(*) As Appearances
                    FROM dbo.PlayerMatchStatistics PlayerMatchStatistics
                    WHERE PlayerMatchStatistics.PlayerId = {playerId}
                    GROUP BY PlayerMatchStatistics.PlayerId,
                            DATEPART(day, PlayerMatchStatistics.CreatedDate),
                            DATEPART(week, PlayerMatchStatistics.CreatedDate),
                            DATEPART(month, PlayerMatchStatistics.CreatedDate),
                            DATEPART(year, PlayerMatchStatistics.CreatedDate)
                    ORDER BY DATEPART(year, PlayerMatchStatistics.CreatedDate), DATEPART(month, PlayerMatchStatistics.CreatedDate), DATEPART(week, PlayerMatchStatistics.CreatedDate), DATEPART(day, PlayerMatchStatistics.CreatedDate)")
                .ToList();
        }

        public List<TeamPerformanceSnapshot> GetContinuousTeamPerformance(int teamId)
        {
            return _coachBotContext
                 .TeamPerformanceSnapshots
                 .FromSqlInterpolated($@"SELECT TOP 30
                                    {teamId} AS TeamId,
                                    DATEPART(day, TeamMatchStatistics.CreatedDate) AS Day,
                                    DATEPART(week, TeamMatchStatistics.CreatedDate) AS Week,
                                    DATEPART(month, TeamMatchStatistics.CreatedDate) AS Month,
                                    DATEPART(year, TeamMatchStatistics.CreatedDate) AS Year,
                                    ISNULL(ROUND(AVG(CAST(TeamMatchStatistics.Goals AS FLOAT)), 2), 0) AS AverageGoals,
                                    ISNULL(ROUND(AVG(CAST(TeamMatchStatistics.Assists AS FLOAT)), 2), 0) AS AverageAssists,
                                    ISNULL(ROUND(AVG(CAST(TeamMatchStatistics.GoalsConceded AS FLOAT)), 2), 0) AS AverageGoalsConceded,
                                    SUM(CASE WHEN TeamMatchStatistics.GoalsConceded = 0 THEN 1 ELSE 0 END) As CleanSheets,
                                    SUM(CASE WHEN TeamMatchStatistics.MatchOutcome = {MatchOutcomeType.Win} THEN 1 ELSE 0 END) As Wins,
                                    SUM(CASE WHEN TeamMatchStatistics.MatchOutcome = {MatchOutcomeType.Draw} THEN 1 ELSE 0 END) As Draws,
                                    SUM(CASE WHEN TeamMatchStatistics.MatchOutcome = {MatchOutcomeType.Loss} THEN 1 ELSE 0 END) As Losses,
                                    SUM(CASE WHEN CreatedDate IS NOT NULL THEN 1 ELSE 0 END) As Matches
                        FROM dbo.TeamMatchStatistics TeamMatchStatistics
                        WHERE TeamMatchStatistics.TeamId = {teamId}
                        GROUP BY TeamMatchStatistics.TeamId,
                                DATEPART(day, TeamMatchStatistics.CreatedDate),
                                DATEPART(week, TeamMatchStatistics.CreatedDate),
                                DATEPART(month, TeamMatchStatistics.CreatedDate),
                                DATEPART(year, TeamMatchStatistics.CreatedDate)
                        ORDER BY DATEPART(year, TeamMatchStatistics.CreatedDate), DATEPART(month, TeamMatchStatistics.CreatedDate), DATEPART(week, TeamMatchStatistics.CreatedDate), DATEPART(day, TeamMatchStatistics.CreatedDate)")
                .ToList();
        }

        private Position GetMostCommonPosition(PlayerTeam playerTeam)
        {
            var topPosition = _coachBotContext.PlayerPositionMatchStatistics
                 .AsNoTracking()
                 .Where(p => p.PlayerId == playerTeam.Player.Id)
                 .Where(p => p.Match.KickOff > playerTeam.JoinDate)
                 .Where(p => playerTeam.LeaveDate == null || p.Match.KickOff < playerTeam.LeaveDate)
                 .GroupBy(p => new { p.Position.Id, p.Position.Name })
                 .Select(p => new PositionAppearances()
                 {
                     Appearances = p.Count(),
                     Position = new Position()
                     {
                         Name = p.Key.Name,
                         Id = p.Key.Id
                     }
                 })
                 .OrderByDescending(p => p.Appearances)
                 .FirstOrDefault();

            return topPosition.Position;
        }

        private Player GetOrCreatePlayer(MatchDataPlayer matchDataPlayer)
        {
            var player = _coachBotContext.Players.FirstOrDefault(p => p.SteamID == matchDataPlayer.Info.SteamId64);

            if (player == null)
            {
                player = new Player()
                {
                    Name = matchDataPlayer.Info.Name,
                    SteamID = matchDataPlayer.Info.SteamId64
                };
                _coachBotContext.Players.Add(player);
            }

            return player;
        }

        private int? GetOrCreateMap(string mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))
            {
                return null;
            }

            var map = _coachBotContext.Maps.FirstOrDefault(m => m.Name == mapName.ToLower());

            if (map == null)
            {
                map = new Map()
                {
                    Name = mapName.ToLower()
                };
                _coachBotContext.Maps.Add(map);
                _coachBotContext.SaveChanges();
            }

            return map.Id;
        }

        private PlayerTeam MapToSimplePlayerTeamInstance(PlayerTeam playerTeam)
        {
            return new PlayerTeam()
            {
                TeamRole = playerTeam.TeamRole,
                JoinDate = playerTeam.JoinDate,
                LeaveDate = playerTeam.LeaveDate,                
                PlayerId = playerTeam.PlayerId,
                Player = new Player()
                {
                    Name = playerTeam.Player.Name,
                    Country = playerTeam.Player.Country,
                    SteamID = playerTeam.Player.SteamID
                },
                Team = new Team()
                {
                    Id = playerTeam.Team.Id,
                    Name = playerTeam.Team.Name,
                    BadgeImage = playerTeam.Team.BadgeImage
                }
            };
        }

        private struct PositionAppearances
        {
            public int Appearances;
            public Position Position;
        }

        #endregion Private Methods
    }
}