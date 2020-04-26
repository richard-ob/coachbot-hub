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
    [Route("api/tournaments")]
    [ApiController]
    public class TournamentController : Controller
    {
        private readonly TournamentService _tournamentService;

        public TournamentController(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
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

        [HttpPost]
        public void CreateTournament(Tournament tournament)
        {
            _tournamentService.CreateTournament(tournament);
        }
    }
}
