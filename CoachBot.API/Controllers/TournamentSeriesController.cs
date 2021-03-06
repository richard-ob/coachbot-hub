﻿using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

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

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost]
        public void CreateTournament(TournamentSeries tournament)
        {
            _tournamentService.CreateTournamentSeries(tournament);
        }
    }
}