using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class ConfigController : Controller
    {
        private readonly ConfigService _configService;

        public ConfigController(ConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet]
        public Config Get()
        {
            return _configService.Config;
        }

        [HttpPost]
        public void Update([FromBody]Config config)
        {
            _configService.UpdateBotToken(config.BotToken);
            _configService.UpdateOAuthTokens(config.OAuth2Id, config.OAuth2Secret);
        }
    }
}
