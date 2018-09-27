using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServerController : Controller
    {
        private readonly ConfigService _configService;

        public ServerController(ConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet("{id}")]
        public Server Get(int id)
        {
            return _configService.Config.Servers[id];
        }

        [HttpGet]
        public IEnumerable<Server> GetAll()
        {
            return _configService.GetServers();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _configService.RemoveServer(id);
        }

        [HttpPost]
        public void Add(Server server)
        {
            _configService.AddServer(server);
        }
    }
}
