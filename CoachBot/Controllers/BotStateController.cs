using CoachBot.Model;
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
    public class BotStateController : Controller
    {
        private readonly BotService _botService;

        public BotStateController(BotService botService)
        {
            _botService = botService;
        }

        [HttpGet]
        public BotState Get()
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            return _botService.GetCurrentBotState();
        }

        [HttpPost("reconnect")]
        public void Reconnect()
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            _botService.Reconnect();
        }
    }
}
