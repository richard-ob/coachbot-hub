using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Services;
using CoachBot.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TeamStatisticsController : Controller
    {
        private readonly MatchStatisticsService _matchStatisticsService;

        public TeamStatisticsController(MatchStatisticsService matchStatisticsService)
        {
            _matchStatisticsService = matchStatisticsService;
        }

        [HttpPost]
        public PagedResult<TeamStatisticTotals> PagedTeamStatistics([FromBody]PagedTeamStatisticsRequestDto pagedRequest)
        {
            return _matchStatisticsService.GetTeamStatistics(pagedRequest.Page, pagedRequest.PageSize, pagedRequest.SortOrderFull, pagedRequest.TimePeriod);
        }

    }
}
