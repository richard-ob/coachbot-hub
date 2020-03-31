using CoachBot.Database;
using CoachBot.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
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

        public void AddPlayerToTeam(int teamId, int playerId, TeamRole teamRole)
        {
            var playerTeam = new PlayerTeam()
            {
                PlayerId = playerId,
                TeamId = teamId,
                TeamRole = teamRole,
                CreatedDate = DateTime.Now
            };

            var team = _dbContext.Teams.Single(t => t.Id == teamId);       
            
            if (team.TeamType == TeamType.Club && _dbContext.PlayerTeams.Any(pt => pt.LeaveDate == null && pt.PlayerId == playerId && pt.Team.TeamType == TeamType.Club))
            {
                throw new Exception("A player cannot belong to two club teams at once");
            }

            if (_dbContext.PlayerTeams.Any(pt => pt.TeamId == teamId && pt.PlayerId == playerId && pt.LeaveDate == null))
            {
                throw new Exception("Player already belongs to this team");
            }

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
