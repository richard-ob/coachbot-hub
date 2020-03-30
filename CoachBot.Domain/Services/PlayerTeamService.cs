using CoachBot.Database;
using CoachBot.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class PlayerTeamService
    {
        private readonly CoachBotContext _dbContext;

        public PlayerTeamService(CoachBotContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(PlayerTeam playerTeam)
        {
            // TODO: Validation logic here to prevent someone being in multiple teams (Not on load etc)
            _dbContext.PlayerTeams.Add(playerTeam);
            _dbContext.SaveChanges();
        }

        public void Update(PlayerTeam playerTeam)
        {
            _dbContext.PlayerTeams.Update(playerTeam);
            _dbContext.SaveChanges();
        }

        public List<PlayerTeam> GetForPlayer(int playerId, bool includeInactive = false)
        {
            return _dbContext.PlayerTeams
                .Include(pt => pt.Team)
                    .ThenInclude(t => t.Region)
                .Include(pt => pt.Team)
                    .ThenInclude(t => t.Guild)
                .Include(pt => pt.Player)
                .Where(pt => pt.Player.Id == playerId)
                .Where(pt => pt.LeaveDate == null || includeInactive)
                .AsNoTracking()
                .ToList();
        }

        public List<PlayerTeam> GetForTeam(int teamId, bool includeInactive = false)
        {
            return _dbContext.PlayerTeams
                .Where(pt => pt.TeamId == teamId)
                .Where(pt => pt.LeaveDate == null || includeInactive)
                .Include(pt => pt.Player)
                .AsNoTracking()
                .ToList();
        }
    }
}
