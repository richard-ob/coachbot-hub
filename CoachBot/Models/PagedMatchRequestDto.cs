using CoachBot.Domain.Model;

namespace CoachBot.Models
{
    public class PagedMatchRequestDto: PagedRequest
    {
        public MatchFilters Filters { get; set; }
    }
}
