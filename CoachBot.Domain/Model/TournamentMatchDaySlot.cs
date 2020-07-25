using CoachBot.Database;
using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class TournamentMatchDaySlot: IUserUpdateableEntity
    {
        [Key]
        public int Id { get; set; }

        public int TournamentId { get; set; }

        public Tournament Tournament { get; set; }

        public TournamentMatchDay MatchDay { get; set; }

        public DateTime MatchTime { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public int? CreatedById { get; set; }

        public Player CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int? UpdatedById { get; set; }

        public Player UpdatedBy { get; set; }
    }
}