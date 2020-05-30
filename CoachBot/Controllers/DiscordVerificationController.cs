using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Services;
using CoachBot.Tools;
using Effortless.Net.Encryption;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DiscordVerificationController : Controller
    {
        private const string PROFILE_EDITOR_PATH = "/edit-profile";
        private readonly PlayerService _playerService;
        private readonly ConfigService _configService;
        private readonly CacheService _cacheService;

        public DiscordVerificationController(PlayerService playerService, ConfigService configService, CacheService cacheService)
        {
            _playerService = playerService;
            _configService = configService;
            _cacheService = cacheService;
        }

        [Authorize]
        [HttpGet("/verify-discord")]
        public IActionResult Verify()
        {
            var steamId = User.GetSteamId();

            _cacheService.Set(CacheService.CacheItemType.DiscordVerificationSessionExpiry, steamId.ToString(), DateTime.Now.AddMinutes(5));

            return Challenge(new AuthenticationProperties { RedirectUri = "/verification-complete?steamId=" + steamId }, Discord.OAuth2.DiscordDefaults.AuthenticationScheme);
        }

        [HttpGet("/verification-complete")]
        public IActionResult VerificationComplete(ulong steamId)
        {
            DateTime? verificationSessionExpiry = _cacheService.Get(CacheService.CacheItemType.DiscordVerificationSessionExpiry, steamId.ToString()) as DateTime?;
            if (verificationSessionExpiry != null && verificationSessionExpiry.Value > DateTime.Now)
            {
                _playerService.UpdateDiscordUserId(User.GetDiscordUserId(), steamId);
                _cacheService.Remove(CacheService.CacheItemType.DiscordVerificationSessionExpiry, steamId.ToString());
            }

            HttpContext.SignOutAsync("Cookies").Wait();

            return new RedirectResult(_configService.Config.ClientUrl + PROFILE_EDITOR_PATH);
        }
    }
}
