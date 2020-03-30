using CoachBot.Domain.Services;
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
    public class ServerController : Controller
    {
        private readonly ServerService _serverService;
        private readonly ChannelService _channelService;

        public ServerController(ServerService serverService, ChannelService channelService)
        {
            _serverService = serverService;
            _channelService = channelService;
        }

        [HttpGet("{id}")]
        public Server Get(int id)
        {
            if (!_channelService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            return _serverService.GetServer(id);
        }

        [HttpGet]
        public IEnumerable<Server> GetAll()
        {
            if (!_channelService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            return _serverService.GetServers();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            if (!_channelService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            _serverService.RemoveServer(id);
        }

        [HttpPut]
        public void Update(Server server)
        {
            if (!_channelService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            _serverService.UpdateServer(server);
        }

        [HttpPost]
        public void Create(Server server)
        {
            if (!_channelService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            server.Name = server.Name.Trim();
            _serverService.AddServer(server);
        }
    }
}
