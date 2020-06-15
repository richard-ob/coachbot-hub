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

        public int TeamId { get; set; }

        public Team Team { get; set; }

        public string SubTeamName { get; set; }

        public ulong DiscordChannelId { get; set; }

        public string DiscordChannelName { get; set; }

        public Formation Formation { get; set; }

        public bool UseClassicLineup { get; set; }

        public bool IsMixChannel { get; set; }

        public List<int> SearchIgnoreList { get; set; } = new List<int>();

        public bool DisableSearchNotifications { get; set; }

        public bool DuplicityProtection { get; set; }

        public bool Inactive { get; set; }

        public ICollection<ChannelPosition> ChannelPositions { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

    }
}
