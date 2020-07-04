using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class PlayerMatchStatistics : MatchStatisticsBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SecondsPlayed { get; set; }

        public string Nickname { get; set; }

        public bool Substitute { get; set; }

        public int MatchId { get; set; }

        public Match Match { get; set; }

        public int? TeamId { get; set; }

        public Team Team { get; set; }

        public int ChannelId { get; set; }

        public Channel Channel { get; set; }

        public int PlayerId { get; set; }

        public Player Player { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}