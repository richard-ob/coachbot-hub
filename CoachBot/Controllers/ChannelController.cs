using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class ChannelController : Controller
    {
        private readonly ChannelService _channelService;
        private readonly TeamService _teamService;
        private readonly DiscordService _discordService;

        public ChannelController(ChannelService channelService, TeamService teamService, DiscordService discordService)
        {
            _channelService = channelService;
            _teamService = teamService;
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
            var discordUserId = User.GetDiscordUserId();
            var channel = _channelService.GetChannel(id);

            if (!_teamService.IsTeamCaptain(channel.TeamId, User.GetDiscordUserId()) && !_teamService.IsViceCaptain(channel.TeamId, User.GetDiscordUserId()))
            {
                return Forbid();
            }

            return Ok(channel);
        }

        [HttpPost]
        public IActionResult Create([FromBody]Channel channel)
        {
            if (!_teamService.IsTeamCaptain(channel.TeamId, User.GetDiscordUserId()) && !_teamService.IsViceCaptain(channel.TeamId, User.GetDiscordUserId()))
            {
                return Forbid();
            }

            _channelService.CreateChannel(channel);

            return Ok();
        }

        [HttpPut]
        public IActionResult Update(Channel channel)
        {
            if (!_teamService.IsTeamCaptain(channel.Id, User.GetDiscordUserId()) && !_teamService.IsViceCaptain(channel.Id, User.GetDiscordUserId()))
            {
                return Forbid();
            }

            _channelService.UpdateChannel(channel);

            return Ok();
        }
    }
}
