using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using CoachBot.Model;
using System.Collections.Generic;
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

        public List<TeamStatisticTotals> GetTeamStatistics()
        {
            return null;
        }

        public List<PlayerStatisticTotals> GetPlayerStatistics()
        {
            return null;
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
            foreach(var matchDataPlayer in match.MatchStatistics.MatchData.Players)
            {
                var player = _coachBotContext.Players.FirstOrDefault(p => p.SteamID == matchDataPlayer.Info.SteamId);

                if (player == null)
                {
                    player = new Player()
                    {
                        Name = matchDataPlayer.Info.Name,
                        SteamID = matchDataPlayer.Info.SteamId
                    };
                    _coachBotContext.Players.Add(player);
                }

                AddMatchDataTotalsToPlayerStatisticTotals(player, match.MatchStatistics.MatchData);
            }
        }

        private void AddMatchDataTotalsToPlayerStatisticTotals(Player player, MatchData matchData)
        {
            // Game-generated statistics
            var playerTotalMatchStatistics = matchData.Players.First(p => p.Info.SteamId == player.SteamID).GetMatchStatisticsPlayerTotal();
            player.PlayerStatisticTotals.StatisticTotals.AddMatchDataStatistics(playerTotalMatchStatistics);

            // Custom statistics
            player.PlayerStatisticTotals.StatisticTotals.Matches++;
            var homeGoals = matchData.GetMatchStatistic(MatchDataStatisticType.Goals, MatchDataTeamType.Home);
            var awayGoals = matchData.GetMatchStatistic(MatchDataStatisticType.Goals, MatchDataTeamType.Away);
            var firstPeriod = matchData.Players.First(p => p.Info.SteamId == player.SteamID).MatchPeriodData.First();
            if ((homeGoals > awayGoals && firstPeriod.Info.IsHomeTeam) || (awayGoals > homeGoals && firstPeriod.Info.IsAwayTeam)) player.PlayerStatisticTotals.StatisticTotals.Wins++;
            if (homeGoals == awayGoals) player.PlayerStatisticTotals.StatisticTotals.Draws++;
            if ((awayGoals > homeGoals && firstPeriod.Info.IsHomeTeam) || (homeGoals > awayGoals && firstPeriod.Info.IsAwayTeam)) player.PlayerStatisticTotals.StatisticTotals.Losses++;
        }
        #endregion
    }
}
