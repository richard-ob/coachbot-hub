using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace CoachBot.Domain.Services
{
    public class MatchStatisticsService
    {
        private readonly CoachBotContext _coachBotContext;
        private readonly FantasyService _fantasyService;

        public MatchStatisticsService(CoachBotContext coachBotContext, FantasyService fantasyService)
        {
            _coachBotContext = coachBotContext;
            _fantasyService = fantasyService;
        }

        public void SaveMatchData(MatchData matchData, int matchId, bool manualSave = false)
        {
            var match = _coachBotContext.Matches.Single(m => m.Id == matchId);
            match.MatchStatistics = new MatchStatistics()
            {
                MatchData = matchData
            };

            if (matchData.IsValid(match, manualSave))
            {
                _coachBotContext.SaveChanges();
                GeneratePlayerMatchStatistics(match);
                GenerateTeamMatchStatistics(match);
                GenerateTeamForm();
                if (match.TournamentId.HasValue && match.TournamentId > 0)
                {
                    var tournamentPhaseId = _coachBotContext.TournamentGroupMatches.Single(m => m.MatchId == match.Id).TournamentPhaseId;
                    _fantasyService.GenerateFantasyPhaseSnapshots(tournamentPhaseId);
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

        public Model.Dtos.PagedResult<PlayerPositionMatchStatistics> GetPlayerPositionMatchStatistics(int page, int pageSize, string sortOrder, PlayerStatisticFilters filters)
        {
            return GetPlayerPositionMatchStatisticsQueryable(filters).GetPaged(page, pageSize, sortOrder);
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
                .OrderByDescending(p => p.JoinDate);

            var allPlayerTeamStatisticTotals = new List<PlayerTeamStatisticsTotals>();

            foreach (var playerTeam in playerTeams.Take(30))
            {
                var filter = new PlayerStatisticFilters()
                {
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
                .Include(m => m.LineupHome)
                    .ThenInclude(t => t.Channel)
                .Include(m => m.LineupAway)
                    .ThenInclude(t => t.Channel)
                .Where(m => m.MatchStatistics != null);
            
            foreach(var match in matches)
            {
                GeneratePlayerMatchStatistics(match);
                GenerateTeamMatchStatistics(match);
            }
        }

        public List<MatchDayTotals> GetPlayerMatchDayTotals(int playerId)
        {
            return _coachBotContext.PlayerPositionMatchStatistics
                .AsNoTracking()
                .Where(p => p.PlayerId == playerId)
                .Where(p => p.Match.ReadiedDate != null && p.Match.ReadiedDate.Value.Year == DateTime.Now.Year)
                .GroupBy(p => p.Match.ReadiedDate.Value.Date)
                .Select(s => new MatchDayTotals() {
                    Matches = s.Count(),
                    MatchDayDate = s.Key
                }).ToList();
        }

        public List<MatchDayTotals> GetTeamMatchDayTotals(int teamId)
        {
            return _coachBotContext.TeamMatchStatistics
                .AsNoTracking()
                .Where(t => t.TeamId == teamId)
                .Where(t => t.Match.ReadiedDate != null && t.Match.ReadiedDate.Value.Year == DateTime.Now.Year)
                .GroupBy(t => t.Match.ReadiedDate.Value.Date)
                .Select(s => new MatchDayTotals()
                {
                    Matches = s.Count(),
                    MatchDayDate = s.Key
                }).ToList();
        }

        public void GenerateTeamForm()
        {
            foreach (var teamId in _coachBotContext.TeamMatchStatistics.Select(t => t.TeamId).Distinct())
            {
                var team = _coachBotContext.Teams.Include(t => t.TeamMatchStatistics).Single(t => t.Id == teamId);
                team.Form = team.TeamMatchStatistics.OrderByDescending(t => t.CreatedDate).Take(5).Select(m => m.MatchOutcome).ToList();
            }
            _coachBotContext.SaveChanges();
        }

        #region Private Methods
        
        private void GeneratePlayerMatchStatistics(Match match)
        {
            foreach (var matchDataPlayer in match.MatchStatistics.MatchData.Players.Where(p => p.MatchPeriodData != null && p.MatchPeriodData.Any()))
            {
                if (matchDataPlayer.Info.SteamId == "BOT") continue;

                var player = GetOrCreatePlayer(matchDataPlayer);
                foreach (var teamType in matchDataPlayer.MatchPeriodData.Select(m => m.Info.Team).Distinct())
                {
                    var isHomeTeam = teamType == MatchDataSideConstants.Home;
                    var team = isHomeTeam ? match.LineupHome : match.LineupAway;
                    foreach (var position in matchDataPlayer.MatchPeriodData.Where(m => m.Info.Team == teamType).Select(m => m.Info.Position).Distinct())
                    {
                        var positionId = _coachBotContext.Positions.Where(p => p.Name == position).Select(p => p.Id).FirstOrDefault();
                        var playerPositionMatchStatistics = new PlayerPositionMatchStatistics()
                        {
                            PlayerId = player.Id,
                            MatchId = match.Id,
                            ChannelId = team != null ? (int)team.ChannelId : 1,
                            TeamId =  team != null ? team.Channel.TeamId : match.TeamHomeId,
                            PositionId = positionId > 0 ? positionId : _coachBotContext.Positions.FirstOrDefault().Id,
                            MatchOutcome = match.MatchStatistics.GetMatchOutcomeTypeForTeam(isHomeTeam ? MatchDataTeamType.Home : MatchDataTeamType.Away),
                            SecondsPlayed = matchDataPlayer.GetPlayerPositionSeconds(teamType, position),
                            PossessionPercentage = match.MatchStatistics.MatchData.GetPlayerPositionPossession(matchDataPlayer, teamType, position),
                            Nickname = matchDataPlayer.Info.Name,
                            Substitute = matchDataPlayer.WasPlayerSubstitute(teamType, position)
                        };
                        playerPositionMatchStatistics.AddMatchDataStatistics(matchDataPlayer.GetPlayerTotals(teamType, position));
                        _coachBotContext.PlayerPositionMatchStatistics.Add(playerPositionMatchStatistics);
                    }
                    var playerMatchStatistics = new PlayerMatchStatistics()
                    {
                        PlayerId = player.Id,
                        MatchId = match.Id,
                        ChannelId = team != null ? (int)team.ChannelId : 1,
                        TeamId = team != null ? team.Channel.TeamId : match.TeamHomeId,
                        MatchOutcome = match.MatchStatistics.GetMatchOutcomeTypeForTeam(isHomeTeam ? MatchDataTeamType.Home : MatchDataTeamType.Away),
                        SecondsPlayed = matchDataPlayer.GetPlayerSeconds(teamType),
                        PossessionPercentage = match.MatchStatistics.MatchData.GetPlayerPossession(matchDataPlayer),
                        Nickname = matchDataPlayer.Info.Name,
                        Substitute = matchDataPlayer.WasPlayerSubstitute(teamType)
                    };
                    playerMatchStatistics.AddMatchDataStatistics(matchDataPlayer.GetMatchStatisticsPlayerTotal());
                    _coachBotContext.PlayerMatchStatistics.Add(playerMatchStatistics);
                    _coachBotContext.SaveChanges();
                }
            }
        }

        private void GenerateTeamMatchStatistics(Match match)
        {
            foreach (var matchDataTeam in match.MatchStatistics.MatchData.Teams)
            {
                var isHomeTeam = matchDataTeam.MatchTotal.Side == MatchDataSideConstants.Home;
                var team = isHomeTeam ? match.LineupHome : match.LineupAway;
                var teamMatchStatistics = new TeamMatchStatistics()
                {
                    MatchId = match.Id,
                    ChannelId = team != null ? (int)team.ChannelId : 1,
                    TeamId = team != null ? team.Channel.TeamId : match.TeamHomeId,
                    MatchOutcome = match.MatchStatistics.GetMatchOutcomeTypeForTeam(isHomeTeam ? MatchDataTeamType.Home : MatchDataTeamType.Away),
                    PossessionPercentage = match.MatchStatistics.MatchData.GetTeamPosession(isHomeTeam ? MatchDataTeamType.Home : MatchDataTeamType.Away),
                    TeamName = matchDataTeam.MatchTotal.Name
                };
                teamMatchStatistics.AddMatchDataStatistics(matchDataTeam.MatchTotal.Statistics);
                _coachBotContext.TeamMatchStatistics.Add(teamMatchStatistics);
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
                .Where(p => filters.ChannelId == null || p.ChannelId == filters.ChannelId)
                .Where(p => filters.PositionId == null || p.PositionId == filters.PositionId)
                .Where(p => filters.RegionId == null || p.Team.RegionId == filters.RegionId)
                .Where(p => filters.TournamentId == null || p.Match.TournamentId == filters.TournamentId)
                .Where(p => string.IsNullOrWhiteSpace(filters.PlayerName) || p.Player.Name.Contains(filters.PlayerName))
                .Where(p => filters.TimePeriod != StatisticsTimePeriod.Week || p.Match.ReadiedDate > DateTime.Now.AddDays(-7))
                .Where(p => filters.TimePeriod != StatisticsTimePeriod.Month || p.Match.ReadiedDate > DateTime.Now.AddMonths(-1))
                .Where(p => filters.TimePeriod != StatisticsTimePeriod.Year || p.Match.ReadiedDate > DateTime.Now.AddYears(-1))
                .Include(p => p.Match)
                    .ThenInclude(p => p.TeamHome)
                .Include(p => p.Match)
                    .ThenInclude(p => p.TeamAway)
                .Include(p => p.Team)
                .Include(c => c.Channel)
                .Include(p => p.Position)
                .Where(p => filters.PlayerId == null || p.PlayerId == filters.PlayerId);
        }

        private IQueryable<PlayerStatisticTotals> GetPlayerStatisticTotals(PlayerStatisticFilters filters)
        {
            return _coachBotContext
                 .PlayerPositionMatchStatistics
                 .AsNoTracking()
                 .Where(p => filters.IncludeSubstituteAppearances || !p.Substitute)
                 .Where(p => filters.MatchId == null || p.MatchId == filters.MatchId)
                 .Where(p => filters.PlayerId == null || p.PlayerId == filters.PlayerId)
                 .Where(p => filters.TeamId == null || p.TeamId == filters.TeamId)
                 .Where(p => filters.ChannelId == null || p.ChannelId == filters.ChannelId)
                 .Where(p => filters.PositionId == null || p.PositionId == filters.PositionId)
                 .Where(p => string.IsNullOrWhiteSpace(filters.PositionName) || p.Position.Name == filters.PositionName)
                 .Where(p => filters.RegionId == null || p.Team.RegionId == filters.RegionId)
                 .Where(p => filters.TournamentId == null || p.Match.TournamentId == filters.TournamentId)
                 .Where(p => string.IsNullOrWhiteSpace(filters.PlayerName) || p.Player.Name.Contains(filters.PlayerName))
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Week || p.Match.ReadiedDate > DateTime.Now.AddDays(-7))
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Month || p.Match.ReadiedDate > DateTime.Now.AddMonths(-1))
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Year || p.Match.ReadiedDate > DateTime.Now.AddYears(-1))
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
                     ShotAccuracyPercentage = s.Average(p => p.Shots > 0 ? Convert.ToDouble(p.ShotsOnGoal) / Convert.ToDouble(p.Shots) : 0),
                     Passes = s.Sum(p => p.Passes),
                     PassesAverage = s.Average(p => p.Passes),
                     PassesCompleted = s.Sum(p => p.PassesCompleted),
                     PassesCompletedAverage = s.Average(p => p.PassesCompleted),
                     PassCompletionPercentageAverage = s.Average(p => p.Passes > 0 ? Convert.ToDouble(p.PassesCompleted) / Convert.ToDouble(p.Passes) : 0),
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
                     SubstituteAppearances = s.Sum(p => p.Substitute ? 1 : 0),
                     Wins = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Win ? 1 : 0),
                     Losses = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Loss ? 1 : 0),
                     Draws = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Draw ? 1 : 0),
                     Name = key.Name,
                     SteamID = key.SteamID,
                     Rating = key.Rating
                 });
        }

        private IQueryable<TeamStatisticTotals> GetTeamStatisticTotals(TeamStatisticsFilters filters)
        {
            return _coachBotContext
                 .TeamMatchStatistics
                 .Where(t => filters.TournamentId == null || t.TournamentId == filters.TournamentId)
                 .Where(t => filters.TeamId == null || t.TeamId == filters.TeamId)
                 .Where(t => filters.RegionId == null || t.Team.RegionId == filters.RegionId)
                 .Where(t => filters.RegionId == null || t.Team.RegionId == filters.RegionId)
                 .Where(t => filters.IncludeInactive || t.Team.Inactive == false)
                 .Where(t => filters.TeamType == null || t.Team.TeamType == filters.TeamType)
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Week || p.Match.ReadiedDate > DateTime.Now.AddDays(-7))
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Month || p.Match.ReadiedDate > DateTime.Now.AddMonths(-1))
                 .Where(p => filters.TimePeriod != StatisticsTimePeriod.Year || p.Match.ReadiedDate > DateTime.Now.AddYears(-1))
                 .AsNoTracking()
                 .Select(m => new
                 {
                     m.TeamId,
                     m.ChannelId,
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
                     m.Team.BadgeImage.Base64EncodedImage,
                     m.Team.Form,
                     m.Team.Name
                 })
                 .GroupBy(p => new { p.TeamId, p.ChannelId, p.Name, p.Base64EncodedImage, p.Form }, (key, s) => new TeamStatisticTotals()
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
                     ShotAccuracyPercentage = s.Average(p => p.Shots > 0 ? Convert.ToDouble(p.ShotsOnGoal) / Convert.ToDouble(p.Shots) : 0),
                     Passes = s.Sum(p => p.Passes),
                     PassesAverage = s.Average(p => p.Passes),
                     PassesCompleted = s.Sum(p => p.PassesCompleted),
                     PassesCompletedAverage = s.Average(p => p.PassesCompleted),
                     PassCompletionPercentageAverage = s.Average(p => p.Passes > 0 ? Convert.ToDouble(p.PassesCompleted) / Convert.ToDouble(p.Passes) : 0),
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
                     Wins = s.Sum(p => (int)p.MatchOutcome == (int)MatchOutcomeType.Win ? 1 : 0),
                     Losses = s.Sum(p => (int)p.MatchOutcome == (int)MatchOutcomeType.Loss ? 1 : 0),
                     Draws = s.Sum(p => (int)p.MatchOutcome == (int)MatchOutcomeType.Draw ? 1 : 0),
                     TeamId = key.TeamId,
                     TeamName = key.Name,
                     ChannelId = key.ChannelId,
                     BadgeImage = key.Base64EncodedImage,
                     Form = key.Form,
                     GoalDifference = s.Sum(p => p.Goals) - s.Sum(p => p.GoalsConceded),
                     Points = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Win ? 3 : p.MatchOutcome == MatchOutcomeType.Draw ? 1 : 0)
                 });
        }

        private Position GetMostCommonPosition(PlayerTeam playerTeam)
        {
           var topPosition = _coachBotContext.PlayerPositionMatchStatistics
                .AsNoTracking()
                .Where(p => p.PlayerId == playerTeam.Player.Id)
                .Where(p => p.Match.ReadiedDate > playerTeam.JoinDate)
                .Where(p => playerTeam.LeaveDate == null || p.Match.ReadiedDate < playerTeam.LeaveDate)
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
                    Country = playerTeam.Player.Country
                },
                Team = new Team()
                {
                    Name = playerTeam.Team.Name,
                    BadgeImage = playerTeam.Team.BadgeImage
                }
            };
        }

        struct PositionAppearances
        {
            public int Appearances;
            public Position Position;
        }
        #endregion
    }
}
