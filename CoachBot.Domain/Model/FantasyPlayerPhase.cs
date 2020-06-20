using System;
using System.ComponentModel.DataAnnotations;

namespace CoachBot.Domain.Model
{
    public class FantasyPlayerPhase
    {
        [Key]
        public int Id { get; set; }

        public int Points { get; set; }

        public int FantasyPlayerId { get; set; }

        public FantasyPlayer FantasyPlayer { get; set; }

        public int? TournamentPhaseId { get; set; }

        public TournamentPhase TournamentPhase { get; set; }

        public int? PlayerMatchStatisticsId { get; set; }

        public PlayerMatchStatistics PlayerMatchStatistics { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
