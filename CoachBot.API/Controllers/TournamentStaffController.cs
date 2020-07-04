using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using Microsoft.AspNetCore.Mvc;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/tournament-staff")]
    [ApiController]
    public class TournamentStaffController : Controller
    {
        private readonly TournamentService _tournamentService;
        private readonly PlayerService _playerService;

        public TournamentStaffController(TournamentService tournamentService, PlayerService playerService)
        {
            _tournamentService = tournamentService;
            _playerService = playerService;
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpDelete("{id}")]
        public IActionResult DeleteTournamentStaffMember(int id)
        {
            var tournamentStaff = _tournamentService.GetTournamentStaffMember(id);
            if (!_tournamentService.IsTournamentOrganiser((int)tournamentStaff.TournamentId, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Unauthorized();
            }

            _tournamentService.DeleteTournamentStaff(id);

            return Ok();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost]
        public IActionResult CreateTournamentStaffMember(TournamentStaff tournamentStaff)
        {
            if (!_tournamentService.IsTournamentOrganiser((int)tournamentStaff.TournamentId, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Unauthorized();
            }

            _tournamentService.CreateTournamentStaff(tournamentStaff);

            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTournamentStaffMember(TournamentStaff tournamentStaff)
        {
            if (!_tournamentService.IsTournamentOrganiser((int)tournamentStaff.TournamentId, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Unauthorized();
            }

            _tournamentService.UpdateTournamentStaff(tournamentStaff);

            return Ok();
        }

    }
}
