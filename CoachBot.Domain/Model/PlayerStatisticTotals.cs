namespace CoachBot.Domain.Model
{
    public class PlayerStatisticTotals : StatisticTotals
    {
        public int PlayerId { get; set; }

        public string Name { get; set; }

        public ulong? SteamID { get; set; }

        public double Rating { get; set; }
    }
}