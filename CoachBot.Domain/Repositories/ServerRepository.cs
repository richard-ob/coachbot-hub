using CoachBot.Database;
using CoachBot.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Repositories
{
    public class ServerRepository
    {
        private readonly CoachBotContext _coachBotContext;

        public ServerRepository(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public List<Server> GetAll()
        {
            return _coachBotContext.Servers.Include(r => r.Region).ToList();
        }

        public Server Get(int id)
        {
            return _coachBotContext.Servers
                .Include(r => r.Region)
                .FirstOrDefault(s => s.Id == id);
        }

        public void Add(Server server)
        {
            _coachBotContext.Servers.Add(server);
            _coachBotContext.SaveChanges();
        }

        public void Update(Server server)
        {
            _coachBotContext.Servers.Update(server);
            _coachBotContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var server = _coachBotContext.Servers.First(s => s.Id == id);
            _coachBotContext.Servers.Remove(server);
            _coachBotContext.SaveChanges();
        }
    }
}
