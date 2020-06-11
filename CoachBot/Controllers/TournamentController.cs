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
    [Route("api/tournaments")]
    [ApiController]
    public class TournamentController : Controller
    {
        private readonly TournamentService _tournamentService;

        public TournamentController(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HttpGet("current")]
        public List<Tournament> GetCurrentTournamentEditions()
        {
            return _tournamentService.GetTournamentEditions(true);
        }

        [HttpGet]
        public List<Tournament> GetTournamentEditions()
        {
            return _tournamentService.GetTournamentEditions();
        }

        [HttpGet("{id}")]
        public Tournament GetTournamentEdition(int id)
        {
            return _tournamentService.GetTournamentEdition(id);
        }

        [Authorize]
        [HttpPost]
        public void CreateTournamentEdition(Tournament tournamentEdition)
        {
            // TODO: Check player is tournament organiser
            _tournamentService.CreateTournamentEdition(tournamentEdition, User.GetSteamId());
        }

        [Authorize]
        [HttpPut]
        public void UpdateTournamentEdition(Tournament tournamentEdition)
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
        public List<TournamentStaff> GetTournamentEditionStaff(int id)
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
        public List<TournamentMatchDaySlot> GetTournamentEditionMatchDayslots(int id)
        {
            return _tournamentService.GetTournamentMatchDaySlots(id);
        }

        [Authorize]
        [HttpPost("{id}/match-day-slots")]
        public void CreateTournamentMatchDaySlot(TournamentMatchDaySlot tournamentEditionMatchDaySlot)
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
