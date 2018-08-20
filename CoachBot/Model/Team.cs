using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoachBot.Model
{
    public class Team
    {
        [Key]
        public Guid Id { get; set; }

        public ulong? ChannelId { get; set; }

        public string Name { get; set; }

        public string KitEmote { get; set; }

        public string BadgeEmote { get; set; }

        public string Color { get; set; }

        public bool IsMix { get; set; }

        public List<Player> Substitutes { get; set; }

        public List<Player> Players { get; set; }

    }
}
