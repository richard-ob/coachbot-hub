using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
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
        public IEnumerable<Match> PagedMatchList([FromBody]PagedMatchRequestDto pagedRequest)
        {
            return _matchService.GetMatches(pagedRequest.RegionId, pagedRequest.PageSize, pagedRequest.Offset);
        }
    }
}
