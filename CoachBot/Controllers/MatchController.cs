using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class MatchController : Controller
    {
        private readonly StatisticsService _statisticsService;

        public MatchController(StatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("channel/{id}")]
        public IEnumerable<Match> GetChannelMatches(ulong id)
        {
            return _statisticsService.MatchHistory.Where(m => m.ChannelId == id).OrderByDescending(x => x.MatchDate);
        }

        [HttpGet("player/{id}")]
        public IEnumerable<Match> GetPlayerMatches(ulong id)
        { 
            return _statisticsService.MatchHistory.Where(m => m.Players.Any(p => p.DiscordUserId == id)).OrderByDescending(x => x.MatchDate);
        }
    }
}
