using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/tournament-groups")]
    [ApiController]
    public class TournamentGroupController : Controller
    {
        private readonly TournamentService _tournamentService;
        private readonly PlayerService _playerService;

        public TournamentGroupController(TournamentService tournamentService, PlayerService playerService)
        {
            _tournamentService = tournamentService;
            _playerService = playerService;
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpDelete("{id}")]
        public IActionResult DeleteTournamentGroup(int id)
        {
            var tournamentGroup = _tournamentService.GetTournamentGroup(id);
            if (!_tournamentService.IsTournamentOrganiser(tournamentGroup.TournamentStage.TournamentId, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Unauthorized();
            }

            _tournamentService.DeleteTournamentGroup(id);

            return Ok();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost]
        public IActionResult CreateTournamentGroup(TournamentGroup tournamentGroup)
        {
            var tournamentStage = _tournamentService.GetTournamentStage((int)tournamentGroup.TournamentStageId);
            if (!_tournamentService.IsTournamentOrganiser(tournamentStage.TournamentId, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Unauthorized();
            }

            _tournamentService.CreateTournamentGroup(tournamentGroup);

            return Ok();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPut("{id}")]
        public IActionResult UpdateTournamentGroup(TournamentGroup tournamentGroup)
        {
            var existingTournamentGroup = _tournamentService.GetTournamentGroup(tournamentGroup.Id);
            if (!_tournamentService.IsTournamentOrganiser(existingTournamentGroup.TournamentStage.TournamentId, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Unauthorized();
            }

            _tournamentService.UpdateTournamentGroup(tournamentGroup);

            return Ok();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost("{id}/teams")]
        public IActionResult AddTournamentGroupTeam(int id, [FromBody]TournamentGroupTeamDto tournamentGroupTeamDto)
        {
            var tournamentGroup = _tournamentService.GetTournamentGroup(id);
            if (!_tournamentService.IsTournamentOrganiser(tournamentGroup.TournamentStage.TournamentId, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Unauthorized();
            }

            _tournamentService.AddTournamentTeam(tournamentGroupTeamDto.TeamId, tournamentGroupTeamDto.TournamentGroupId);
            return Ok();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpDelete("{id}/teams/{teamId}")]
        public IActionResult RemoveTournamentGroupTeam(int id, int teamId)
        {
            var tournamentGroup = _tournamentService.GetTournamentGroup(id);
            if (!_tournamentService.IsTournamentOrganiser(tournamentGroup.TournamentStage.TournamentId, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Unauthorized();
            }

            _tournamentService.RemoveTournamentTeam(teamId, id);

            return Ok();
        }

        [HttpGet("{tournamentGroupId}/standings")]
        public List<TournamentGroupStanding> GetTournamentStandings(int tournamentGroupId)
        {
            return _tournamentService.GetTournamentGroupStandings(tournamentGroupId);
        }
    }
}