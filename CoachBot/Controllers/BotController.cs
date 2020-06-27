using CoachBot.Domain.Model;
using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class BotController : Controller
    {
        private readonly BotService _botService;

        public BotController(BotService botService)
        {
            _botService = botService;
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpGet("state")]
        public BotState Get()
        {
            return _botService.GetCurrentBotState();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost("reconnect")]
        public void Reconnect()
        {
            _botService.Reconnect();
        }
    }
}
