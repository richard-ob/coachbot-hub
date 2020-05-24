using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/fantasy")]
    [ApiController]
    public class FantasyTeamController : Controller
    {
        private readonly FantasyService _fantasyService;

        public FantasyTeamController(FantasyService fantasyService)
        {
            _fantasyService = fantasyService;
        }

        [HttpGet("tournament/{tournamentEditionId}")]
        public IEnumerable<FantasyTeam> GetFantasyTeams(int tournamentEditionId)
        {
            return _fantasyService.GetFantasyTeams(tournamentEditionId);
        }

        [HttpGet("{id}")]
        public FantasyTeam GetFantasyTeam(int id)
        {
            return _fantasyService.GetFantasyTeam(id);
        }

        [HttpPost]
        public void CreateFantasyTeam(FantasyTeam fantasyTeam)
        {
            _fantasyService.CreateFantasyTeam(fantasyTeam);
        }

        [HttpPut]
        public void UpdateFantasyTeam(FantasyTeam fantasyTeam)
        {
            _fantasyService.UpdateFantasyTeam(fantasyTeam);
        }

        [HttpPost("{id}/selections")]
        public void AddFantasyTeamSelection(FantasyTeamSelection fantasySelection)
        {
            _fantasyService.AddFantasyTeamSelection(fantasySelection);
        }

        [HttpDelete("{id}/selections/{fantasyTeamSelectionId}")]
        public void RemoveFantasyTeamSelection(int fantasyTeamSelectionId)
        {
            _fantasyService.RemoveFantasyTeamSelection(fantasyTeamSelectionId);
        }

        [HttpPost("tournament/{tournamentEditionId}/players")]
        public PagedResult<FantasyPlayer> GetFantasyPlayers([FromBody]PagedPlayerStatisticsRequestDto pagedRequest)
        {
            var players = _fantasyService.GetFantasyPlayers(pagedRequest.Page, pagedRequest.PageSize, pagedRequest.SortOrderFull, pagedRequest.Filters);
            return players;
        }

        [HttpGet("tournament/available")]
        public IEnumerable<TournamentEdition> GetAvailableTournamentsForUser()
        {
            return _fantasyService.GetAvailableTournamentsForUser(User.GetSteamId());
        }

    }
}
