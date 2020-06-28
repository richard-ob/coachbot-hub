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

        [HttpPost("matches-by-position")]
        public PagedResult<PlayerPositionMatchStatistics> PagedPlayerPositionMatchStatistics([FromBody]PagedPlayerStatisticsRequestDto pagedRequest)
        {
            return _matchStatisticsService.GetPlayerPositionMatchStatistics(pagedRequest.Page, pagedRequest.PageSize, pagedRequest.SortOrderFull, pagedRequest.Filters);
        }

        [HttpPost("matches")]
        public PagedResult<PlayerMatchStatistics> PagedPlayerMatchStatistics([FromBody]PagedPlayerStatisticsRequestDto pagedRequest)
        {
            return _matchStatisticsService.GetPlayerMatchStatistics(pagedRequest.Page, pagedRequest.PageSize, pagedRequest.SortOrderFull, pagedRequest.Filters);
        }

        [HttpGet("appearance-totals/{playerId}")]
        public List<MatchDayTotals> GetPlayerAppearanceTotals(int playerId)
        {
            return _matchStatisticsService.GetPlayerMatchDayTotals(playerId);
        }

        [HttpGet("performance/monthly/{playerId}")]
        public List<PlayerPerformanceSnapshot> GetMonthlyPlayerPerformanceSnapshots(int playerId)
        {
            return _matchStatisticsService.GetMonthlyPlayerPerformance(playerId);
        }

        [HttpGet("performance/weekly/{playerId}")]
        public List<PlayerPerformanceSnapshot> GetWeeklyPlayerPerformanceSnapshots(int playerId)
        {
            return _matchStatisticsService.GetWeeklyPlayerPerformance(playerId);
        }

        [HttpGet("performance/daily/{playerId}")]
        public List<PlayerPerformanceSnapshot> GetDailyPlayerPerformanceSnapshots(int playerId)
        {
            return _matchStatisticsService.GetDailyPlayerPerformance(playerId);
        }

    }
}
