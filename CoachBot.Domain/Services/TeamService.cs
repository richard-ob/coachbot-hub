using CoachBot.Database;
using CoachBot.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class TeamService
    {
        private readonly CoachBotContext _dbContext;

        public TeamService(CoachBotContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Team GetTeam(int teamId)
        {
            return _dbContext.Teams
                .Include(t => t.Guild)
                .Include(t => t.Channels)
                    .ThenInclude(c => c.ChannelPositions)
                .Include(t => t.Region)
                .Single(t => t.Id == teamId);
        }

        public List<Team> GetTeams()
        {
            return _dbContext.Teams.ToList();
        }
    }
}
