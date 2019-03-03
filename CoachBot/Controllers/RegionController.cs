using CoachBot.Domain.Services;
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
        private readonly RegionService _regionService;
        private readonly BotService _botService;

        public RegionController(RegionService regionService, BotService botService)
        {
            _regionService = regionService;
            _botService = botService;
        }

        [HttpGet]
        public IEnumerable<Region> Get()
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            return _regionService.GetRegions();
        }

        [HttpPost]
        public void Add(Region region)
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            _regionService.AddRegion(region);
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
            _regionService.RemoveRegion(id);
        }
    }
}
