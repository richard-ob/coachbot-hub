namespace CoachBot.Domain.Model
{
    public class FantasyPlayerRank
    {
        public int Rank { get; set; }

        public int Points { get; set; }

        public double Rating { get; set; }

        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public int Goals { get; set; }

        public int Assists { get; set; }

        public int CleanSheets { get; set; }

        public int OwnGoals { get; set; }

        public int YellowCards { get; set; }

        public int RedCards { get; set; }

        public int KeeperSaves { get; set; }

        public int GoalsConceded { get; set; }

        public int SecondsPlayed { get; set; }
    }
}