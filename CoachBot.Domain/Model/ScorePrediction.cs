using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class ScorePrediction
    {
        [Key]
        public int Id { get; set; }

        public int HomeGoals { get; set; }

        public int AwayGoals { get; set; }

        public int MatchId { get; set; }

        public Match Match { get; set; }

        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public int TournamentPhaseId { get; set; }

        public TournamentPhase TournamentPhase { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}
