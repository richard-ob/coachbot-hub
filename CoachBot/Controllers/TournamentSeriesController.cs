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
    [Route("api/tournament-series")]
    [ApiController]
    public class TournamentSeriesController : Controller
    {
        private readonly TournamentService _tournamentService;

        public TournamentSeriesController(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HttpGet]
        public List<TournamentSeries> GetTournaments()
        {
            return _tournamentService.GetTournamentSeries();
        }

        [HttpGet("{id}")]
        public TournamentSeries GetTournament(int id)
        {
            return _tournamentService.GetTournamentSeries(id);
        }

        [HttpPost]
        public void CreateTournament(TournamentSeries tournament)
        {
            _tournamentService.CreateTournamentSeries(tournament);
        }
    }
}
