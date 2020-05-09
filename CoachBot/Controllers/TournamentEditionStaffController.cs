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
    [Route("api/tournament-edition-staff")]
    [ApiController]
    public class TournamentEditionStaffController : Controller
    {
        private readonly TournamentService _tournamentService;

        public TournamentEditionStaffController(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HttpDelete("{id}")]
        public void DeleteTournamentStaffMember(int id)
        {
            _tournamentService.DeleteTournamentEditionStaff(id);
        }

        [HttpPost]
        public void CreateTournamentStaffMember(TournamentEditionStaff tournamentEditionStaff)
        {
            // TODO: MUST BE ADMIN OF TOURNAMENT
            _tournamentService.CreateTournamentEditionStaff(tournamentEditionStaff);
        }

        [HttpPut("{id}")]
        public void UpdateTournamentStaffMember(TournamentEditionStaff tournamentEditionStaff)
        {
            // TODO: MUST BE ADMIN OF TOURNAMENT
            _tournamentService.UpdateTournamentEditionStaff(tournamentEditionStaff);
        }

    }
}
