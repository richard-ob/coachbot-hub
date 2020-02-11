using CoachBot.Domain.Extensions;
using CoachBot.Model;
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

        public int MatchId { get; set; }

        public Match Match { get; set; }

        public MatchData MatchData { get; set; }

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
