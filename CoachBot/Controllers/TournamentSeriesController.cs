using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using Microsoft.AspNetCore.Authorization;
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
        private readonly PlayerService _playerService;

        public TournamentSeriesController(TournamentService tournamentService, PlayerService playerService)
        {
            _tournamentService = tournamentService;
            _playerService = playerService;
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

        [Authorize]
        [HttpPost]
        public void CreateTournament(TournamentSeries tournament)
        {
            if (!_playerService.IsAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            _tournamentService.CreateTournamentSeries(tournament);
        }
    }
}
