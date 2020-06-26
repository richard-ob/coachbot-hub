namespace CoachBot.Domain.Model
{
    public class TeamPerformanceSnapshot
    {
        public int TeamId { get; set; }

        public int? Day { get; set; }

        public int? Week { get; set; }

        public int? Month { get; set; }

        public int Year { get; set; }

        public double AverageGoals { get; set; }

        public double AverageAssists { get; set; }

        public double AverageGoalsConceded { get; set; }

        public int CleanSheets { get; set; }

        public int Matches { get; set; }

        public int Wins { get; set; }

        public int Draws { get; set; }

        public int Losses { get; set; }

    }
}
