using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Model;
using CoachBot.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServerController : Controller
    {
        private readonly ServerService _serverService;

        public ServerController(ServerService serverService)
        {
            _serverService = serverService;
        }

        [HttpGet("{id}")]
        public Server Get(int id)
        {
            return _serverService.GetServer(id);
        }

        [HttpGet]
        public IEnumerable<Server> GetAll()
        {
            return _serverService.GetServers();
        }

        [HttpGet("deactivated")]
        public IEnumerable<Server> GetDeactivatedServers()
        {
            return _serverService.GetDeactivatedServers();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Manager)]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _serverService.RemoveServer(id);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Manager)]
        [HttpPatch("{id}")]
        public void UpdateRconPassword(int id, [FromForm]string rconPassword)
        {
            _serverService.UpdateServerRconPassword(id, rconPassword);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Manager)]
        [HttpPatch("{id}/reactivate")]
        public void ReactivateServer(int id)
        {
            _serverService.ReactivateServer(id);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Manager)]
        [HttpPut]
        public void Update(Server server)
        {
            _serverService.UpdateServer(server);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Manager)]
        [HttpPost]
        public void Create(CreateServerDto serverDto)
        {
            var server = new Server()
            {
                Address = serverDto.Address.Trim(),
                CountryId = serverDto.CountryId,
                Name = serverDto.Name.Trim(),
                RegionId = serverDto.RegionId,
                RconPassword = serverDto.RconPassword?.Trim()
            };

            _serverService.AddServer(server);
        }
    }
}