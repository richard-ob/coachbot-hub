using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Models;
using Microsoft.AspNetCore.Authorization;
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
        private readonly TeamService _teamService;
        private readonly PlayerService _playerService;

        public PlayerTeamController(PlayerTeamService playerTeamService, TeamService teamService, PlayerService playerService)
        {
            _playerTeamService = playerTeamService;
            _teamService = teamService;
            _playerService = playerService;
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(PlayerTeam playerTeam)
        {
            if (!_teamService.IsTeamCaptain(playerTeam.TeamId, User.GetSteamId()) && !_teamService.IsViceCaptain(playerTeam.TeamId, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Forbid();
            }

            _playerTeamService.AddPlayerToTeam(playerTeam.TeamId, playerTeam.PlayerId, playerTeam.TeamRole);

            return Ok();
        }

        [Authorize]
        [HttpPut]
        public IActionResult Update(PlayerTeam playerTeam)
        {
            var hasCaptainPermissions = _teamService.IsTeamCaptain(playerTeam.TeamId, User.GetSteamId()) || _teamService.IsViceCaptain(playerTeam.TeamId, User.GetSteamId());
            var currentPlayer = _playerService.GetPlayerBySteamId(User.GetSteamId());

            if (hasCaptainPermissions || currentPlayer.Id == playerTeam.PlayerId || _playerService.IsOwner(User.GetSteamId()))
            {
                _playerTeamService.Update(playerTeam, currentPlayer, hasCaptainPermissions);

                return Ok();
            }

            return Forbid();
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