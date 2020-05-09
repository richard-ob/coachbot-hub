using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace CoachBot.Domain.Model
{
    public class TournamentEditionStaff
    {
        [Key]
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public int TournamentEditionId { get; set; }

        public TournamentEdition TournamentEdition { get; set; }

        public TournamentStaffRole Role { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
