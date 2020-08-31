using CoachBot.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class PlayerOfTheMatchStatistics
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public PositionGroup PositionGroup { get; set; }

        public int Goals { get; set; }

        public int Assists { get; set; }

        public int GoalsConceded { get; set; }

        public int Interceptions { get; set; }

        public int PassCompletion { get; set; }

        public int KeeperSaves { get; set; }

    }
}
