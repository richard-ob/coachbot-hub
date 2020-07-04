using CoachBot.Extensions;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Mvc;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BotController : Controller
    {
        private readonly BotService _botService;

        public BotController(BotService botService)
        {
            _botService = botService;
        }

        [HttpGet("state")]
        public IActionResult Get()
        {
            if (!Request.IsLocal())
            {
                return Unauthorized();
            }

            return Ok(_botService.GetCurrentBotState());
        }

        [HttpPost("reconnect")]
        public IActionResult Reconnect()
        {
            if (!Request.IsLocal())
            {
                return Unauthorized();
            }

            _botService.Reconnect();

            return Ok();
        }
    }
}