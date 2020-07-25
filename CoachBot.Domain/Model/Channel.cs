using CoachBot.Database;
using CoachBot.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class Channel : IUserUpdateableEntity
    {
        [Key]
        public int Id { get; set; }

        public int TeamId { get; set; }

        public Team Team { get; set; }

        public string SubTeamName { get; set; }

        public string SubTeamCode { get; set; }

        public ulong DiscordChannelId { get; set; }

        public string DiscordChannelName { get; set; }

        public Formation Formation { get; set; }

        public bool UseClassicLineup { get; set; }

        public bool IsMixChannel => ChannelType == ChannelType.PublicMix || ChannelType == ChannelType.PrivateMix;

        public ChannelType ChannelType { get; set; } = ChannelType.Team;

        public MatchFormat Format => ChannelPositions != null ? (MatchFormat)ChannelPositions.Count : MatchFormat.Unknown;

        public string SearchTeamCode => Team != null ? Team.TeamCode + SubTeamCode : null;

        public List<int> SearchIgnoreList { get; set; } = new List<int>();

        public bool DisableSearchNotifications { get; set; }

        public bool DuplicityProtection { get; set; }

        public bool Inactive { get; set; }

        public ICollection<ChannelPosition> ChannelPositions { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public int? CreatedById { get; set; }

        public Player CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int? UpdatedById { get; set; }

        public Player UpdatedBy { get; set; }
    }
}