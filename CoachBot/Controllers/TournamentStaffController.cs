using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/tournament-staff")]
    [ApiController]
    public class TournamentStaffController : Controller
    {
        private readonly TournamentService _tournamentService;

        public TournamentStaffController(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HttpDelete("{id}")]
        public void DeleteTournamentStaffMember(int id)
        {
            _tournamentService.DeleteTournamentStaff(id);
        }

        [HttpPost]
        public void CreateTournamentStaffMember(TournamentStaff tournamentStaff)
        {
            // TODO: MUST BE ADMIN OF TOURNAMENT
            _tournamentService.CreateTournamentStaff(tournamentStaff);
        }

        [HttpPut("{id}")]
        public void UpdateTournamentStaffMember(TournamentStaff tournamentStaff)
        {
            // TODO: MUST BE ADMIN OF TOURNAMENT
            _tournamentService.UpdateTournamentStaff(tournamentStaff);
        }

    }
}
