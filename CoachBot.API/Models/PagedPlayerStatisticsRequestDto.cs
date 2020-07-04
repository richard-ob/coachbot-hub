using CoachBot.Domain.Model;

namespace CoachBot.Models
{
    public class PagedPlayerStatisticsRequestDto : PagedRequest
    {
        public PlayerStatisticFilters Filters { get; set; }
    }
}