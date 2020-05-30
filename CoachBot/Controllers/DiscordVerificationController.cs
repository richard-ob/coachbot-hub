using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Services;
using CoachBot.Tools;
using Effortless.Net.Encryption;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            if (_cacheService.Get(CacheService.CacheItemType.EncryptionKey, "") == null)
            {
                _cacheService.Set(CacheService.CacheItemType.EncryptionIv, "", EncryptionTools.GetRandomData(128));
                _cacheService.Set(CacheService.CacheItemType.EncryptionKey, "", EncryptionTools.GetRandomData(256));
            }
        }

        [Authorize]
        [HttpGet("/verify-discord")]
        public IActionResult Verify()
        {
            var steamId = User.GetSteamId();
            byte[] key = GetKey();
            byte[] iv = GetIv();
            string encrypted = EncryptionTools.Encrypt(steamId.ToString(), key, iv);

            return Challenge(new AuthenticationProperties { RedirectUri = "/verification-complete?steamId=" + encrypted }, Discord.OAuth2.DiscordDefaults.AuthenticationScheme);
        }

        [HttpGet("/verification-complete")]
        public IActionResult VerificationComplete(string encryptedSteamId)
        {
            byte[] key = GetKey();
            byte[] iv = GetIv();
            var steamId = EncryptionTools.Decrypt(encryptedSteamId, key, iv);
            _playerService.UpdateDiscordUserId(User.GetDiscordUserId(), ulong.Parse(steamId));
            HttpContext.SignOutAsync("Cookies").Wait();

            return new RedirectResult(_configService.Config.ClientUrl + PROFILE_EDITOR_PATH);
        }

        private byte[] GetKey()
        {
            return _cacheService.Get(CacheService.CacheItemType.EncryptionKey, "") as byte[];
        }

        private byte[] GetIv()
        {
            return _cacheService.Get(CacheService.CacheItemType.EncryptionIv, "") as byte[];
        }
    }
}
