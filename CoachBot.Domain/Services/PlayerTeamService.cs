﻿using CoachBot.Database;
using CoachBot.Domain.Model;
using CoachBot.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class PlayerTeamService
    {
        private readonly CoachBotContext _dbContext;
        private readonly MatchStatisticsService _matchStatisticsService;

        public PlayerTeamService(CoachBotContext dbContext, MatchStatisticsService matchStatisticsService)
        {
            _dbContext = dbContext;
            _matchStatisticsService = matchStatisticsService;
        }

        public void AddPlayerToTeam(int teamId, int playerId, TeamRole teamRole)
        {
            var playerTeam = new PlayerTeam()
            {
                PlayerId = playerId,
                TeamId = teamId,
                TeamRole = teamRole,
                JoinDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            };

            var team = _dbContext.Teams.Single(t => t.Id == teamId);

            if (team.TeamType == TeamType.Club && _dbContext.PlayerTeams.Any(pt => pt.LeaveDate == null && pt.PlayerId == playerId && pt.Team.TeamType == TeamType.Club && pt.TeamRole != TeamRole.Loaned))
            {
                throw new Exception("A player cannot belong to two club teams at once, unless a loan has been arranged (loan & loanee roles)");
            }

            if (_dbContext.PlayerTeams.Any(pt => pt.TeamId == teamId && pt.PlayerId == playerId && pt.LeaveDate == null))
            {
                throw new Exception("Player already belongs to this team");
            }

            _dbContext.PlayerTeams.Add(playerTeam);
            _dbContext.SaveChanges();
        }

        public void Update(PlayerTeam playerTeam, Player player, bool hasCaptainPermissions)
        {
            var current = _dbContext.PlayerTeams.Single(pt => pt.Id == playerTeam.Id);

            if (current.TeamId != playerTeam.TeamId)
            {
                throw new ArgumentException("Team cannot be changed in this manner");
            }

            if (current.TeamRole != playerTeam.TeamRole && !hasCaptainPermissions)
            {
                throw new UnauthorizedAccessException("You must be a captain or vice captain to change roles");
            }

            var currentCaptainCount = _dbContext.PlayerTeams.Count(pt => pt.LeaveDate == null && pt.TeamRole == TeamRole.Captain);
            if (current.TeamRole != playerTeam.TeamRole && current.TeamRole == TeamRole.Captain && currentCaptainCount == 1)
            {
                throw new UnauthorizedAccessException("You cannot remove yourself as a captain of a team without closing the team");
            }

            if ((current.PlayerId == player.Id || player.HubRole >= PlayerHubRole.Administrator) && !playerTeam.IsPending && current.IsPending)
            {
                current.IsPending = false;
            }

            if (playerTeam.LeaveDate != null && current.JoinDate > playerTeam.LeaveDate)
            {
                throw new Exception("Leave date cannot be before join date");
            }

            current.LeaveDate = playerTeam.LeaveDate;
            current.TeamRole = playerTeam.TeamRole;

            _dbContext.PlayerTeams.Update(current);
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
                .AsQueryable()
                .Where(pt => pt.TeamId == teamId)
                .Where(pt => pt.LeaveDate == null || includeInactive)
                .Include(pt => pt.Player)
                .AsNoTracking()
                .ToList();
        }
    }
}