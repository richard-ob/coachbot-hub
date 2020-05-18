using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
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
                .Include(t => t.BadgeImage)
                .Single(t => t.Id == teamId);
        }

        public List<Team> GetTeams()
        {
            return _dbContext.Teams.ToList();
        }

        public void CreateTeam(Team team, ulong captainSteamUserId)
        {
            var player = _dbContext.GetPlayerBySteamId(captainSteamUserId);

            team.FoundedDate = team.FoundedDate ?? DateTime.Now;
            _dbContext.Teams.Add(team);

            var playerTeam = new PlayerTeam()
            {
                PlayerId = player.Id,
                TeamId = team.Id,
                TeamRole = TeamRole.Captain
            };
            _dbContext.PlayerTeams.Add(playerTeam);

            _dbContext.SaveChanges();
        }

        public void UpdateTeam(Team team)
        {
            team.UpdatedDate = DateTime.Now;
            _dbContext.Teams.Update(team);
            _dbContext.SaveChanges();
        }

        public void UpdateTeamGuildId(int teamId, int guildId)
        {
            var team = _dbContext.Teams.Single(t => t.Id == teamId);
            team.GuildId = guildId;
            _dbContext.SaveChanges();
        }

        public bool IsTeamCaptain(int teamId, ulong steamUserId)
        {
            var discordUserId = _dbContext.GetPlayerBySteamId(steamUserId).DiscordUserId;
            return _dbContext.PlayerTeams.Any(pt => pt.Player.DiscordUserId == discordUserId && pt.TeamRole == TeamRole.Captain && pt.LeaveDate == null);
        }

        public bool IsViceCaptain(int teamId, ulong steamUserId)
        {
            var discordUserId = _dbContext.GetPlayerBySteamId(steamUserId).DiscordUserId;
            return _dbContext.PlayerTeams.Any(pt => pt.Player.DiscordUserId == discordUserId && pt.TeamRole == TeamRole.ViceCaptain && pt.LeaveDate == null);
        }
    }
}
