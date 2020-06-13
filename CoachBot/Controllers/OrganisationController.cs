using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/organisations")]
    [ApiController]
    public class OrganisationController : Controller
    {
        private readonly TournamentService _tournamentService;
        private readonly PlayerService _playerService;

        public OrganisationController(TournamentService tournamentService, PlayerService playerService)
        {
            _tournamentService = tournamentService;
            _playerService = playerService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public Organisation Get(int id)
        {
            if (!_playerService.IsAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }

            return _tournamentService.GetOrganisation(id);
        }

        [Authorize]
        [HttpGet]
        public IEnumerable<Organisation> GetAll()
        {
            if (!_playerService.IsAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            return _tournamentService.GetOrganisations();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            if (!_playerService.IsAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            _tournamentService.RemoveOrganisation(id);
        }

        [Authorize]
        [HttpPost]
        public void Create(Organisation organisation)
        {
            if (!_playerService.IsAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            _tournamentService.CreateOrganisation(organisation);
        }
    }
}
