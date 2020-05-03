
using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : Controller
    {
        private readonly TeamService _teamService;
        private readonly MatchStatisticsService _matchStatisticsService;

        public TeamController(TeamService teamService, MatchStatisticsService matchStatisticsService)
        {
            _teamService = teamService;
            _matchStatisticsService = matchStatisticsService;
        }

        [HttpGet("{id}")]
        public Team Get(int id)
        {
            return _teamService.GetTeam(id);
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

        [HttpGet]
        public List<Team> GetAll()
        {
            return _teamService.GetTeams();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(Team team)
        {
            _teamService.CreateTeam(team, User.GetDiscordUserId());

            return Ok();
        }

        [Authorize]
        [HttpPut]
        public IActionResult Update(Team team)
        {
            if (!_teamService.IsTeamCaptain(team.Id, User.GetDiscordUserId()) && !_teamService.IsViceCaptain(team.Id, User.GetDiscordUserId()))
            {
                return Forbid();
            }

            _teamService.UpdateTeam(team);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("update-guild-id")]
        public IActionResult UpdateGuildId([FromBody]UpdateGuildIdDto updateGuildIdDto)
        {
            if (!_teamService.IsTeamCaptain(updateGuildIdDto.TeamId, User.GetDiscordUserId()) && !_teamService.IsViceCaptain(updateGuildIdDto.TeamId, User.GetDiscordUserId()))
            {
                return Forbid();
            }

            _teamService.UpdateTeamGuildId(updateGuildIdDto.TeamId, updateGuildIdDto.GuildId);

            return Ok();
        }
    }
}
