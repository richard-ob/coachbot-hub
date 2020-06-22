using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class FantasyTeamSummary
    {
        public int FantasyTeamId { get; set; }

        public string FantasyTeamName { get; set; }

        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public string TournamentName { get; set; }

        public int TournamentId { get; set; }

        public FantasyTeamStatus FantasyTeamStatus { get; set; }

        public bool IsComplete { get; set; }

    }
}
