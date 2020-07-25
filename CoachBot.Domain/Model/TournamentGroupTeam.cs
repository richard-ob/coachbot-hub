using CoachBot.Database;
using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class TournamentGroupTeam: IUserUpdateableEntity
    {
        [Key]
        public int Id { get; set; }

        public int TeamId { get; set; }

        public Team Team { get; set; }

        public int TournamentGroupId { get; set; }

        public TournamentGroup TournamentGroup { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public int? CreatedById { get; set; }

        public Player CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int? UpdatedById { get; set; }

        public Player UpdatedBy { get; set; }
    }
}