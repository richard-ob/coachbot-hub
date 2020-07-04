using CoachBot.Database;
using CoachBot.Domain.Model;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class CountryService
    {
        private readonly CoachBotContext _coachBotContext;

        public CountryService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public List<Country> GetCountries()
        {
            return _coachBotContext.Countries.OrderBy(c => c.Name).ToList();
        }
    }
}