using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class TournamentStage
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int TournamentId { get; set; }

        public Tournament Tournament { get; set; }

        public ICollection<TournamentPhase> TournamentPhases { get; set; }

        public ICollection<TournamentGroup> TournamentGroups { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}