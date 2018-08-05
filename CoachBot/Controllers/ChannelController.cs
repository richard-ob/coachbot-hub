using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class ChannelController : Controller
    {
        private readonly MatchmakerService _matchmakerService;

        public ChannelController(MatchmakerService matchmakerService)
        {
            _matchmakerService = matchmakerService;
        }

        [HttpGet]
        public IList<Channel> Get()
        {
            return _matchmakerService._config.Channels;
        }

        [HttpPost]
        public void Update([FromBody]Channel channel)
        {
            _matchmakerService.ConfigureChannel(channel.Id, channel.Team1.Name, channel.Positions, null, null, false, Formation.None, true);
        }
    }
}
