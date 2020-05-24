using CoachBot.Domain.Model;

namespace CoachBot.Models
{
    public class PagedTeamStatisticsRequestDto : PagedRequest
    {
        public StatisticsTimePeriod TimePeriod { get; set; }

        public int? TeamId { get; set; }

        public int? TournamentEditionId { get; set; }

        public int? RegionId { get; set; }

    }
}
