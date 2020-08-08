using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Models;
using CoachBot.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/fantasy")]
    [ApiController]
    public class FantasyController : Controller
    {
        private readonly FantasyService _fantasyService;

        public FantasyController(FantasyService fantasyService)
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

        [HubRolePermission(HubRole = PlayerHubRole.Player)]
        [HttpPost]
        public void CreateFantasyTeam(FantasyTeam fantasyTeam)
        {
            _fantasyService.CreateFantasyTeam(fantasyTeam, User.GetSteamId());
        }

        [HubRolePermission(HubRole = PlayerHubRole.Player)]
        [HttpPut]
        public void UpdateFantasyTeam(FantasyTeam fantasyTeam)
        {
            _fantasyService.UpdateFantasyTeam(fantasyTeam, User.GetSteamId());
        }

        [HubRolePermission(HubRole = PlayerHubRole.Player)]
        [HttpPost("{id}/selections")]
        public void AddFantasyTeamSelection(FantasyTeamSelection fantasySelection)
        {
            _fantasyService.AddFantasyTeamSelection(fantasySelection, User.GetSteamId());
        }

        [HubRolePermission(HubRole = PlayerHubRole.Player)]
        [HttpDelete("{id}/selections/{fantasyTeamSelectionId}")]
        public void RemoveFantasyTeamSelection(int fantasyTeamSelectionId)
        {
            _fantasyService.RemoveFantasyTeamSelection(fantasyTeamSelectionId, User.GetSteamId());
        }

        [HttpPost("tournament/{tournamentId}/players")]
        public PagedResult<FantasyPlayer> GetFantasyPlayers([FromBody]PagedPlayerStatisticsRequestDto pagedRequest)
        {
            var players = _fantasyService.GetFantasyPlayers(pagedRequest.Page, pagedRequest.PageSize, pagedRequest.SortOrderFull, pagedRequest.Filters);
            return players;
        }

        [Authorize]
        [HttpGet("tournament/available")]
        public IEnumerable<Tournament> GetAvailableTournamentsForUser()
        {
            return _fantasyService.GetAvailableTournamentsForUser(User.GetSteamId());
        }

        [HttpGet("tournament/{tournamentId}/rankings")]
        public IEnumerable<FantasyTeamRank> GetFantasyTeamRankings(int tournamentId)
        {
            return _fantasyService.GetFantasyTeamRankings(tournamentId);
        }

        [HttpGet("tournament/phase/{tournamentPhaseId}/rankings")]
        public IEnumerable<FantasyTeamRank> GetFantasyTeamPhaseRankings(int tournamentPhaseId)
        {
            return _fantasyService.GetFantasyTeamPhaseRankings(tournamentPhaseId);
        }

        [HttpGet("tournament/{tournamentId}/player-rankings")]
        public IEnumerable<FantasyPlayerRank> GetFantasyPlayerRankings(int tournamentId)
        {
            return _fantasyService.GetFantasyPlayerRankings(tournamentId);
        }

        [HttpGet("tournament/phase/{tournamentPhaseId}/player-rankings")]
        public IEnumerable<FantasyPlayerRank> GetFantasyPlayerPhaseRankings(int tournamentPhaseId)
        {
            return _fantasyService.GetFantasyPlayerPhaseRankings(tournamentPhaseId);
        }

        [HttpGet("tournament/{tournamentId}/current-phase-spotlight-player")]
        public FantasyPlayerRank GetCurrentPhaseSpotlightPlayer(int tournamentId)
        {
            return _fantasyService.GetFantasyPlayerRankSpotlight(tournamentId);
        }

        [HttpGet("tournament/{tournamentId}/current-phase-spotlight-team")]
        public FantasyTeamRank GetCurrentPhaseSpotlightTeam(int tournamentId)
        {
            return _fantasyService.GetFantasyTeamrRankSpotlight(tournamentId);
        }

        [HttpGet("tournament/{tournamentId}/team-summaries")]
        public IEnumerable<FantasyTeamSummary> GetFantasyTeamSummaries(int tournamentId)
        {
            return _fantasyService.GetFantasyTeamSummaries(tournamentId);
        }

        [HttpGet("{fantasyTeamId}/summary")]
        public FantasyTeamSummary GetFantasyTeamSummary(int fantasyTeamId)
        {
            return _fantasyService.GetFantasyTeamSummary(fantasyTeamId);
        }

        [HttpGet("{fantasyTeamId}/performances")]
        public List<FantasyPlayerPerformance> GetFantasyPlayerPerformances(int fantasyTeamId, [FromQuery]int? tournamentPhaseId = null)
        {
            return _fantasyService.GetFantasyTeamPlayerPerformances(fantasyTeamId, tournamentPhaseId);
        }

        [Authorize]
        [HttpGet("teams/@me")]
        public IEnumerable<FantasyTeamSummary> GetFantasyTeamSummariesForUser()
        {
            return _fantasyService.GetFantasyTeamSummariesForPlayer(User.GetSteamId());
        }
    }
}