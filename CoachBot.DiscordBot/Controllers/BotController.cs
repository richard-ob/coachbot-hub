using CoachBot.Extensions;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

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
        public IActionResult Reconnect()
        {
            if (!Request.IsLocal())
            {
                return Unauthorized();
            }

            _botService.Reconnect();

            return Ok();
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