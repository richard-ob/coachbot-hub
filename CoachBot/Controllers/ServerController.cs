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
    public class ServerController : Controller
    {
        private readonly ConfigService _configService;
        private readonly BotService _botService;

        public ServerController(ConfigService configService, BotService botService)
        {
            _configService = configService;
            _botService = botService;
        }

        [HttpGet("{id}")]
        public Server Get(int id)
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            return _configService.Config.Servers[id];
        }

        [HttpGet]
        public IEnumerable<Server> GetAll()
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            return _configService.GetServers();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            _configService.RemoveServer(id);
        }

        [HttpPost]
        public void Add(Server server)
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            _configService.AddServer(server);
        }
    }
}
