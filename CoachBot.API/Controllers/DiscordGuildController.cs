using CoachBot.Domain.Services;
using CoachBot.Shared.Extensions;
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
        private readonly PlayerService _playerService;

        public DiscordGuildController(DiscordService discordService, PlayerService playerService)
        {
            _discordService = discordService;
            _playerService = playerService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_discordService.GetGuildsForUser(User.GetSteamId()));
        }

        [HttpGet("{id}/channels")]
        public IActionResult GetForGuild(ulong id)
        {
            var steamId = User.GetSteamId();
            if (!_discordService.UserIsGuildAdministrator(steamId, id) && !_playerService.IsOwner(User.GetSteamId()))
            {
                return Unauthorized();
            }

            return Ok(_discordService.GetChannelsForGuild(id));
        }
    }
}