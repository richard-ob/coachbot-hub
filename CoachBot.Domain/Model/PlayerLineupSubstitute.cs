using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class PlayerLineupSubstitute
    {
        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public int LineupId { get; set; }

        public Lineup Lineup { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}