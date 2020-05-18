using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Model;
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
    public class ServerController : Controller
    {
        private readonly ServerService _serverService;
        private readonly DiscordService _discordService;

        public ServerController(ServerService serverService, DiscordService discordService)
        {
            _serverService = serverService;
            _discordService = discordService;
        }

        [HttpGet("{id}")]
        public Server Get(int id)
        {
            if (!_discordService.UserIsOwningGuildAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            return _serverService.GetServer(id);
        }

        [HttpGet]
        public IEnumerable<Server> GetAll()
        {
            if (!_discordService.UserIsOwningGuildAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            return _serverService.GetServers();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            if (!_discordService.UserIsOwningGuildAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            _serverService.RemoveServer(id);
        }

        [HttpPut]
        public void Update(Server server)
        {
            if (!_discordService.UserIsOwningGuildAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            _serverService.UpdateServer(server);
        }

        [HttpPost]
        public void Create(Server server)
        {
            if (!_discordService.UserIsOwningGuildAdmin(User.GetSteamId()))
            {
                throw new Exception();
            }
            server.Name = server.Name.Trim();
            _serverService.AddServer(server);
        }
    }
}
