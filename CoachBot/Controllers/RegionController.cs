using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Model;
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
        private readonly DiscordService _discordService;

        public RegionController(RegionService regionService, DiscordService discordService)
        {
            _regionService = regionService;
            _discordService = discordService;
        }

        [HttpGet]
        public IEnumerable<RegionDto> Get()
        {
            if (!_discordService.UserIsOwningGuildAdmin(User.GetDiscordUserId()))
            {
                throw new Exception();
            }
            return _regionService.GetRegions();
        }

        [HttpPost]
        public void Add(Region region)
        {
            if (!_discordService.UserIsOwningGuildAdmin(User.GetDiscordUserId()))
            {
                throw new Exception();
            }
            _regionService.Add(region);
        }

        [HttpPost("{id}")]
        public void Update(Region region)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            if (!_discordService.UserIsOwningGuildAdmin(User.GetDiscordUserId()))
            {
                throw new Exception();
            }
            _regionService.Delete(id);
        }
    }
}
