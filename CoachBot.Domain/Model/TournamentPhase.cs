using CoachBot.Database;
using CoachBot.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class TournamentPhase: ISystemEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int TournamentStageId { get; set; }

        [JsonIgnore]
        public TournamentStage TournamentStage { get; set; }

        public ICollection<TournamentGroupMatch> TournamentGroupMatches { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}