using CoachBot.Database;
using CoachBot.Model;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Repositories
{
    public class ChannelRepository
    {
        private readonly CoachBotContext _coachBotContext;

        public ChannelRepository(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public List<Channel> GetAll()
        {
            var channels = _coachBotContext.Channels.ToList();

            return channels;
        }

        public Channel Get(int id)
        {
            return _coachBotContext.Channels.FirstOrDefault(s => s.RegionId == id);
        }

        public void Add(Channel channel)
        {
            _coachBotContext.Channels.Add(channel);
            _coachBotContext.SaveChanges();
        }

        public void Update(Channel channel)
        {
            _coachBotContext.Channels.Update(channel);
            _coachBotContext.SaveChanges();
        }

        public void Delete(ulong id)
        {
            var channel = _coachBotContext.Channels.First(c => c.Id == id);
            _coachBotContext.Channels.Remove(channel);
            _coachBotContext.SaveChanges();
        }
    }
}
