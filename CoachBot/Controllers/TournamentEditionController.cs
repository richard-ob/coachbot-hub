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
            return _tournamentService.GetTournamentEdition(id);
        }

        [HttpPost]
        public void CreateTournamentEdition(TournamentEdition tournamentEdition)
        {
            _tournamentService.CreateTournamentEdition(tournamentEdition);
        }

        [HttpPut]
        public void UpdateTournamentEdition(TournamentEdition tournamentEdition)
        {
            _tournamentService.UpdateTournamentEdition(tournamentEdition);
        }

        [HttpGet("{id}/groups")]
        public List<TournamentGroup> GetTournamentGroups(int tournamentEditionId)
        {
            return _tournamentService.GetTournamentGroups(tournamentEditionId);
        }

        [HttpPost("{id}/generate-schedule")]
        public void GenerateTournamentEditionSchedule(int id)
        {
            _tournamentService.GenerateTournamentSchedule(id);
        }

    }
}
