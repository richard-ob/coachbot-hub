using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.LegacyImporter.Model
{
    public class LegacyMatch
    {
        public ulong ChannelId { get; set; }

        public string ChannelName { get; set; }

        public List<LegacyPlayer> Players { get; set; }

        public string Team1Name { get; set; }

        public string Team2Name { get; set; }

        public DateTime MatchDate { get; set; }

    }
}
