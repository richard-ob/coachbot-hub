
using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : Controller
    {
        private readonly TeamService _teamService;

        public TeamController(TeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet("{id}")]
        public Team Get(int id)
        {
            return _teamService.GetTeam(id);
        }

        [HttpPost]
        public IActionResult Create(Team team)
        {
            _teamService.CreateTeam(team, User.GetDiscordUserId());

            return Ok();
        }

        [HttpPut]
        public IActionResult Update(Team team)
        {
            if (!_teamService.IsTeamCaptain(team.Id, User.GetDiscordUserId()) && !_teamService.IsViceCaptain(team.Id, User.GetDiscordUserId()))
            {
                return Forbid();
            }

            _teamService.UpdateTeam(team);

            return Ok();
        }
    }
}
