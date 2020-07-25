using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class ChannelController : Controller
    {
        private readonly ChannelService _channelService;
        private readonly TeamService _teamService;
        private readonly PlayerService _playerService;
        private readonly DiscordService _discordService;

        public ChannelController(ChannelService channelService, PlayerService playerService, TeamService teamService, DiscordService discordService)
        {
            _channelService = channelService;
            _teamService = teamService;
            _playerService = playerService;
            _discordService = discordService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_channelService.GetChannels());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var steamUserId = User.GetSteamId();
            var channel = _channelService.GetChannel(id);

            if (!_teamService.IsTeamCaptain(channel.TeamId, steamUserId) && !_teamService.IsViceCaptain(channel.TeamId, steamUserId) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Forbid();
            }

            return Ok(channel);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Player)]
        [HttpPost]
        public IActionResult Create([FromBody]Channel channel)
        {
            if (!_teamService.IsTeamCaptain(channel.TeamId, User.GetSteamId()) && !_teamService.IsViceCaptain(channel.TeamId, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Forbid();
            }

            _channelService.CreateChannel(channel);

            return Ok();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Player)]
        [HttpPut]
        public IActionResult Update([FromBody]Channel channel)
        {
            if (!_teamService.IsTeamCaptain(channel.Id, User.GetSteamId()) && !_teamService.IsViceCaptain(channel.Id, User.GetSteamId()) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Forbid();
            }

            _channelService.UpdateChannel(channel);

            return Ok();
        }
    }
}