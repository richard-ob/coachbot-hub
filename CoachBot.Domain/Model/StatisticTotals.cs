using CoachBot.Domain.Attributes;

namespace CoachBot.Domain.Model
{
    public class StatisticTotals
    {
        [MatchDataStatistic(MatchDataStatisticType.RedCards, MatchDataTotalsType.Aggregate)]
        public int RedCards { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.YellowCards, MatchDataTotalsType.Average)]
        public double RedCardsAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.YellowCards, MatchDataTotalsType.Aggregate)]
        public int YellowCards { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.YellowCards, MatchDataTotalsType.Average)]
        public double YellowCardsAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Fouls, MatchDataTotalsType.Aggregate)]
        public int Fouls { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Fouls, MatchDataTotalsType.Average)]
        public double FoulsAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.FoulsSuffered, MatchDataTotalsType.Aggregate)]
        public int FoulsSuffered { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.FoulsSuffered, MatchDataTotalsType.Average)]
        public double FoulsSufferedAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.SlidingTackles, MatchDataTotalsType.Average)]
        public double SlidingTacklesAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.SlidingTacklesCompleted, MatchDataTotalsType.Average)]
        public double SlidingTacklesCompletedAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.GoalsConceded, MatchDataTotalsType.Aggregate)]
        public int GoalsConceded { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.GoalsConceded, MatchDataTotalsType.Average)]
        public double GoalsConcededAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Goals, MatchDataTotalsType.Aggregate)]
        public int Goals { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Goals, MatchDataTotalsType.Average)]
        public double GoalsAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.OwnGoals, MatchDataTotalsType.Aggregate)]
        public int OwnGoals { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.OwnGoals, MatchDataTotalsType.Average)]
        public double OwnGoalsAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Assists, MatchDataTotalsType.Aggregate)]
        public int Assists { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Assists, MatchDataTotalsType.Average)]
        public double AssistsAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Shots, MatchDataTotalsType.Aggregate)]
        public int Shots { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.GoalsConceded, MatchDataTotalsType.Average)]
        public double ShotsAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.ShotsOnGoal, MatchDataTotalsType.Aggregate)]
        public int ShotsOnGoal { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.ShotsOnGoal, MatchDataTotalsType.Average)]
        public double ShotsOnGoalAverage { get; set; }

        public double ShotAccuracyPercentage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Passes, MatchDataTotalsType.Aggregate)]
        public int Passes { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Passes, MatchDataTotalsType.Average)]
        public double PassesAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.PassesCompleted, MatchDataTotalsType.Average)]
        public int PassesCompleted { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.PassesCompleted, MatchDataTotalsType.Aggregate)]
        public double PassesCompletedAverage { get; set; }

        public double PassCompletionPercentageAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Interceptions, MatchDataTotalsType.Average)]
        public int Interceptions { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Interceptions, MatchDataTotalsType.Average)]
        public double InterceptionsAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Offsides, MatchDataTotalsType.Aggregate)]
        public int Offsides { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Offsides, MatchDataTotalsType.Average)]
        public double OffsidesAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.OwnGoals, MatchDataTotalsType.Aggregate)]
        public int FreeKicks { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.OwnGoals, MatchDataTotalsType.Average)]
        public double FreeKicksAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.OwnGoals, MatchDataTotalsType.Average)]
        public int Penalties { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Penalties, MatchDataTotalsType.Average)]
        public double PenaltiesAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Corners, MatchDataTotalsType.Average)]
        public double Corners { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.ThrowIns, MatchDataTotalsType.Average)]
        public double ThrowIns { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.KeeperSaves, MatchDataTotalsType.Aggregate)]
        public int KeeperSaves { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.KeeperSaves, MatchDataTotalsType.Average)]
        public double KeeperSavesAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.KeeperSavesCaught, MatchDataTotalsType.Average)]
        public double KeeperSavesCaughtAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.GoalKicks, MatchDataTotalsType.Average)]
        public double GoalKicksAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Possession, MatchDataTotalsType.Average)]
        public double PossessionAverage { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.DistanceCovered, MatchDataTotalsType.Average)]
        public double DistanceCoveredAverage { get; set; }

        public int Matches { get; set; }

        public int Wins { get; set; }

        public int Draws { get; set; }

        public int Losses { get; set; }

    }
}