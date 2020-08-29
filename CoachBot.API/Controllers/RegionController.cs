using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Services;
using CoachBot.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : Controller
    {
        private readonly RegionService _regionService;

        public RegionController(RegionService regionService)
        {
            _regionService = regionService;
        }

        [HttpGet]
        public IEnumerable<RegionDto> Get()
        {
            return _regionService.GetRegions();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost]
        public void Add(Region region)
        {
            _regionService.Add(region);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost("{id}")]
        public void Update(Region region)
        {
            throw new NotImplementedException();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _regionService.Delete(id);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Owner)]
        [HttpPost("{id}/regenerate-token")]
        public void RegenerateAuthorizationToken(int id)
        {
            _regionService.RegenerateAuthorizationToken(id);
        }
    }
}