using CoachBot.Domain.Services;
using CoachBot.Shared.Extensions;
using CoachBot.Shared.Model;
using CoachBot.Shared.Services;
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
        private readonly CacheService _cacheService;
        private readonly Config _config;

        public DiscordVerificationController(PlayerService playerService, Config config, CacheService cacheService)
        {
            _playerService = playerService;
            _config = config;
            _cacheService = cacheService;
        }

        [Authorize]
        [HttpGet("/verify-discord")]
        public IActionResult Verify()
        {
            var steamId = User.GetSteamId();
            var token = Guid.NewGuid().ToString();

            HttpContext.SignOutAsync("Cookies").Wait();

            _cacheService.Set(CacheService.CacheItemType.DiscordVerificationSessionExpiry, steamId.ToString(), DateTime.UtcNow.AddMinutes(5));
            _cacheService.Set(CacheService.CacheItemType.DiscordVerificationSessionToken, steamId.ToString(), token);

            return Challenge(new AuthenticationProperties { RedirectUri = "/verification-complete?steamId=" + steamId + "&token=" + token }, Discord.OAuth2.DiscordDefaults.AuthenticationScheme);
        }

        [HttpGet("/verification-complete")]
        public IActionResult VerificationComplete(ulong steamId, string token)
        {
            try
            {
                var verificationSessionExpiry = _cacheService.Get(CacheService.CacheItemType.DiscordVerificationSessionExpiry, steamId.ToString()) as DateTime?;
                var verificationSessionToken = _cacheService.Get(CacheService.CacheItemType.DiscordVerificationSessionToken, steamId.ToString()) as string;
                if (verificationSessionExpiry != null && verificationSessionExpiry.Value > DateTime.UtcNow && !string.IsNullOrEmpty(verificationSessionToken) && verificationSessionToken == token)
                {
                    var discordUserId = User.GetDiscordUserId();
                    if (discordUserId != null)
                    {
                        _playerService.UpdateDiscordUserId(discordUserId.Value, steamId);
                    }
                    _cacheService.Remove(CacheService.CacheItemType.DiscordVerificationSessionExpiry, steamId.ToString());
                }
            }
            finally
            {
                // INFO: If we don't wrap the mechanism in try/finally, we don't log the temporary Discord user out, causing a player record to be created with the Discord User ID as Steam ID, due to the cookie created
                HttpContext.SignOutAsync("Cookies").Wait();
            }      

            return new RedirectResult($"{Request.Scheme}://{_config.WebServerConfig.ClientUrl}" + PROFILE_EDITOR_PATH);
        }
    }
}