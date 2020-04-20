using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Services;
using CoachBot.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerStatisticsController : Controller
    {       
        private readonly MatchStatisticsService _matchStatisticsService;

        public PlayerStatisticsController(MatchStatisticsService matchStatisticsService)
        {
            _matchStatisticsService = matchStatisticsService;
        }

        [HttpPost]
        public PagedResult<PlayerStatisticTotals> PagedPlayerStatistics([FromBody]PagedPlayerStatisticsRequestDto pagedRequest)
        {
            return _matchStatisticsService.GetPlayerStatistics(pagedRequest.Page, pagedRequest.PageSize, pagedRequest.SortOrderFull, pagedRequest.Filters);
        }

    }
}
