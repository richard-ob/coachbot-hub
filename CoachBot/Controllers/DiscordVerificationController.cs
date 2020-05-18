using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Model;
using CoachBot.Models;
using CoachBot.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DiscordVerificationController : Controller
    {
        private readonly PlayerService _playerService;
        private readonly ConfigService _configService;

        public DiscordVerificationController(PlayerService playerService, ConfigService configService)
        {
            _playerService = playerService;
            _configService = configService;
        }

        [Authorize]
        [HttpGet("/verify-discord")]
        public IActionResult Verify()
        {
            var steamId = User.GetSteamId();
            return Challenge(new AuthenticationProperties { RedirectUri = "/verification-complete?steamId=" + steamId.ToString() }, Discord.OAuth2.DiscordDefaults.AuthenticationScheme);
        }

        [HttpGet("/verification-complete")]
        public IActionResult VerificationComplete(ulong steamId)
        {
            const string PROFILE_EDITOR_PATH = "/edit-profile";
            _playerService.UpdateDiscordUserId(User.GetDiscordUserId(), steamId);
            HttpContext.SignOutAsync("Cookies").Wait();
            return new RedirectResult(_configService.Config.ClientUrl + PROFILE_EDITOR_PATH);
        }
    }
}
