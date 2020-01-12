using CoachBot.Domain.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class StatisticTotals
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.RedCards, MatchDataTotalsType.Aggregate)]
        public int RedCards { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.YellowCards, MatchDataTotalsType.Aggregate)]
        public int YellowCards { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Fouls, MatchDataTotalsType.Average)]
        public int Fouls { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.FoulsSuffered, MatchDataTotalsType.Average)]
        public int FoulsSuffered { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.SlidingTackles, MatchDataTotalsType.Average)]
        public int SlidingTackles { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.SlidingTacklesCompleted, MatchDataTotalsType.Average)]
        public int SlidingTacklesCompleted { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.GoalsConceded, MatchDataTotalsType.Aggregate)]
        public int GoalsConceded { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Shots, MatchDataTotalsType.Average)]
        public int Shots { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.ShotsOnGoal, MatchDataTotalsType.Average)]
        public int ShotsOnGoal { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.PassesCompleted, MatchDataTotalsType.Average)]
        public int PassesCompleted { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Interceptions, MatchDataTotalsType.Average)]
        public int Interceptions { get; set; }
        
        [MatchDataStatistic(MatchDataStatisticType.Offsides, MatchDataTotalsType.Average)]
        public int Offsides { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Goals, MatchDataTotalsType.Aggregate)]
        public int Goals { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.OwnGoals, MatchDataTotalsType.Aggregate)]
        public int OwnGoals { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Assists, MatchDataTotalsType.Aggregate)]
        public int Assists { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Passes, MatchDataTotalsType.Average)]
        public int Passes { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.OwnGoals, MatchDataTotalsType.Average)]
        public int FreeKicks { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.OwnGoals, MatchDataTotalsType.Average)]
        public int Penalties { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Corners, MatchDataTotalsType.Average)]
        public int Corners { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.ThrowIns, MatchDataTotalsType.Average)]
        public int ThrowIns { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.KeeperSaves, MatchDataTotalsType.Average)]
        public int KeeperSaves { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.GoalKicks, MatchDataTotalsType.Average)]
        public int GoalKicks { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.Possession, MatchDataTotalsType.Average)]
        public int Possession { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.DistanceCovered, MatchDataTotalsType.Average)]
        public int DistanceCovered { get; set; }

        [MatchDataStatistic(MatchDataStatisticType.KeeperSavesCaught, MatchDataTotalsType.Average)]
        public int KeeperSavesCaught { get; set; }

        public int Matches { get; set; }

        public int Wins { get; set; }

        public int Draws { get; set; }

        public int Losses { get; set; }
    }
}