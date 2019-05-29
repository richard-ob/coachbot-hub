using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegionController : Controller
    {
        private readonly ConfigService _configService;
        private readonly BotService _botService;

        public RegionController(ConfigService configService, BotService botService)
        {
            _configService = configService;
            _botService = botService;
        }

        [HttpGet]
        public IEnumerable<Region> Get()
        {
            return _configService.GetRegions();
        }

        [HttpPost]
        public void Add(Region region)
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            _configService.AddRegion(region);
        }

        [HttpPost("{id}")]
        public void Update(Region region)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            _configService.RemoveRegion(id);
        }
    }
}
