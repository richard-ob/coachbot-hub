using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
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
    }
}
