using CoachBot.Database;
using CoachBot.Domain.Model;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Repositories
{
    public class MatchRepository
    {
        private readonly CoachBotContext _coachBotContext;

        public MatchRepository(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public Match Get(int id)
        {
            return _coachBotContext.Matches.FirstOrDefault(m => m.Id == id);
        }

        public List<Match> GetAll()
        {
            return _coachBotContext.Matches.ToList();
        }

        public void Add(Match match)
        {
            _coachBotContext.Matches.Add(match);
            _coachBotContext.SaveChanges();
        }

        public void Update(Match match)
        {
            _coachBotContext.Matches.Update(match);
            _coachBotContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var match = _coachBotContext.Matches.First(m => m.Id == id);
            _coachBotContext.Matches.Remove(match);
            _coachBotContext.SaveChanges();
        }
    }
}
