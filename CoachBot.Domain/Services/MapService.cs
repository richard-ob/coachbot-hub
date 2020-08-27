using CoachBot.Database;
using CoachBot.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoachBot.Domain.Services
{
    public class MapService
    {
        private readonly CoachBotContext _coachBotContext;

        public MapService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public List<Map> GetAllMaps()
        {
            return _coachBotContext.Maps.ToList();
        }
    }
}
