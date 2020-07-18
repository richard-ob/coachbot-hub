using CoachBot.Domain.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class MatchStatistics
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public MatchData MatchData { get; set; }

        public string Token { get; set; }

        public DateTime? KickOff => MatchData != null? new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(MatchData.MatchInfo.StartTime) : (DateTime?)null;

        public string MapName => MatchData != null && MatchData.MatchInfo?.MapName != null ? MatchData.MatchInfo.MapName : null;

        public int? HomeGoals { get; set; }

        public int? AwayGoals { get; set; }

        public int MatchGoalsHome => MatchData.GetMatchStatistic(MatchDataStatisticType.Goals, MatchDataTeamType.Home);

        public int MatchGoalsAway => MatchData.GetMatchStatistic(MatchDataStatisticType.Goals, MatchDataTeamType.Away);

        public MatchOutcomeType GetMatchOutcomeTypeForTeam(MatchDataTeamType teamType)
        {
            var teamGoals = MatchData.GetMatchStatistic(MatchDataStatisticType.Goals, teamType);
            var opponentGoals = MatchData.GetMatchStatistic(MatchDataStatisticType.Goals, 1 - teamType);

            if (teamGoals > opponentGoals) return MatchOutcomeType.Win;
            if (opponentGoals > teamGoals) return MatchOutcomeType.Loss;

            return MatchOutcomeType.Draw;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}