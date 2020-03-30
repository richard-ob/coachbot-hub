using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Models;
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

        [HttpPost("player")]
        public List<PlayerTeam> GetForPlayer([FromBody]PlayerTeamRequestDto playerTeamRequestDto)
        {
            return _playerTeamService.GetForPlayer(playerTeamRequestDto.Id, playerTeamRequestDto.IncludeInactive);
        }

        [HttpPost("team")]
        public List<PlayerTeam> GetForTeam([FromBody]PlayerTeamRequestDto playerTeamRequestDto)
        {
            return _playerTeamService.GetForTeam(playerTeamRequestDto.Id, playerTeamRequestDto.IncludeInactive);
        }
    }
}
