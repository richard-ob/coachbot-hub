using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class TournamentEditionMatchDaySlot
    {
        [Key]
        public int Id { get; set; }

        public int TournamentEditionId { get; set; }

        public TournamentEdition TournamentEdition { get; set; }

        public TournamentMatchDay MatchDay { get; set; }

        public DateTime MatchTime { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

    }
}
