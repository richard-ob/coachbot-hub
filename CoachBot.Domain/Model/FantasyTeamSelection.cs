using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class FantasyTeamSelection
    {
        [Key]
        public int Id { get; set; }

        public bool IsFlex { get; set; }

        public int? FantasyPlayerId { get; set; }

        public FantasyPlayer FantasyPlayer { get; set; }

        public int? FantasyTeamId { get; set; }

        public FantasyTeam FantasyTeam { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}