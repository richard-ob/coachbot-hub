using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class TournamentPhase
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int TournamentStageId { get; set; }

        public TournamentStage TournamentStage { get; set; }

        public ICollection<TournamentGroupMatch> TournamentGroupMatches { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

    }
}
