using CoachBot.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class FantasyTeam
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsFinalised { get; set; } = false;

        public int? PlayerId { get; set; }

        public Player Player { get; set; }

        public int? TournamentEditionId { get; set; }

        public TournamentEdition TournamentEdition { get; set; }

        public ICollection<FantasyTeamSelection> FantasyTeamSelections { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

    }
}
