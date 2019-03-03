using CoachBot.Domain.Services;
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
        private readonly ServerService _serverService;
        private readonly BotService _botService;

        public ServerController(ServerService serverService, BotService botService)
        {
            _serverService = serverService;
            _botService = botService;
        }

        [HttpGet("{id}")]
        public Server Get(int id)
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            return _serverService.GetServer(id);
        }

        [HttpGet]
        public IEnumerable<Server> GetAll()
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            return _serverService.GetServers();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            _serverService.RemoveServer(id);
        }

        [HttpPut]
        public void Update(Server server)
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            _serverService.UpdateServer(server);
        }

        [HttpPost]
        public void Add(Server server)
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            _serverService.AddServer(server);
        }
    }
}
