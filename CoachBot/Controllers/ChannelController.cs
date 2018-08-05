using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class ChannelController : Controller
    {
        private readonly MatchmakerService _matchmakerService;
        private readonly BotService _botService;

        public ChannelController(MatchmakerService matchmakerService, BotService botService)
        {
            _matchmakerService = matchmakerService;
            _botService = botService;
        }

        [HttpGet]
        public IList<Channel> Get()
        {
            var userId = ulong.Parse(User.Claims.ToList().First().Value);
            return _botService.GetChannelsForUser(userId, false); 
        }

        [HttpGet("unconfigured")]
        public IList<Channel> GetUnconfiguredChannels()
        {
            var userId = ulong.Parse(User.Claims.ToList().First().Value);
            return _botService.GetChannelsForUser(userId, true);
        }

        [HttpPost]
        public void Update([FromBody]Channel channel)
        {
            _matchmakerService.ConfigureChannel(channel.Id, channel.Team1.Name, channel.Positions, null, null, false, Formation.None, true);
        }
    }
}
