using CoachBot.Domain.Model;

namespace CoachBot.Models
{
    public class PagedTeamStatisticsRequestDto : PagedRequest
    {
        public TeamStatisticsFilters Filters { get; set; }

    }
}
