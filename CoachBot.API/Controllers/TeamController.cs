using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Shared.Extensions;
using CoachBot.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : Controller
    {
        private readonly TeamService _teamService;
        private readonly PlayerService _playerService;
        private readonly MatchStatisticsService _matchStatisticsService;

        public TeamController(TeamService teamService, PlayerService playerService, MatchStatisticsService matchStatisticsService)
        {
            _teamService = teamService;
            _playerService = playerService;
            _matchStatisticsService = matchStatisticsService;
        }

        [HttpGet("{id}")]
        public Team Get(int id)
        {
            return _teamService.GetTeam(id);
        }

        [HttpGet("code/{teamCode}/region/{regionId}")]
        public Team GetByCode(string teamCode, int regionId)
        {
            return _teamService.GetTeam(teamCode, regionId);
        }

        [HttpGet("{id}/squad")]
        public List<PlayerTeamStatisticsTotals> GetTeamSquad(int id)
        {
            return _matchStatisticsService.GetPlayerTeamStatistics(null, id, true);
        }

        [HttpGet("{id}/player-history")]
        public List<PlayerTeamStatisticsTotals> GetTeamPlayerHistory(int id)
        {
            return _matchStatisticsService.GetPlayerTeamStatistics(null, id, false);
        }

        [HttpGet("region/{regionId}")]
        public List<Team> GetAll(int regionId, [FromQuery]TeamType? teamType = null)
        {
            return _teamService.GetTeams(regionId, teamType);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Player)]
        [HttpPost]
        public IActionResult Create(Team team)
        {
            _teamService.CreateTeam(team, User.GetSteamId());

            return Ok();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Player)]
        [HttpPut]
        public IActionResult Update(Team team)
        {
            if (!_teamService.IsTeamCaptain(team.Id, User.GetSteamId()) && !_teamService.IsViceCaptain(team.Id, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Forbid();
            }

            _teamService.UpdateTeam(team);

            return Ok();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Player)]
        [HttpPost]
        [Route("update-guild-id")]
        public IActionResult UpdateGuildId([FromBody]UpdateGuildIdDto updateGuildIdDto)
        {
            if (!_teamService.IsTeamCaptain(updateGuildIdDto.TeamId, User.GetSteamId()) && !_teamService.IsViceCaptain(updateGuildIdDto.TeamId, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Forbid();
            }

            _teamService.UpdateTeamGuildId(updateGuildIdDto.TeamId, updateGuildIdDto.GuildId);

            return Ok();
        }
    }
}