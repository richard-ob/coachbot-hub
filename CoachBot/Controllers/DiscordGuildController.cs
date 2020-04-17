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
    public class DiscordGuildController : Controller
    {
        private readonly DiscordService _discordService;

        public DiscordGuildController(DiscordService discordService)
        {
            _discordService = discordService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_discordService.GetGuildsForUser(User.GetDiscordUserId()));
        }

        [HttpGet("{id}/channels")]
        public IActionResult GetForGuild(ulong id)
        {
            var discordUserId = User.GetDiscordUserId();
            if (!_discordService.UserIsGuildAdministrator(discordUserId, id) && !_discordService.UserIsOwningGuildAdmin(discordUserId))
            {
                return Unauthorized();
            }

            return Ok(_discordService.GetChannelsForGuild(id));
        }
    }
}
