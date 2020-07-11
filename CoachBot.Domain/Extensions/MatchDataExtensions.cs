using CoachBot.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static bool WasPlayerSubstitute(this MatchDataPlayer matchDataPlayer, string team)
        {
            return !matchDataPlayer.MatchPeriodData.Where(m => m.Info.Team == team).Any(m => m.Info.StartSecond == 0);
        }

        public static bool WasPlayerSubstitute(this MatchDataPlayer matchDataPlayer, string team, string position)
        {
            return !matchDataPlayer.MatchPeriodData.Where(m => m.Info.Team == team && m.Info.Position == position).Any(m => m.Info.StartSecond == 0);
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

        public static int GetTotalTeamPosession(this MatchData matchData)
        {
            var homePossession = GetMatchStatistic(matchData, MatchDataStatisticType.Possession, MatchDataTeamType.Home);
            var awayPossession = GetMatchStatistic(matchData, MatchDataStatisticType.Possession, MatchDataTeamType.Away);
            var totalPosession = homePossession + awayPossession;

            return totalPosession;
        }

        public static int GetTotalPlayerPosession(this MatchData matchData)
        {
            return matchData.Players.Sum(player => GetMatchStatisticPlayerTotal(player, MatchDataStatisticType.Possession));
        }

        public static int GetPlayerPositionPossession(this MatchData matchData, MatchDataPlayer matchDataPlayer, string team, string position)
        {
            var playerPositionPossession = matchDataPlayer.GetMatchStatisticPlayerTotal(MatchDataStatisticType.Possession, team, position);
            var totalGamePossession = GetTotalPlayerPosession(matchData);
            var roundedPercentage = (int)(((float)playerPositionPossession / (float)totalGamePossession) * 100);

            return roundedPercentage;
        }

        public static int GetPlayerPossession(this MatchData matchData, MatchDataPlayer matchDataPlayer)
        {
            var playerPositionPossession = GetMatchStatisticPlayerTotal(matchDataPlayer, MatchDataStatisticType.Possession);
            var totalGamePossession = GetTotalPlayerPosession(matchData);
            var roundedPercentage = (int)(((float)playerPositionPossession / (float)totalGamePossession) * 100);

            return roundedPercentage;
        }

        public static int GetTeamPosession(this MatchData matchData, MatchDataTeamType matchDataTeamType)
        {
            var homePossession = GetMatchStatistic(matchData, MatchDataStatisticType.Possession, MatchDataTeamType.Home);
            var awayPossession = GetMatchStatistic(matchData, MatchDataStatisticType.Possession, MatchDataTeamType.Away);
            var totalPosession = homePossession + awayPossession;

            if (matchDataTeamType == MatchDataTeamType.Home)
            {
                return (homePossession / totalPosession) * 100;
            }
            else
            {
                return (awayPossession / totalPosession) * 100;
            }
        }

        public static bool IsValid(this MatchData matchData, Match match, bool manualSave)
        {
            if (manualSave)
            {
                return true;
            }

            // Validate match took place within an hour of the match ready time
            if (DateTime.UtcNow.AddHours(-2) > match.KickOff && !manualSave)
            {
                throw new Exception("The match should finish no later than two hours after being readied.");
            }

            if (match.TournamentId.HasValue)
            {
                return true;
            }

            // Validate match has correct player counts
            var expectedPlayerCount = (int)match.Format * 2;
            var actualPlayerCount = matchData.Players.Count();
            if (expectedPlayerCount * EXPECTED_PLAYERCOUNT_THRESHOLD_MULTIPLIER > actualPlayerCount)
            {
                throw new Exception($"Too few players present in match data. Expected at least {expectedPlayerCount}, found {actualPlayerCount}.");
            }

            return true;
        }
    }
}