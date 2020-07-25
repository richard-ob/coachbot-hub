using AspNetCore.Proxy;
using CoachBot.Domain.Model;
using CoachBot.Model;
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
            return this.ProxyAsync($"{this.Request.Scheme}://{this.Request.Host.Host}:{_config.BotApiPort}/api/bot/state");
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost("reconnect")]
        public async Task<IActionResult> Reconnect()
        {
            await this.ProxyAsync($"{this.Request.Scheme}://{this.Request.Host.Host}:{_config.BotApiPort}/api/bot/reconnect");

            return NoContent();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost("disconnect")]
        public async Task<IActionResult> Disconnect()
        {
            await this.ProxyAsync($"{this.Request.Scheme}://{this.Request.Host.Host}:{_config.BotApiPort}/api/bot/disconnect");

            return NoContent();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost("connect")]
        public async Task<IActionResult> Connect()
        {
            await this.ProxyAsync($"{this.Request.Scheme}://{this.Request.Host.Host}:{_config.BotApiPort}/api/bot/connect");

            return NoContent();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpGet("logs")]
        public Task GetLogs()
        {
            return this.ProxyAsync($"{this.Request.Scheme}://{this.Request.Host.Host}:{_config.BotApiPort}/api/bot/logs");
        }
    }
}