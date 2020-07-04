using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class PlayerLineupPosition
    {
        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public int LineupId { get; set; }

        public Lineup Lineup { get; set; }

        public int PositionId { get; set; }

        public Position Position { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}