using CoachBot.Database;
using CoachBot.Model;
using System.Linq;

namespace CoachBot.Domain.Repositories
{
    public class PlayerRepository
    {
        private readonly CoachBotContext _coachBotContext;

        public PlayerRepository(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public Player Get(int id)
        {
            return _coachBotContext.Players.FirstOrDefault(m => m.Id == id);
        }

        public void Add(Player player)
        {
            _coachBotContext.Players.Add(player);
            _coachBotContext.SaveChanges();
        }

        public void Update(Player player)
        {
            _coachBotContext.Players.Update(player);
            _coachBotContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var player = _coachBotContext.Players.First(m => m.Id == id);
            _coachBotContext.Players.Remove(player);
            _coachBotContext.SaveChanges();
        }
    }
}
