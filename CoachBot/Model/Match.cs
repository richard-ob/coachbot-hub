using System;
using System.Collections.Generic;

namespace CoachBot.Model
{
    public class Match
    {
        public ulong ChannelId { get; set; }

        public List<Player> Players { get; set; }

        public string Team1Name { get; set; }

        public string Team2Name { get; set; }

        public DateTime MatchDate { get; set; }

    }
}
