using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class TournamentGroupStanding
    {
        public int Position { get; set; }

        public string TeamName { get; set; }

        public int TeamId { get; set; }

        public string TeamCode { get; set; }

        public string BadgeImageUrl { get; set; }

        public int MatchesPlayed { get; set; }

        public int Wins { get; set; }

        public int Draws { get; set; }

        public int Losses { get; set; }

        public int GoalsScored { get; set; }

        public int GoalsConceded { get; set; }

        public int GoalDifference { get; set; }

        public int Points { get; set; }

        public List<MatchOutcomeType> Form { get; set; } = new List<MatchOutcomeType>();

    }
}
