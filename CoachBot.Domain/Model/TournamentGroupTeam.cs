using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class TournamentGroupTeam
    {
        [Key]
        public int Id { get; set; }

        public int TeamId { get; set; }

        public Team Team { get; set; }

        public int TournamentGroupId { get; set; }

        public TournamentGroup TournamentGroup { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}