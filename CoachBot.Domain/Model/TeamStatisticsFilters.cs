using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class TeamStatisticsFilters
    {
        public StatisticsTimePeriod TimePeriod { get; set; }

        public int? TeamId { get; set; }

        public int? TournamentEditionId { get; set; }

        public int? RegionId { get; set; }

        public bool IncludeInactive { get; set; } = true;

    }
}
