﻿namespace CoachBot.Domain.Model
{
    public class FantasyTeamRank
    {
        public int Rank { get; set; }

        public int FantasyTeamId { get; set; }

        public string FantasyTeamName { get; set; }

        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public int Points { get; set; }
    }
}