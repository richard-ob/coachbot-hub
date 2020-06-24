using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class PlayerPerformanceSnapshot
    {
        public int PlayerId { get; set; }

        public int Week { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public int AverageGoals { get; set; }

        public int AverageAssists { get; set; }

        public int AverageGoalsConceded { get; set; }

        public int CleanSheets { get; set; }

        public int Appearances { get; set; }

    }
}
