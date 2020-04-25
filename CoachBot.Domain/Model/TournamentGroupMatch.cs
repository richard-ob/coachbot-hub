using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class TournamentGroupMatch
    {
        [Key]
        public int Id { get; set; }

        public int MatchId { get; set; }

        public Match Match { get; set; }

        public int TournamentGroupId { get; set; }

        public TournamentGroup TournamentGroup { get; set; }

        public int TournamentPhaseId { get; set; }

        public TournamentPhase TournamentPhase { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

    }
}
