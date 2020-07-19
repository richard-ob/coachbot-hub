using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Services;
using CoachBot.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
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
            return _matchStatisticsService.GetTeamStatistics(pagedRequest.Page, pagedRequest.PageSize, pagedRequest.SortOrderFull, pagedRequest.Filters);
        }

        [HttpPost("matches")]
        public PagedResult<TeamMatchStatistics> PagedTeamMatchStatistics([FromBody]PagedTeamStatisticsRequestDto pagedRequest)
        {
            return _matchStatisticsService.GeTeamMatchStatistics(pagedRequest.Page, pagedRequest.PageSize, pagedRequest.SortOrderFull, pagedRequest.Filters);
        }

        [HttpGet("match-totals/{teamId}")]
        public List<MatchDayTotals> GetPlayerAppearanceTotals(int teamId)
        {
            return _matchStatisticsService.GetTeamMatchDayTotals(teamId);
        }

        [HttpGet("performance/monthly/{teamId}")]
        public List<TeamPerformanceSnapshot> GetMonthlyTeamPerformanceSnapshots(int teamId)
        {
            return _matchStatisticsService.GetMonthlyTeamPerformance(teamId);
        }

        [HttpGet("performance/weekly/{teamId}")]
        public List<TeamPerformanceSnapshot> GetWeeklyTeamPerformanceSnapshots(int teamId)
        {
            return _matchStatisticsService.GetWeeklyTeamPerformance(teamId);
        }

        [HttpGet("performance/daily/{teamId}")]
        public List<TeamPerformanceSnapshot> GetDailyPlayerPerformanceSnapshots(int teamId)
        {
            return _matchStatisticsService.GetDailyTeamPerformance(teamId);
        }

        [HttpGet("performance/continuous/{teamId}")]
        public List<TeamPerformanceSnapshot> GetContinuousPlayerPerformanceSnapshots(int teamId)
        {
            return _matchStatisticsService.GetContinuousTeamPerformance(teamId);
        }
    }
}