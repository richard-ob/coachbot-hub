using CoachBot.Domain.Model;

namespace CoachBot.Domain.Helpers
{
    public static class MatchDataHelper
    {
        public static int GetMatchStatistic(MatchData matchData, MatchDataStatisticType matchDataStatisticType, MatchDataTeamType matchDataTeamType)
        {
            return matchData.Teams[(int)matchDataTeamType].MatchTotal.Statistics[(int)matchDataStatisticType];
        }
    }
}