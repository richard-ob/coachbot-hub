using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Services;
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
        private readonly ChannelService _channelService;

        public RegionController(RegionService regionService, ChannelService channelService)
        {
            _regionService = regionService;
            _channelService = channelService;
        }

        [HttpGet]
        public IEnumerable<RegionDto> Get()
        {
            if (!_channelService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            return _regionService.GetRegions();
        }

        [HttpPost]
        public void Add(Region region)
        {
            if (!_channelService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
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
            if (!_channelService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            _regionService.RemoveRegion(id);
        }
    }
}
