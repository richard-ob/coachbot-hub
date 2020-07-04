using CoachBot.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class DiscordUserController : Controller
    {
        private readonly DiscordService _discordService;

        public DiscordUserController(DiscordService discordService)
        {
            _discordService = discordService;
        }

        [HttpGet("{id}")]
        public IActionResult Get(ulong id)
        {
            var user = _discordService.GetDiscordUser(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}