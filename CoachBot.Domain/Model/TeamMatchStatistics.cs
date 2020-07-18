using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class TeamMatchStatistics : MatchStatisticsBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string TeamName { get; set; }

        public int MatchId { get; set; }

        public Match Match { get; set; }

        public int? TeamId { get; set; }

        public Team Team { get; set; }

        public MatchTeamType MatchTeamType { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}