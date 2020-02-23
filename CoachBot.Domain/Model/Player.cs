using CoachBot.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Model
{
    public class Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public ulong? DiscordUserId { get; set; }

        public string DiscordUserMention
        {
            get
            {
                if (DiscordUserId == null) return null;

                return $"<@!{DiscordUserId}>";
            }
        }

        public string SteamID { get; set; }

        public bool DisableDMNotifications { get; set; }

        public ICollection<PlayerLineupPosition> PlayerTeamPositions { get; set; }

        public ICollection<PlayerLineupSubstitute> PlayerSubstitutes { get; set; }

        public string DisplayName
        {
            get { return DiscordUserMention ?? Name ?? "Unknown"; }
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

    }
}
