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
    [Route("api/tournament-groups")]
    [ApiController]
    public class TournamentGroupController : Controller
    {
        private readonly TournamentService _tournamentService;

        public TournamentGroupController(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HttpDelete("{id}")]
        public void DeleteTournamentGroup(int id)
        {
            _tournamentService.DeleteTournamentGroup(id);
        }

        [HttpPost]
        public void CreateTournamentGroup(TournamentGroup tournamentGroup)
        {
            _tournamentService.CreateTournamentGroup(tournamentGroup);
        }

        [HttpPut("{id}")]
        public void UpdateTournamentGroup(TournamentGroup tournamentGroup)
        {
            _tournamentService.UpdateTournamentGroup(tournamentGroup);
        }

        [HttpPost("{id}/teams")]
        public void AddTournamentGroupTeam(TournamentGroupTeamDto tournamentGroupTeamDto)
        {
            _tournamentService.AddTournamentTeam(tournamentGroupTeamDto.TeamId, tournamentGroupTeamDto.TournamentGroupId);
        }

        [HttpPost("{id}/teams/{teamId}")]
        public void RemoveTournamentGroupTeam(int id, int teamId)
        {
            _tournamentService.RemoveTournamentTeam(teamId, id);
        }
    }
}
