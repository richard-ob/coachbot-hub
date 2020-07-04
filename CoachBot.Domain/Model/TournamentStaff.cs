using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace CoachBot.Domain.Model
{
    public class TournamentStaff
    {
        [Key]
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public int? TournamentId { get; set; }

        public Tournament Tournament { get; set; }

        public TournamentStaffRole Role { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}