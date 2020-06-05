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
    [Route("api/tournament-editions")]
    [ApiController]
    public class TournamentEditionController : Controller
    {
        private readonly TournamentService _tournamentService;

        public TournamentEditionController(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HttpGet("current")]
        public List<TournamentEdition> GetCurrentTournamentEditions()
        {
            return _tournamentService.GetTournamentEditions(true);
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

        [Authorize]
        [HttpPost]
        public void CreateTournamentEdition(TournamentEdition tournamentEdition)
        {
            // TODO: Check player is tournament organiser
            _tournamentService.CreateTournamentEdition(tournamentEdition, User.GetSteamId());
        }

        [Authorize]
        [HttpPut]
        public void UpdateTournamentEdition(TournamentEdition tournamentEdition)
        {
            // TODO: Check player is tournament organiser
            _tournamentService.UpdateTournamentEdition(tournamentEdition);
        }

        [HttpGet("{id}/groups")]
        public List<TournamentGroup> GetTournamentGroups(int id)
        {
            return _tournamentService.GetTournamentGroups(id);
        }

        [HttpGet("{id}/teams")]
        public List<Team> GetTournamentTeams(int id)
        {
            return _tournamentService.GetTournamentTeams(id);
        }

        [HttpGet("{id}/staff")]
        public List<TournamentEditionStaff> GetTournamentEditionStaff(int id)
        {
            return _tournamentService.GetTournamentEditionStaff(id);
        }

        [Authorize]
        [HttpPost("{id}/generate-schedule")]
        public void GenerateTournamentEditionSchedule(int id)
        {
            // TODO: Check player is tournament organiser
            _tournamentService.GenerateTournamentSchedule(id);
        }

        [HttpGet("{id}/current-phase")]
        public TournamentPhase GetCurrentPhase(int id)
        {
            return _tournamentService.GetCurrentTournamentPhase(id);
        }

        [HttpGet("{id}/match-day-slots")]
        public List<TournamentEditionMatchDaySlot> GetTournamentEditionMatchDayslots(int id)
        {
            return _tournamentService.GetTournamentMatchDaySlots(id);
        }

        [Authorize]
        [HttpPost("{id}/match-day-slots")]
        public void CreateTournamentMatchDaySlot(TournamentEditionMatchDaySlot tournamentEditionMatchDaySlot)
        {
            // TODO: Check player is tournament organiser
            _tournamentService.CreateTournamentMatchDaySlot(tournamentEditionMatchDaySlot);
        }

        [Authorize]
        [HttpDelete("{id}/match-day-slots/{matchDaySlotId}")]
        public void DeleteTournamentMatchDaySlot(int matchDaySlotId)
        {
            _tournamentService.DeleteTournamentMatchDaySlot(matchDaySlotId);
        }
    }
}
