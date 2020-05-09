using CoachBot.Model;
using System;

namespace CoachBot.Domain.Model
{
    public class TournamentEditionStaff
    {
        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public int TournamentEditionId { get; set; }

        public TournamentEdition TournamentEdition { get; set; }

        public TournamentStaffRole Role { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
