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
            _tournamentService.DeleteTournamentEditionStaff(id);
        }

        [HttpPost]
        public void CreateTournamentStaffMember(TournamentStaff tournamentEditionStaff)
        {
            // TODO: MUST BE ADMIN OF TOURNAMENT
            _tournamentService.CreateTournamentEditionStaff(tournamentEditionStaff);
        }

        [HttpPut("{id}")]
        public void UpdateTournamentStaffMember(TournamentStaff tournamentEditionStaff)
        {
            // TODO: MUST BE ADMIN OF TOURNAMENT
            _tournamentService.UpdateTournamentEditionStaff(tournamentEditionStaff);
        }

    }
}
