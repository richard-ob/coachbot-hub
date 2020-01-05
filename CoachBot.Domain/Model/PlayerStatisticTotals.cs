using CoachBot.Model;

namespace CoachBot.Domain.Model
{
    public class PlayerStatisticTotals
    {
        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public StatisticTotals StatisticTotals { get; set; } = new StatisticTotals();

    }
}
