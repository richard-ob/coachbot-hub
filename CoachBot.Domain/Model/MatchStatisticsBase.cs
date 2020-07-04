using CoachBot.Domain.Attributes;

namespace CoachBot.Domain.Model
{
    public class MatchStatisticsBase
    {
        [MatchDataStatistic(MatchDataStatisticType.RedCards)]
        public int RedCards { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.YellowCards)]
        public int YellowCards { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Fouls)]
        public int Fouls { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.FoulsSuffered)]
        public int FoulsSuffered { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.SlidingTackles)]
        public int SlidingTackles { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.SlidingTacklesCompleted)]
        public int SlidingTacklesCompleted { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.GoalsConceded)]
        public int GoalsConceded { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Shots)]
        public int Shots { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.ShotsOnGoal)]
        public int ShotsOnGoal { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.PassesCompleted)]
        public int PassesCompleted { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Interceptions)]
        public int Interceptions { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Offsides)]
        public int Offsides { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Goals)]
        public int Goals { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.OwnGoals)]
        public int OwnGoals { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Assists)]
        public int Assists { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Passes)]
        public int Passes { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.OwnGoals)]
        public int FreeKicks { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.OwnGoals)]
        public int Penalties { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Corners)]
        public int Corners { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.ThrowIns)]
        public int ThrowIns { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.KeeperSaves)]
        public int KeeperSaves { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.GoalKicks)]
        public int GoalKicks { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Possession)]
        public int Possession { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.DistanceCovered)]
        public int DistanceCovered { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.KeeperSavesCaught)]
        public int KeeperSavesCaught { get; set; }

        // Manually generated stats

        public int PossessionPercentage { get; set; }

        public MatchOutcomeType MatchOutcome { get; set; }
    }
}