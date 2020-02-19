using CoachBot.Domain.Model;

namespace CoachBot.Models
{
    public class PagedPlayerStatisticsRequestDto : PagedRequest
    {
        public StatisticsTimePeriod TimePeriod { get; set; }
    }
}
