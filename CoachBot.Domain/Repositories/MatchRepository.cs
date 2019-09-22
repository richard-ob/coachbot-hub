using CoachBot.Database;
using CoachBot.Model;
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

      /*  public List<Match> GetAll()
        {
            var matches = _coachBotContext.Matches.ToList();

            return matches;
        }

        public void Add(Match match)
        {
            _coachBotContext.Matches.Add(match);
            _coachBotContext.SaveChanges();
        }*/
    }
}
