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

        public MatchStatisticsService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
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
                GeneratePlayerMatchStatistics(match);
                GenerateTeamMatchStatistics(match);
                _coachBotContext.SaveChanges();
            }
        }

        public Model.Dtos.PagedResult<TeamStatisticTotals> GetTeamStatistics(int page, int pageSize, string sortOrder, StatisticsTimePeriod timePeriod, int? teamId)
        {
            return GetTeamStatisticTotals().GetPaged(page, pageSize, sortOrder);
        }

        public Model.Dtos.PagedResult<PlayerStatisticTotals> GetPlayerStatistics(int page, int pageSize, string sortOrder, StatisticsTimePeriod timePeriod)
        {
            return GetPlayerStatisticTotals().GetPaged(page, pageSize, sortOrder);
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

        #region Private Methods

        private void GeneratePlayerMatchStatistics(Match match)
        {
            foreach (var matchDataPlayer in match.MatchStatistics.MatchData.Players.Where(p => p.MatchPeriodData != null && p.MatchPeriodData.Any()))
            {
                var player = GetOrCreatePlayer(matchDataPlayer);
                foreach(var teamType in matchDataPlayer.MatchPeriodData.Select(m => m.Info.Team).Distinct())
                {
                    foreach(var position in matchDataPlayer.MatchPeriodData.Where(m => m.Info.Team == teamType).Select(m => m.Info.Position).Distinct())
                    {
                        var isHomeTeam = teamType == MatchDataSideConstants.Home;
                        var team = isHomeTeam ? match.LineupHome : match.LineupAway;
                        var positionId = _coachBotContext.Positions.Where(p => p.Name == position).Select(p => p.Id).FirstOrDefault();
                        var playerMatchStatistics = new PlayerMatchStatistics()
                        {
                            PlayerId = player.Id,
                            MatchId = match.Id,
                            ChannelId = (int)team.ChannelId,
                            TeamId = team.Channel.TeamId,
                            PositionId = positionId > 0 ? positionId : _coachBotContext.Positions.FirstOrDefault().Id,
                            MatchOutcome = match.MatchStatistics.GetMatchOutcomeTypeForTeam(isHomeTeam ? MatchDataTeamType.Home : MatchDataTeamType.Away),
                            SecondsPlayed = matchDataPlayer.GetPlayerPositionSeconds(teamType, position),
                            Nickname = matchDataPlayer.Info.Name,
                            Substitute = false
                        };
                        playerMatchStatistics.AddMatchDataStatistics(matchDataPlayer.GetPlayerTotals(teamType, position));
                        _coachBotContext.PlayerMatchStatistics.Add(playerMatchStatistics);
                        _coachBotContext.SaveChanges();
                    }
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
                    ChannelId = (int)team.ChannelId,
                    TeamId = team.Channel.TeamId,
                    MatchOutcome = match.MatchStatistics.GetMatchOutcomeTypeForTeam(isHomeTeam ? MatchDataTeamType.Home : MatchDataTeamType.Away),
                    TeamName = matchDataTeam.MatchTotal.Name
                };
                teamMatchStatistics.AddMatchDataStatistics(matchDataTeam.MatchTotal.Statistics);
                _coachBotContext.TeamMatchStatistics.Add(teamMatchStatistics);
                _coachBotContext.SaveChanges();
            }
        }

        private IQueryable<PlayerStatisticTotals> GetPlayerStatisticTotals()
        {
            return _coachBotContext
                 .PlayerMatchStatistics
                 .AsNoTracking()
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
                     m.ThrowIns,
                     m.Corners,
                     m.Goals,
                     m.Assists,
                     m.MatchOutcome
                 })
                 .GroupBy(p => new { p.PlayerId, p.SteamID, p.Name }, (key, s) => new PlayerStatisticTotals()
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
                     RedCards = s.Sum(p => p.RedCards),
                     RedCardsAverage = s.Average(p => p.RedCards),
                     YellowCards = s.Sum(p => p.YellowCards),
                     YellowCardsAverage = s.Average(p => p.YellowCards),
                     ThrowIns = s.Sum(p => p.ThrowIns),
                     Corners = s.Sum(p => p.Corners),
                     PlayerId = key.PlayerId,
                     Matches = s.Count(),
                     Wins = s.Sum(p => (int)p.MatchOutcome == (int)MatchOutcomeType.Win ? 1 : 0),
                     Losses = s.Sum(p => (int)p.MatchOutcome == (int)MatchOutcomeType.Loss ? 1 : 0),
                     Draws = s.Sum(p => (int)p.MatchOutcome == (int)MatchOutcomeType.Draw ? 1 : 0),
                     Name = key.Name,
                     SteamID = key.SteamID
                 });
        }


        private IQueryable<TeamStatisticTotals> GetTeamStatisticTotals()
        {
            return _coachBotContext
                 .TeamMatchStatistics
                 .AsNoTracking()
                 .Select(m => new
                 {
                     m.TeamId,
                     m.ChannelId,
                     m.TeamName,
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
                     m.ThrowIns,
                     m.Corners,
                     m.Goals,
                     m.Assists,
                     m.MatchOutcome
                 })
                 .GroupBy(p => new { p.TeamId, p.ChannelId, p.TeamName }, (key, s) => new TeamStatisticTotals()
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
                     RedCards = s.Sum(p => p.RedCards),
                     RedCardsAverage = s.Average(p => p.RedCards),
                     YellowCards = s.Sum(p => p.YellowCards),
                     YellowCardsAverage = s.Average(p => p.YellowCards),
                     ThrowIns = s.Sum(p => p.ThrowIns),
                     Corners = s.Sum(p => p.Corners),
                     Matches = s.Count(),
                     Wins = s.Sum(p => (int)p.MatchOutcome == (int)MatchOutcomeType.Win ? 1 : 0),
                     Losses = s.Sum(p => (int)p.MatchOutcome == (int)MatchOutcomeType.Loss ? 1 : 0),
                     Draws = s.Sum(p => (int)p.MatchOutcome == (int)MatchOutcomeType.Draw ? 1 : 0),
                     TeamId = key.TeamId,
                     TeamName = key.TeamName,
                     ChannelId = key.ChannelId
                 });
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
        #endregion
    }
}
