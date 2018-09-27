using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

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
            return _configService.Config.Regions;
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

        [HttpPost("{id}")]
        public void Delete(int id)
        {
            _configService.RemoveRegion(id);
        }
    }
}
