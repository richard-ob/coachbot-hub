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
    public class MatchController : Controller
    {
        private readonly MatchService _matchService;

        public MatchController(MatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet("{id}")]
        public Match Get(int id)
        {
            return _matchService.GetMatch(id);
        }

        [HttpPost]
        public PagedResult<Match> PagedMatchList([FromBody]PagedMatchRequestDto pagedRequest)
        {
            return _matchService.GetMatches(
                pagedRequest.RegionId,
                pagedRequest.Page,
                pagedRequest.PageSize,
                pagedRequest.SortOrderFull,
                pagedRequest.PlayerId,
                pagedRequest.TeamId,
                pagedRequest.UpcomingOnly
            );
        }

        [HttpPut("{id}")]
        public void UpdateMatch([FromBody]Match match)
        {
            _matchService.UpdateMatch(match);
        }
    }
}
