using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

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

        [Authorize]
        [HttpDelete("{id}")]
        public void DeleteTournamentStaffMember(int id)
        {
            _tournamentService.DeleteTournamentStaff(id);
        }

        [Authorize]
        [HttpPost]
        public void CreateTournamentStaffMember(TournamentStaff tournamentStaff)
        {
            if (!_playerService.IsAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            // TODO: MUST BE ADMIN OF TOURNAMENT
            _tournamentService.CreateTournamentStaff(tournamentStaff);
        }

        [Authorize]
        [HttpPut("{id}")]
        public void UpdateTournamentStaffMember(TournamentStaff tournamentStaff)
        {
            if (!_playerService.IsAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            // TODO: MUST BE ADMIN OF TOURNAMENT
            _tournamentService.UpdateTournamentStaff(tournamentStaff);
        }

    }
}
