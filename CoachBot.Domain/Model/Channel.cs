using CoachBot.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoachBot.Domain.Model
{
    public class Channel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ulong DiscordChannelId { get; set; }

        public string DiscordChannelName { get; set; }

        public string TeamCode { get; set; }

        public Formation Formation { get; set; }

        public string KitEmote { get; set; }

        public string BadgeEmote { get; set; }

        public string Color { get; set; }

        public bool UseClassicLineup { get; set; }

        public bool IsMixChannel { get; set; }

        public bool DisableSearchNotifications { get; set; }

        public bool DuplicityProtection { get; set; }

        public bool Inactive { get; set; }

        public int GuildId { get; set; }

        public Guild Guild { get; set; }

        public int? RegionId { get; set; }

        public Region Region { get; set; }

        public ICollection<ChannelPosition> ChannelPositions { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

    }
}
