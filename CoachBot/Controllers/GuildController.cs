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
    public class GuildController : Controller
    {
        private readonly BotStateService _botStateService;

        public GuildController(BotStateService botStateService)
        {
            _botStateService = botStateService;
        }

        [HttpDelete("{id}")]
        public void Leave(string id)
        {
            _botStateService.LeaveGuild(id);
        }
    }
}
