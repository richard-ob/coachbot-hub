
using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamActivityController : Controller
    {
        private readonly TeamService _teamService;

        public TeamActivityController(TeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet("{channelId}")]
        public Team Get(int channelId)
        {
            return _teamService.GetTeam(channelId);
        }
    }
}
