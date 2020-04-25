using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class TournamentEditionMatchDay
    {
        [Key]
        public int Id { get; set; }

        public int TournamentEditionId { get; set; }

        public TournamentEdition TournamentEdition { get; set; }

        public TournamentMatchDay MatchDay { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

    }
}
