using AspNetCore.Proxy;
using CoachBot.Model;
using CoachBot.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/steam-proxy")]
    public class SteamProxyController : Controller
    {
        private readonly Config _config;
        private const string STEAM_URL = "http://api.steampowered.com";
        private const string STEAM_PLAYER_URL = STEAM_URL + "/IPlayerService";
        private const string STEAM_USER_URL = STEAM_URL + "/ISteamUser";
        private const string STEAM_NEWS_URL = STEAM_URL + "/ISteamNews";
        private const string STEAM_IOSOCCER_APPID = "673560";

        public SteamProxyController(Config config)
        {
            _config = config;
            if (string.IsNullOrWhiteSpace(_config.SteamApiToken)) throw new Exception("Missing Steam API Token");
        }

        [HttpGet("user-profiles")]
        public Task GetPlayerSummaries([FromQuery]string steamIdsCsv)
        {
            var url = STEAM_USER_URL + "/GetPlayerSummaries/v0002/?key=" + _config.SteamApiToken + "&steamids=" + steamIdsCsv;

            return this.HttpProxyAsync(url);
        }

        [HttpGet("playing-time")]
        public Task GetRecentlyPlayedGames([FromQuery]string steamId)
        {
            var url = STEAM_PLAYER_URL + "/GetRecentlyPlayedGames/v0001/?key=" + _config.SteamApiToken + "&steamid=" + steamId;

            return this.HttpProxyAsync(url);
        }

        [HttpGet("nicknames")]
        public Task GetOwnedGames([FromQuery]string steamId)
        {
            var url = STEAM_PLAYER_URL + "/GetOwnedGames/v0001/?key=" + _config.SteamApiToken + "&steamid=" + steamId;

            return this.HttpProxyAsync(url);
        }

        [HttpGet("news")]
        public Task GetNewsForApp()
        {
            var url = STEAM_NEWS_URL + "/GetNewsForApp/v2/?key=" + _config.SteamApiToken + "&appid=" + STEAM_IOSOCCER_APPID;

            return this.HttpProxyAsync(url);
        }
    }
}