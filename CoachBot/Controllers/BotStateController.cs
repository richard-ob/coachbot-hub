using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class BotStateController : Controller
    {
        private readonly BotStateService _botService;

        public BotStateController(BotStateService botService)
        {
            _botService = botService;
        }

        [HttpGet]
        public BotState Get()
        {
            return _botService.GetCurrentBotState();
        }

        [HttpPost("reconnect")]
        public void Reconnect()
        {
            _botService.Reconnect();
        }
    }
}
