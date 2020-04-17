using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Model;
using Microsoft.EntityFrameworkCore;
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
            /*var queryable = _coachBotContext.TeamStatisticTotals
              .Include(ts => ts.Channel)
                .ThenInclude(c => c.Team)
                .ThenInclude(t => t.Region)
              .Include(ts => ts.StatisticTotals)
              .Where(ts => ts.StatisticTotals.TimePeriod == timePeriod);

            if (teamId != null && teamId > 0)
            {
                queryable.Where(ts => ts.Channel.TeamId == teamId);
            }

            return queryable.GetPaged(page, pageSize, sortOrder);*/
            return null;
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
                 .GroupBy(p => new { p.PlayerId, p.Player.Name, p.Player.SteamID, p.Player.DiscordUserId }, (key, grp) => 
                 .Select(s => new PlayerStatisticTotals()
                 {
                     RedCards = s.Sum(p => p.RedCards),
                     YellowCards = s.Sum(p => p.YellowCards),
                     Fouls = s.Sum(p => p.Fouls),
                     FoulsSuffered = s.Sum(p => p.FoulsSuffered),
                     SlidingTackles = s.Sum(p => p.SlidingTackles),
                     SlidingTacklesCompleted = s.Sum(p => p.SlidingTacklesCompleted),
                     GoalsConceded = s.Sum(p => p.GoalsConceded),
                     Shots = s.Sum(p => p.Shots),
                     ShotsOnGoal = s.Sum(p => p.ShotsOnGoal),
                     Passes = s.Sum(p => p.Passes),
                     PassesCompleted = s.Sum(p => p.PassesCompleted),
                     Interceptions = s.Sum(p => p.Interceptions),
                     Offsides = s.Sum(p => p.Offsides),
                     GoalKicks = s.Sum(p => p.GoalKicks),
                     OwnGoals = s.Sum(p => p.OwnGoals),
                     DistanceCovered = s.Sum(p => p.DistanceCovered),
                     FreeKicks = s.Sum(p => p.FreeKicks),
                     KeeperSaves = s.Sum(p => p.KeeperSaves),
                     KeeperSavesCaught = s.Sum(p => p.KeeperSavesCaught),
                     Penalties = s.Sum(p => p.Penalties),
                     Possession = s.Sum(p => p.Possession),
                     ThrowIns = s.Sum(p => p.ThrowIns),
                     Corners = s.Sum(p => p.Corners),
                     Goals = s.Sum(p => p.Goals),
                     Assists = s.Sum(p => p.Assists),
                     PlayerId = s.Key.PlayerId,
                     Matches = s.Count(),
                     Wins = s.Key.Name.Length > 8 ? 1 : 2,
                     /*Losses = s.Where(t => t.MatchOutcome == MatchOutcomeType.Loss).Count(),
                     Draws = s.Where(t => t.MatchOutcome == MatchOutcomeType.Draw).Count(),*/
                     Player = new Player()
                     {
                         Name = s.Key.Name,
                         DiscordUserId = s.Key.DiscordUserId,
                         SteamID = s.Key.SteamID
                     }
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
