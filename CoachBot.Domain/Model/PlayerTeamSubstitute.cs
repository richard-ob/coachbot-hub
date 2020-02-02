using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class PlayerTeamSubstitute
    {
        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public int TeamId { get; set; }

        public Team Team { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}
