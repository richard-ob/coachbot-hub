using CoachBot.Database;
using CoachBot.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class FantasyTeam : IUserUpdateableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsComplete { get; set; } = false;

        public int? PlayerId { get; set; }

        public Player Player { get; set; }

        public int? TournamentId { get; set; }

        public Tournament Tournament { get; set; }

        public ICollection<FantasyTeamSelection> FantasyTeamSelections { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public int? CreatedById { get; set; }

        public Player CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int? UpdatedById { get; set; }

        public Player UpdatedBy { get; set; }
    }
}