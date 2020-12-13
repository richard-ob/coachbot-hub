using CoachBot.Model;
using System;

namespace CoachBot.Domain.Model
{
    public class TeamStatisticsFilters
    {
        public StatisticsTimePeriod TimePeriod { get; set; }

        public int? TeamId { get; set; }

        public int? OppositionTeamId { get; set; }

        public bool HeadToHead { get; set; } = false;

        public MatchTeamType? MatchTeamType { get; set; }

        public MatchOutcomeType? MatchOutcome { get; set; }

        public MatchFormat? MatchFormat { get; set; }

        public MatchType? MatchType { get; set; }

        public int? TournamentId { get; set; }

        public int? TournamentGroupId { get; set; }

        public int? RegionId { get; set; }

        public bool IncludeInactive { get; set; } = true;

        public TeamType? TeamType { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }
}