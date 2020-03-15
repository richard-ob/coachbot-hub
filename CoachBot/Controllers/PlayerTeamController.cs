using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/player-team")]
    [ApiController]
    public class PlayerTeamController : Controller
    {
        private readonly PlayerTeamService _playerTeamService;

        public PlayerTeamController(PlayerTeamService playerTeamService)
        {
            _playerTeamService = playerTeamService;
        }

        [HttpPost]
        public IActionResult Create(PlayerTeam playerTeam)
        {
            _playerTeamService.Create(playerTeam);

            return Ok();
        }

        [HttpPost]
        public IActionResult Update(PlayerTeam playerTeam)
        {
            _playerTeamService.Update(playerTeam);

            return Ok();
        }

        [HttpGet("player/{id}")]
        public List<PlayerTeam> GetForPlayer(int id)
        {
            return _playerTeamService.GetForPlayer(id);
        }

        [HttpGet("team/{id}")]
        public List<PlayerTeam> GetForTeam(int id)
        {
            return _playerTeamService.GetForTeam(id);
        }
    }
}
