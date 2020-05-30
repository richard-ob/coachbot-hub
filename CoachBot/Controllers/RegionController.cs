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
        private readonly PlayerService _playerService;

        public RegionController(RegionService regionService, PlayerService playerService)
        {
            _regionService = regionService;
            _playerService = playerService;
        }

        [HttpGet]
        public IEnumerable<RegionDto> Get()
        {
            return _regionService.GetRegions();
        }

        [HttpPost]
        public void Add(Region region)
        {
            if (!_playerService.IsAdmin(User.GetSteamId()))
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
            if (!_playerService.IsAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            _regionService.Delete(id);
        }
    }
}
