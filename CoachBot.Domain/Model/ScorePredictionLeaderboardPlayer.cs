using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class ScorePredictionLeaderboardPlayer
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public int Points { get; set; }

    }
}
