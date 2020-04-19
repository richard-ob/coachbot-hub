using CoachBot.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoachBot.Domain.Extensions
{
    public static class MatchDataExtensions
    {
        private const double EXPECTED_PLAYERCOUNT_THRESHOLD_MULTIPLIER = 0.75;

        public static int GetMatchStatistic(this MatchData matchData, MatchDataStatisticType matchDataStatisticType, MatchDataTeamType matchDataTeamType)
        {
            return matchData.Teams[(int)matchDataTeamType].MatchTotal.Statistics[(int)matchDataStatisticType];
        }

        public static int GetMatchStatisticPlayerTotal(this MatchDataPlayer matchDataPlayer, MatchDataStatisticType matchDataStatisticType)
        {
            return matchDataPlayer.MatchPeriodData.Sum(m => m.Statistics[(int)matchDataStatisticType]);
        }

        public static int GetMatchStatisticPlayerTotal(this MatchDataPlayer matchDataPlayer, MatchDataStatisticType matchDataStatisticType, string team, string position)
        {
            return matchDataPlayer.MatchPeriodData.Where(m => m.Info.Team == team && m.Info.Position == position).Sum(m => m.Statistics[(int)matchDataStatisticType]);
        }

        public static List<int> GetPlayerTotals(this MatchDataPlayer matchDataPlayer, string team, string position)
        {
            var statisticTotals = new List<int>();
            foreach (MatchDataStatisticType statisticType in Enum.GetValues(typeof(MatchDataStatisticType)))
            {
                statisticTotals.Add(matchDataPlayer.GetMatchStatisticPlayerTotal(statisticType, team, position));
            }

            return statisticTotals;
        }

        public static int GetPlayerPositionSeconds(this MatchDataPlayer matchDataPlayer, string team, string position)
        {
            return matchDataPlayer.MatchPeriodData.Where(m => m.Info.Team == team && m.Info.Position == position).Sum(m => m.Info.EndSecond - m.Info.StartSecond);
        }

        public static int GetPlayerSeconds(this MatchDataPlayer matchDataPlayer, string team)
        {
            return matchDataPlayer.MatchPeriodData.Where(m => m.Info.Team == team).Sum(m => m.Info.EndSecond - m.Info.StartSecond);
        }

        public static List<int> GetMatchStatisticsPlayerTotal(this MatchDataPlayer matchDataPlayer)
        {
            var statistics = new List<int>();
            foreach (MatchDataStatisticType statisticType in Enum.GetValues(typeof(MatchDataStatisticType)))
            {
                statistics.Add(matchDataPlayer.GetMatchStatisticPlayerTotal(statisticType));
            }

            return statistics;
        }

        public static bool IsValid(this MatchData matchData, Match match, bool manualSave)
        {
            // Validate match has correct player counts
            var expectedPlayerCount = match.SignedPlayers.Count();
            var actualPlayerCount = matchData.Players.Count();
            if (expectedPlayerCount * EXPECTED_PLAYERCOUNT_THRESHOLD_MULTIPLIER > actualPlayerCount)
            {
                throw new Exception($"Too few players present in match data. Expected at least {expectedPlayerCount}, found {actualPlayerCount}.");
            }

            // Validate match took place within an hour of the match ready time
            if (DateTime.Now.AddHours(-2) > match.ReadiedDate && !manualSave)
            {
                throw new Exception("The match should finish no later than two hours after being readied.");
            }

            return true;
        }
    }
}
