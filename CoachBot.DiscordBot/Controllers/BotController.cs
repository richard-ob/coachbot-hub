using CoachBot.Extensions;
using CoachBot.Services.Matchmaker;
using CoachBot.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BotController : Controller
    {
        private readonly BotService _botService;

        public BotController(BotService botService)
        {
            _botService = botService;
        }

        [HttpGet("state")]
        public IActionResult GetCurrentState()
        {
            if (!Request.IsLocal())
            {
                return Unauthorized();
            }

            return Ok(_botService.GetCurrentBotState());
        }

        [HttpPost("reconnect")]
        public async Task<IActionResult> Reconnect()
        {
            if (!Request.IsLocal())
            {
                return Unauthorized();
            }

            _botService.Reconnect();

            return NoContent();
        }

        [HttpPost("disconnect")]
        public async Task<IActionResult> Disconnect()
        {
            if (!Request.IsLocal())
            {
                return Unauthorized();
            }

            await _botService.Disconnect();

            return NoContent();
        }


        [HttpPost("connect")]
        public async Task<IActionResult> Connect()
        {
            if (!Request.IsLocal())
            {
                return Unauthorized();
            }

            await _botService.Connect();

            return NoContent();
        }

        [HttpGet("logs")]
        public string GetLogs()
        {
            var fileName = Directory.GetFiles(Environment.CurrentDirectory, "log-*.txt", SearchOption.TopDirectoryOnly).ToList().OrderByDescending(t => t).First();
            string log = "";
            using (FileStream fs = new FileStream(fileName,
                                     FileMode.Open,
                                     FileAccess.Read,
                                     FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (sr.Peek() >= 0)
                    {
                        log = sr.ReadLine() + Environment.NewLine + log;
                    }
                }
            }
            return log;
        }
    }
}