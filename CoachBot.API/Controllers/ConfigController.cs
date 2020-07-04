using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class ConfigController : Controller
    {
        private readonly ConfigService _configService;
        private readonly BotService _botService;

        public ConfigController(ConfigService configService, BotService botService)
        {
            _configService = configService;
            _botService = botService;
        }

        [HttpGet]
        public Config Get()
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            return _configService.Config;
        }

        [HttpPost]
        public void Update([FromBody]Config config)
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            _configService.UpdateBotToken(config.BotToken);
            _configService.UpdateOAuthTokens(config.OAuth2Id, config.OAuth2Secret);
        }
    }
}
