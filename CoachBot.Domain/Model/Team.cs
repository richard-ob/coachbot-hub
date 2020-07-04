using CoachBot.Domain.Extensions;
using CoachBot.Model;
using Discord;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string TeamCode { get; set; }

        public string KitEmote { get; set; }

        public string BadgeEmote { get; set; }

        public int? BadgeImageId { get; set; }

        public AssetImage BadgeImage { get; set; }

        public string DisplayName => BadgeEmote ?? Name;

        public TeamType TeamType { get; set; }

        public int? RegionId { get; set; }

        public Region Region { get; set; }

        public int? GuildId { get; set; }

        public Guild Guild { get; set; }

        public string Color { get; set; } // Rename to ColorHex

        public Color SystemColor
        {
            get
            {
                if (!string.IsNullOrEmpty(Color) && Color[0] == '#')
                {
                    return new Color(ColorExtensions.FromHex(Color).R, ColorExtensions.FromHex(Color).G, ColorExtensions.FromHex(Color).B);
                }
                else
                {
                    return new Color(0x2463b0);
                }
            }
        }

        public DateTime? FoundedDate { get; set; }

        public List<MatchOutcomeType> Form { get; set; } = new List<MatchOutcomeType>();

        public bool Inactive { get; set; } = false;

        public ICollection<Channel> Channels { get; set; }

        public ICollection<PlayerTeam> Players { get; set; }

        public ICollection<TeamMatchStatistics> TeamMatchStatistics { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}