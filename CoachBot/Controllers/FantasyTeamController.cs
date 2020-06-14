using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Models;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("tournament/{tournamentId}")]
        public IEnumerable<FantasyTeam> GetFantasyTeams(int tournamentId)
        {
            return _fantasyService.GetFantasyTeams(tournamentId);
        }

        [HttpGet("{id}")]
        public FantasyTeam GetFantasyTeam(int id)
        {
            return _fantasyService.GetFantasyTeam(id);
        }

        [Authorize]
        [HttpPost]
        public void CreateFantasyTeam(FantasyTeam fantasyTeam)
        {
            _fantasyService.CreateFantasyTeam(fantasyTeam);
        }

        [Authorize]
        [HttpPut]
        public void UpdateFantasyTeam(FantasyTeam fantasyTeam)
        {
            _fantasyService.UpdateFantasyTeam(fantasyTeam);
        }

        [Authorize]
        [HttpPost("{id}/selections")]
        public void AddFantasyTeamSelection(FantasyTeamSelection fantasySelection)
        {
            _fantasyService.AddFantasyTeamSelection(fantasySelection);
        }

        [Authorize]
        [HttpDelete("{id}/selections/{fantasyTeamSelectionId}")]
        public void RemoveFantasyTeamSelection(int fantasyTeamSelectionId)
        {
            _fantasyService.RemoveFantasyTeamSelection(fantasyTeamSelectionId);
        }

        [Authorize]
        [HttpPost("tournament/{tournamentId}/players")]
        public PagedResult<FantasyPlayer> GetFantasyPlayers([FromBody]PagedPlayerStatisticsRequestDto pagedRequest)
        {
            var players = _fantasyService.GetFantasyPlayers(pagedRequest.Page, pagedRequest.PageSize, pagedRequest.SortOrderFull, pagedRequest.Filters);
            return players;
        }

        [HttpGet("tournament/available")]
        public IEnumerable<Tournament> GetAvailableTournamentsForUser()
        {
            return _fantasyService.GetAvailableTournamentsForUser(User.GetSteamId());
        }

    }
}
