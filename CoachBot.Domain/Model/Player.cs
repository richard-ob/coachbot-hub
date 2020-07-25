using CoachBot.Database;
using CoachBot.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Model
{
    public class Player: IUserUpdateableEntity
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

        public ulong? SteamID { get; set; }

        public bool DisableDMNotifications { get; set; }

        public DateTime? PlayingSince { get; set; }

        public double Rating { get; set; }

        public int? CountryId { get; set; }

        public Country Country { get; set; }

        public PlayerHubRole HubRole { get; set; }

        public ICollection<PlayerPosition> Positions { get; set; }

        public ICollection<PlayerLineupPosition> PlayerLineupPositions { get; set; }

        public ICollection<PlayerLineupSubstitute> PlayerSubstitutes { get; set; }

        public ICollection<PlayerTeam> Teams { get; set; }

        public string DisplayName
        {
            get { return DiscordUserMention ?? Name ?? "Unknown"; }
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public int? CreatedById { get; set; }

        public Player CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int? UpdatedById { get; set; }

        public Player UpdatedBy { get; set; }
    }
}