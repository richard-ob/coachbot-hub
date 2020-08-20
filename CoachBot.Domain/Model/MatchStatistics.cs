using CoachBot.Database;
using CoachBot.Domain.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class MatchStatistics: ISystemEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public MatchData MatchData { get; set; }

        public string Token { get; set; }

        public string SourceAddress { get; set; }

        public DateTime? KickOff => MatchData != null? new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(MatchData.MatchInfo.StartTime) : (DateTime?)null;

        public int? HomeGoals { get; set; }

        public int? AwayGoals { get; set; }

        public int MatchGoalsHome => HomeGoals ?? MatchData.GetMatchStatistic(MatchDataStatisticType.Goals, MatchDataTeamType.Home);

        public int MatchGoalsAway => AwayGoals ?? MatchData.GetMatchStatistic(MatchDataStatisticType.Goals, MatchDataTeamType.Away);

        public MatchOutcomeType GetMatchOutcomeTypeForTeam(MatchDataTeamType teamType)
        {
            var teamGoals = teamType == MatchDataTeamType.Home ? MatchGoalsHome : MatchGoalsAway;
            var opponentGoals = teamType == MatchDataTeamType.Home ? MatchGoalsAway : MatchGoalsHome;

            if (teamGoals > opponentGoals) return MatchOutcomeType.Win;
            if (opponentGoals > teamGoals) return MatchOutcomeType.Loss;

            return MatchOutcomeType.Draw;
        }

        public MatchDataTeamType KnockoutMatchWinner => MatchGoalsHome > MatchGoalsAway ? MatchDataTeamType.Home : MatchDataTeamType.Away;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}