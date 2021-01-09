using AspNetCore.Proxy;
using CoachBot.Domain.Model;
using CoachBot.Shared.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class BotController : Controller
    {
        private readonly Config _config;

        public BotController(Config config)
        {
            _config = config;
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpGet("state")]
        public Task GetCurrentState()
        {
            return this.HttpProxyAsync(GetBotApiUrl("bot/state"));
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost("reconnect")]
        public async Task<IActionResult> Reconnect()
        {
            await this.HttpProxyAsync(GetBotApiUrl("bot/reconnect"));

            return NoContent();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost("disconnect")]
        public async Task<IActionResult> Disconnect()
        {
            await this.HttpProxyAsync(GetBotApiUrl("bot/disconnect"));

            return NoContent();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost("connect")]
        public async Task<IActionResult> Connect()
        {
            await this.HttpProxyAsync(GetBotApiUrl("bot/connect"));

            return NoContent();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpGet("logs")]
        public Task GetLogs()
        {
            return this.HttpProxyAsync(GetBotApiUrl("bot/logs"));
        }

        private string GetBotApiUrl(string path)
        {
            var port = this.Request.Scheme == "https" ? _config.WebServerConfig.SecureBotApiPort : _config.WebServerConfig.BotApiPort;

            return $"{this.Request.Scheme}://{this.Request.Host.Host}:{port}/api/{path}";
        }
    }
}