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

        public RegionController(ConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet]
        public IEnumerable<Region> Get()
        {
            var regions = _configService.Config.Regions;
            regions.ForEach(r => r.ServerCount = _configService.Config.Servers.Count(s => s.RegionId == r.RegionId));
            return regions;
        }

        [HttpPost]
        public void Add(Region region)
        {
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
            _configService.RemoveRegion(id);
        }
    }
}
