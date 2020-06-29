using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.LegacyImporter.Model
{
    public class LegacyTeam
    {
        public Guid Id { get; set; }

        public ulong? ChannelId { get; set; }

        public string Name { get; set; }

        public string KitEmote { get; set; }

        public string BadgeEmote { get; set; }

        public string Color { get; set; }

        public bool IsMix { get; set; }

        public List<LegacyPlayer> Substitutes { get; set; }

        public List<LegacyPlayer> Players { get; set; }

    }
}
