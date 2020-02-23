using CoachBot.Domain.Services;
using CoachBot.Model;
using CoachBot.Services;
using CoachBot.Services.Matchmaker;
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
        private readonly ChannelService _channelService;
        private readonly ConfigService _configService;
        private readonly PlayerService _playerService;

        public UserController(ChannelService channelService, PlayerService playerService, ConfigService configService)
        {
            _channelService = channelService;
            _configService = configService;
            _playerService = playerService;
        }

        [HttpGet]
        [Authorize]
        public User Get()
        {
            var user = new User();
            if(User.Claims.Any())
            {
                user.Name = User.Identity.Name;
                user.DiscordUserId = ulong.Parse(User.Claims.First().Value);
                user.IsAdministrator = _channelService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value));
                user.PlayerId = _playerService.GetPlayer(user.DiscordUserId, createIfNotExists: true, playerName: User.Identity.Name).Id;
                //user.Channels = _botService.GetChannelsForUser(ulong.Parse(User.Claims.First().Value), false, false);
            }
            return user;
        }

        [HttpGet("/login")]
        public IActionResult LogIn()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = _configService.Config.ClientUrl }, Discord.OAuth2.DiscordDefaults.AuthenticationScheme);
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
