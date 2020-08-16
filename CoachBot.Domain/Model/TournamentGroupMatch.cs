using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class TournamentGroupMatch
    {
        [Key]
        public int Id { get; set; }

        public int MatchId { get; set; }

        public Match Match { get; set; }

        public int TournamentGroupId { get; set; }

        [JsonIgnore]
        public TournamentGroup TournamentGroup { get; set; }

        public int TournamentPhaseId { get; set; }

        [JsonIgnore]
        public TournamentPhase TournamentPhase { get; set; }

        public string TeamHomePlaceholder { get; set; }

        public string TeamAwayPlaceholder { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}