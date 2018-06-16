using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ChannelController : Controller
    {
        private readonly MatchmakerService _matchmakerService;

        public ChannelController(MatchmakerService matchmakerService)
        {
            _matchmakerService = matchmakerService;
        }

        [HttpGet]
        [HttpGet("/channels")]
        public IList<Channel> GetMatchmakers()
        {
            return _matchmakerService.Channels;
        }

        [HttpPost]
        public void UpdateMatchmaker([FromBody]Channel channel)
        {
            _matchmakerService.ConfigureChannel(channel.Id, channel.Team1.Name, channel.Positions, null, null, false, Formation.None, true);
        }
    }
}
