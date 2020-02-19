using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
                GenerateTeamStatisticTotals(match);
                GeneratePlayerStatisticTotals(match);
                _coachBotContext.SaveChanges();
            }
        }

        public PagedResult<TeamStatisticTotals> GetTeamStatistics(int page, int pageSize, string sortOrder, StatisticsTimePeriod timePeriod)
        {
            return _coachBotContext.TeamStatisticTotals
              .Include(ts => ts.StatisticTotals)
              .Where(ts => ts.StatisticTotals.TimePeriod == timePeriod)
              .GetPaged(page, pageSize, sortOrder);
        }

        public PagedResult<PlayerStatisticTotals> GetPlayerStatistics(int page, int pageSize, string sortOrder, StatisticsTimePeriod timePeriod)
        {
            return _coachBotContext.PlayerStatisticTotals
              .Include(ps => ps.Player)
              .Include(ps => ps.StatisticTotals)
              .Where(ps => ps.StatisticTotals.TimePeriod == timePeriod)
              .GetPaged(page, pageSize, sortOrder);
        }

        #region Private Methods

        private void GenerateTeamStatisticTotals(Match match)
        {
            var homeTeamStatisticTotals = _coachBotContext.TeamStatisticTotals.FirstOrDefault(t => t.ChannelId == match.TeamHome.ChannelId);
            if (homeTeamStatisticTotals == null)
            {
                homeTeamStatisticTotals = new TeamStatisticTotals
                {
                    ChannelId = (int)match.TeamHome.ChannelId
                };
                _coachBotContext.TeamStatisticTotals.Add(homeTeamStatisticTotals);
            }
            AddMatchDataTotalsToTeamStatisticTotals(ref homeTeamStatisticTotals, match.MatchStatistics.MatchData, MatchDataTeamType.Home);

            var awayTeamStatisticsTotals = _coachBotContext.TeamStatisticTotals.FirstOrDefault(t => t.ChannelId == match.TeamAway.ChannelId);
            if (awayTeamStatisticsTotals == null)
            {
                awayTeamStatisticsTotals = new TeamStatisticTotals
                {
                    ChannelId = (int)match.TeamAway.ChannelId
                };
                _coachBotContext.TeamStatisticTotals.Add(awayTeamStatisticsTotals);
            }
            AddMatchDataTotalsToTeamStatisticTotals(ref awayTeamStatisticsTotals, match.MatchStatistics.MatchData, MatchDataTeamType.Away);
        }

        private void AddMatchDataTotalsToTeamStatisticTotals(ref TeamStatisticTotals teamStatisticTotals, MatchData matchData, MatchDataTeamType matchDataTeamType)
        {
            // Game-generated statistics
            var matchDataTeamMatchTotal = matchData.Teams[(int)matchDataTeamType].MatchTotal;
            teamStatisticTotals.StatisticTotals.AddMatchDataStatistics(matchDataTeamMatchTotal.Statistics);

            // Custom statistics
            teamStatisticTotals.StatisticTotals.Matches++;
            var teamGoals = matchData.GetMatchStatistic(MatchDataStatisticType.Goals, matchDataTeamType);
            var opponentGoals = matchData.GetMatchStatistic(MatchDataStatisticType.Goals, 1 - matchDataTeamType);
            if (teamGoals > opponentGoals) teamStatisticTotals.StatisticTotals.Wins++;
            if (teamGoals == opponentGoals) teamStatisticTotals.StatisticTotals.Draws++;
            if (opponentGoals > teamGoals) teamStatisticTotals.StatisticTotals.Losses++;
        }

        private void GeneratePlayerStatisticTotals(Match match)
        {
            foreach (var matchDataPlayer in match.MatchStatistics.MatchData.Players)
            {
                var player = _coachBotContext.Players.FirstOrDefault(p => p.SteamID == matchDataPlayer.Info.SteamId);
                var playerStatisticTotals = _coachBotContext.PlayerStatisticTotals.FirstOrDefault(p => player != null && p.PlayerId == player.Id) ?? new PlayerStatisticTotals();
                if (player == null)
                {
                    player = new Player()
                    {
                        Name = matchDataPlayer.Info.Name,
                        SteamID = matchDataPlayer.Info.SteamId
                    };
                    _coachBotContext.Players.Add(player);
                    playerStatisticTotals.Player = player;
                    _coachBotContext.PlayerStatisticTotals.Add(playerStatisticTotals);
                }
                AddMatchDataTotalsToPlayerStatisticTotals(ref playerStatisticTotals, match.MatchStatistics.MatchData, player.SteamID);
            }
        }

        private void AddMatchDataTotalsToPlayerStatisticTotals(ref PlayerStatisticTotals playerStatisticTotals, MatchData matchData, string steamId)
        {
            // Game-generated statistics
            var playerTotalMatchStatistics = matchData.Players.First(p => p.Info.SteamId == steamId).GetMatchStatisticsPlayerTotal();
            playerStatisticTotals.StatisticTotals.AddMatchDataStatistics(playerTotalMatchStatistics);

            // Custom statistics
            playerStatisticTotals.StatisticTotals.Matches++;
            var homeGoals = matchData.GetMatchStatistic(MatchDataStatisticType.Goals, MatchDataTeamType.Home);
            var awayGoals = matchData.GetMatchStatistic(MatchDataStatisticType.Goals, MatchDataTeamType.Away);
            var firstPeriod = matchData.Players.First(p => p.Info.SteamId == steamId).MatchPeriodData.First();
            if ((homeGoals > awayGoals && firstPeriod.Info.IsHomeTeam) || (awayGoals > homeGoals && firstPeriod.Info.IsAwayTeam)) playerStatisticTotals.StatisticTotals.Wins++;
            if (homeGoals == awayGoals) playerStatisticTotals.StatisticTotals.Draws++;
            if ((awayGoals > homeGoals && firstPeriod.Info.IsHomeTeam) || (homeGoals > awayGoals && firstPeriod.Info.IsAwayTeam)) playerStatisticTotals.StatisticTotals.Losses++;
        }
        #endregion
    }
}
