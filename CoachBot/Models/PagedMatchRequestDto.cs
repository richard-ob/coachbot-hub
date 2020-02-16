using CoachBot.Domain.Model;

namespace CoachBot.Models
{
    public class PagedMatchRequestDto: PagedRequest
    {
        public int RegionId { get; set; }
    }
}
