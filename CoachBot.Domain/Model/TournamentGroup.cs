using CoachBot.Database;
using CoachBot.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class TournamentGroup : IUserUpdateableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int? TournamentStageId { get; set; }

        public TournamentStage TournamentStage { get; set; }

        public ICollection<TournamentGroupMatch> TournamentGroupMatches { get; set; }

        public ICollection<TournamentGroupTeam> TournamentGroupTeams { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public int? CreatedById { get; set; }

        public Player CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int? UpdatedById { get; set; }

        public Player UpdatedBy { get; set; }

    }
}