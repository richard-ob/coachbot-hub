using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Model;
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
        [HttpPost]
        public void Create(Server server)
        {
            server.Name = server.Name.Trim();
            _serverService.AddServer(server);
        }
    }
}
