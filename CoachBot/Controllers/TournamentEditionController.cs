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
    [Route("api/tournament-editions")]
    [ApiController]
    public class TournamentEditionController : Controller
    {
        private readonly TournamentService _tournamentService;

        public TournamentEditionController(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HttpGet]
        public List<TournamentEdition> GetTournamentEditions()
        {
            return _tournamentService.GetTournamentEditions();
        }

        [HttpGet("{id}")]
        public TournamentEdition GetTournamentEdition(int id)
        {
            return _tournamentService.GetTournamentEditions().First(t => t.Id == id);
        }
    }
}
