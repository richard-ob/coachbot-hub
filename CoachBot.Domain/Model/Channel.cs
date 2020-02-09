using CoachBot.Domain.Extensions;
using CoachBot.Model;

using Discord;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public string DisplayName => BadgeEmote ?? Name;

        public string Color { get; set; } // Rename to ColorHex

        public Color SystemColor
        {
            get
            {
                if (string.IsNullOrEmpty(Color) && Color[0] == '#')
                {
                    return new Color(ColorExtensions.FromHex(Color).R, ColorExtensions.FromHex(Color).G, ColorExtensions.FromHex(Color).B);
                }
                else
                {
                    return new Color(0x2463b0);
                }
            }
        }

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

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

    }
}
