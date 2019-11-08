using CoachBot.Database;
using CoachBot.Domain.Repositories;
using CoachBot.Model;
using Discord;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class ServerService
    {
        private readonly CoachBotContext _coachBotContext;

        public ServerService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public List<Server> GetServers()
        {
            return _coachBotContext.Servers.Include(s => s.Region).ToList();
        }

        public List<Server> GetServersByRegion(int regionId)
        {
            return _coachBotContext.Servers.Where(s => s.RegionId == regionId).ToList();
        }

        public Server GetServer(int id)
        {
            return _coachBotContext.Servers
                .Include(r => r.Region)
                .FirstOrDefault(s => s.Id == id);
        }

        public void AddServer(Server server)
        {
            _coachBotContext.Servers.Add(server);
            _coachBotContext.SaveChanges();
        }

        public void UpdateServer(Server server)
        {
            _coachBotContext.Servers.Update(server);
            _coachBotContext.SaveChanges();
        }

        public void RemoveServer(int id)
        {
            var server = _coachBotContext.Servers.First(s => s.Id == id);
            _coachBotContext.Servers.Remove(server);
            _coachBotContext.SaveChanges();
        }
    }
}
