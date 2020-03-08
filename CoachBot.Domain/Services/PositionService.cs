using CoachBot.Database;
using CoachBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class PositionService
    {
        private readonly CoachBotContext _coachBotContext;

        public PositionService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public List<Position> GetPositions()
        {
            return _coachBotContext.Positions.Where(p => !Char.IsNumber(p.Name[0])).OrderBy(c => c.Name).ToList();
        }
    }
}
