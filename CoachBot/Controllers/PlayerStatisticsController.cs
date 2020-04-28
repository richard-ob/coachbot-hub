using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Services;
using CoachBot.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/player-statistics")]
    [ApiController]
    public class PlayerStatisticsController : Controller
    {       
        private readonly MatchStatisticsService _matchStatisticsService;

        public PlayerStatisticsController(MatchStatisticsService matchStatisticsService)
        {
            _matchStatisticsService = matchStatisticsService;
        }

        [HttpPost]
        public PagedResult<PlayerStatisticTotals> PagedPlayerTotalStatistics([FromBody]PagedPlayerStatisticsRequestDto pagedRequest)
        {
            return _matchStatisticsService.GetPlayerStatistics(pagedRequest.Page, pagedRequest.PageSize, pagedRequest.SortOrderFull, pagedRequest.Filters);
        }

        [HttpPost("matches")]
        public PagedResult<PlayerPositionMatchStatistics> PagedPlayerMatchStatistics([FromBody]PagedPlayerStatisticsRequestDto pagedRequest)
        {
            return _matchStatisticsService.GetPlayerPositionMatchStatistics(pagedRequest.Page, pagedRequest.PageSize, pagedRequest.SortOrderFull, pagedRequest.Filters);
        }

        [HttpGet("appearance-totals/{playerId}")]
        public List<PlayerAppearanceTotals> GetPlayerAppearanceTotals(int playerId)
        {
            return _matchStatisticsService.GetPlayerAppearanceTotals(playerId);
        }

    }
}
