using CoachBot.Model;

namespace CoachBot.Domain.Model
{
    public class TeamStatisticsFilters
    {
        public StatisticsTimePeriod TimePeriod { get; set; }

        public int? TeamId { get; set; }

        public MatchTeamType? MatchTeamType { get; set; }

        public int? TournamentId { get; set; }

        public int? RegionId { get; set; }

        public bool IncludeInactive { get; set; } = true;

        public TeamType? TeamType { get; set; }
    }
}