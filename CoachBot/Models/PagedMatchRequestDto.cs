using CoachBot.Domain.Model;

namespace CoachBot.Models
{
    public class PagedMatchRequestDto: PagedRequest
    {
        public int RegionId { get; set; }

        public int? PlayerId { get; set; } 

        public int? TeamId { get; set; }

        public int? TournamentEditionId { get; set; }

        public bool IncludeUpcoming { get; set; } = false;

        public bool IncludePast { get; set; } = false;

    }
}
