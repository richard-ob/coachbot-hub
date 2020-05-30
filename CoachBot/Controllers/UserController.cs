using AspNet.Security.OpenId.Steam;
using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Model;
using CoachBot.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly DiscordService _discordService;
        private readonly ConfigService _configService;
        private readonly PlayerService _playerService;

        public UserController(DiscordService discordService, PlayerService playerService, ConfigService configService)
        {
            _discordService = discordService;
            _configService = configService;
            _playerService = playerService;
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
                PlayerId = player.Id, // TODO: Figure out if this works for Steam
                IsAdministrator = player.HubRole.Equals(PlayerHubRole.Administrator)
            };
        }

        [HttpGet("/login")]
        public IActionResult LogIn()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = _configService.Config.ClientUrl }, SteamAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet("/logout")]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync("Cookies").Wait();
            return new RedirectResult(_configService.Config.ClientUrl);
        }

        [HttpGet("/unauthorized")]
        public IActionResult AccessDenied()
        {
            return new UnauthorizedResult();
        }
    }
}
