using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/tournament-groups")]
    [ApiController]
    public class TournamentGroupController : Controller
    {
        private readonly TournamentService _tournamentService;
        private readonly PlayerService _playerService;

        public TournamentGroupController(TournamentService tournamentService, PlayerService playerService)
        {
            _tournamentService = tournamentService;
            _playerService = playerService;
        }

        [Authorize]
        [HttpDelete("{id}")]
        public void DeleteTournamentGroup(int id)
        {
            if (!_playerService.IsAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            _tournamentService.DeleteTournamentGroup(id);
        }

        [Authorize]
        [HttpPost]
        public void CreateTournamentGroup(TournamentGroup tournamentGroup)
        {
            if (!_playerService.IsAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            _tournamentService.CreateTournamentGroup(tournamentGroup);
        }

        [Authorize]
        [HttpPut("{id}")]
        public void UpdateTournamentGroup(TournamentGroup tournamentGroup)
        {
            if (!_playerService.IsAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            _tournamentService.UpdateTournamentGroup(tournamentGroup);
        }

        [Authorize]
        [HttpPost("{id}/teams")]
        public void AddTournamentGroupTeam(TournamentGroupTeamDto tournamentGroupTeamDto)
        {
            if (!_playerService.IsAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            _tournamentService.AddTournamentTeam(tournamentGroupTeamDto.TeamId, tournamentGroupTeamDto.TournamentGroupId);
        }

        [Authorize]
        [HttpPost("{id}/teams/{teamId}")]
        public void RemoveTournamentGroupTeam(int id, int teamId)
        {
            if (!_playerService.IsAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            _tournamentService.RemoveTournamentTeam(teamId, id);
        }
    }
}
