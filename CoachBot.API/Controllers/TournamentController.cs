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
    [Route("api/tournaments")]
    [ApiController]
    public class TournamentController : Controller
    {
        private readonly TournamentService _tournamentService;
        private readonly FantasyService _fantasyService;
        private readonly PlayerService _playerService;

        public TournamentController(TournamentService tournamentService, FantasyService fantasyService, PlayerService playerService)
        {
            _tournamentService = tournamentService;
            _fantasyService = fantasyService;
            _playerService = playerService;
        }

        [HttpGet("current")]
        public List<Tournament> GetCurrentTournaments()
        {
            return _tournamentService.GetTournaments(true);
        }

        [HttpGet("past")]
        public List<Tournament> GetPastTournaments()
        {
            return _tournamentService.GetPastTournaments();
        }

        [HttpGet]
        public List<Tournament> GetTournaments()
        {
            return _tournamentService.GetTournaments();
        }

        [HttpGet("{id}")]
        public Tournament GetTournament(int id)
        {
            return _tournamentService.GetTournament(id);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost]
        public IActionResult CreateTournament(Tournament tournament)
        {
            if (!_tournamentService.IsTournamentOrganiser(tournament.Id, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Unauthorized();
            }

            _tournamentService.CreateTournament(tournament, User.GetSteamId());

            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateTournament(Tournament tournament)
        {
            if (!_tournamentService.IsTournamentOrganiser(tournament.Id, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Unauthorized();
            }

            _tournamentService.UpdateTournament(tournament);

            return Ok();
        }

        [HttpGet("{id}/groups")]
        public List<TournamentGroup> GetTournamentGroups(int id)
        {
            return _tournamentService.GetTournamentGroups(id);
        }

        [HttpGet("{id}/teams")]
        public List<Team> GetTournamentTeams(int id)
        {
            return _tournamentService.GetTournamentTeams(id);
        }

        [HttpGet("{id}/staff")]
        public List<TournamentStaff> GetTournamentStaff(int id)
        {
            return _tournamentService.GetTournamentStaff(id);
        }

        [HttpPost("{id}/generate-schedule")]
        public IActionResult GenerateTournamentSchedule(int id)
        {
            if (!_tournamentService.IsTournamentOrganiser(id, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Unauthorized();
            }

            _tournamentService.GenerateTournamentSchedule(id);

            return Ok();
        }

        [HttpGet("{id}/current-phase")]
        public TournamentPhase GetCurrentPhase(int id)
        {
            return _tournamentService.GetCurrentTournamentPhase(id);
        }

        [HttpGet("{id}/match-day-slots")]
        public List<TournamentMatchDaySlot> GetTournamentMatchDayslots(int id)
        {
            return _tournamentService.GetTournamentMatchDaySlots(id);
        }

        [HttpPost("{id}/match-day-slots")]
        public IActionResult CreateTournamentMatchDaySlot(TournamentMatchDaySlot tournamentMatchDaySlot)
        {
            if (!_tournamentService.IsTournamentOrganiser(tournamentMatchDaySlot.TournamentId, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Unauthorized();
            }

            _tournamentService.CreateTournamentMatchDaySlot(tournamentMatchDaySlot);

            return Ok();
        }

        [HttpDelete("{tournamentId}/match-day-slots/{matchDaySlotId}")]
        public IActionResult DeleteTournamentMatchDaySlot(int tournamentId, int matchDaySlotId)
        {
            if (!_tournamentService.IsTournamentOrganiser(tournamentId, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Unauthorized();
            }

            _tournamentService.DeleteTournamentMatchDaySlot(matchDaySlotId);

            return Ok();
        }

        [HttpPost("{id}/generate-fantasy-snapshots")]
        public IActionResult GenerateFantasyTeamSnapshots(int id)
        {
            if (!_tournamentService.IsTournamentOrganiser(id, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Unauthorized();
            }

            _fantasyService.GenerateFantasyPlayersSnapshot(id);

            return Ok();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Owner)]
        [HttpPost("{tournamentId}/progress/{matchId}")]
        public IActionResult ProgressTournament(int tournamentId, int matchId)
        {
            _tournamentService.ManageTournamentProgress(tournamentId, matchId);

            return Ok();
        }
    }
}