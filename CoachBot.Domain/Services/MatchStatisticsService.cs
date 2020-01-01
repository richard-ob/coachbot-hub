using CoachBot.Database;
using CoachBot.Domain.Model;
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

        public void SaveMatchData(MatchData matchData, int matchId)
        {
            var matchStatistics = new MatchStatistics()
            {
                MatchId = matchId,
                MatchData = matchData
            };
            _coachBotContext.MatchStatistics.Add(matchStatistics);
            _coachBotContext.SaveChanges();
        }

        public MatchStatistics GetMatchStatistics(int matchId)
        {
            return _coachBotContext.MatchStatistics.FirstOrDefault(m => m.MatchId == matchId);
        }

        public List<TeamStatisticTotals> GetTeamStatistics()
        {
            return null;
        }

        public List<PlayerStatisticTotals> GetPlayerStatistics()
        {
            return null;
        }
    }
}
