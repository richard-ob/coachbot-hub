using AspNet.Security.OpenId.Steam;
using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Shared.Extensions;
using CoachBot.Model;
using CoachBot.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoachBot.Shared.Model;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly DiscordService _discordService;
        private readonly PlayerService _playerService;
        private readonly Config _config;

        public UserController(DiscordService discordService, PlayerService playerService, Config config)
        {
            _discordService = discordService;
            _playerService = playerService;
            _config = config;
        }

        [HttpGet]
        [Authorize]
        public User Get()
        {
            var player = _playerService.GetPlayerBySteamId(User.GetSteamId(), createIfNotExists: true, playerName: User.Identity.Name);

            return new User
            {
                Name = User.Identity.Name,
                SteamId = User.GetSteamId(),
                PlayerId = player.Id,
                IsAdministrator = player.HubRole.Equals(PlayerHubRole.Administrator)
            };
        }

        [HttpGet("/login")]
        public IActionResult LogIn()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = _config.ClientUrl }, SteamAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet("/logout")]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync("Cookies").Wait();
            return new RedirectResult(_config.ClientUrl);
        }

        [HttpGet("/unauthorized")]
        public IActionResult AccessDenied()
        {
            return new UnauthorizedResult();
        }
    }
}