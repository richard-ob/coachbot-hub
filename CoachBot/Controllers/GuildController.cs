using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class GuildController : Controller
    {
        private readonly BotService _botService;

        public GuildController(BotService botService)
        {
            _botService = botService;
        }

        [HttpDelete("{id}")]
        public void Leave(string id)
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            _botService.LeaveGuild(id);
        }
    }
}
