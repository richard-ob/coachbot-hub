using CoachBot.Database;
using CoachBot.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class Match : IUserUpdateableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? TeamHomeId { get; set; }

        [ForeignKey("TeamHomeId")]
        public Team TeamHome { get; set; }

        public int? TeamAwayId { get; set; }

        [ForeignKey("TeamAwayId")]
        public Team TeamAway { get; set; }

        public int? ServerId { get; set; }

        public Server Server { get; set; }

        public int? MatchStatisticsId { get; set; }

        [ForeignKey("MatchStatisticsId")]
        public MatchStatistics MatchStatistics { get; set; }

        public DateTime? KickOff { get; set; }

        public MatchFormat Format { get; set; } = MatchFormat.EightVsEight;

        public MatchType MatchType { get; set; } = MatchType.RankedFriendly;

        public int? MapId { get; set; }

        public Map Map { get; set; }

        public int? PlayerOfTheMatchId { get; set; }

        public Player PlayerOfTheMatch { get; set; }

        public int? TournamentId { get; set; }

        public Tournament Tournament { get; set; }

        public Matchup Matchup { get; set; }

        public ICollection<PlayerMatchStatistics> PlayerMatchStatistics { get; set; }

        public ICollection<PlayerPositionMatchStatistics> PlayerPositionMatchStatistics { get; set; }

        public ICollection<TeamMatchStatistics> TeamMatchStatistics { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public int? CreatedById { get; set; }

        public Player CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int? UpdatedById { get; set; }

        public Player UpdatedBy { get; set; }
    }
}