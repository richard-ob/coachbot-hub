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

        public Team GetTeam(string teamCode, int regionId)
        {
            return _dbContext.Teams
                .Include(t => t.Guild)
                .Include(t => t.Channels)
                    .ThenInclude(c => c.ChannelPositions)
                .Include(t => t.Region)
                .Include(t => t.BadgeImage)
                .FirstOrDefault(t => t.TeamCode == teamCode && t.RegionId == regionId);
        }

        public List<Team> GetTeams(int regionId, TeamType? teamType = null)
        {
            return _dbContext.Teams
                .Include(t => t.BadgeImage)
                .Where(t => t.RegionId == regionId)
                .Where(t => teamType == null || t.TeamType == teamType)
                .ToList();
        }

        public void CreateTeam(Team team, ulong captainSteamUserId)
        {
            var player = _dbContext.GetPlayerBySteamId(captainSteamUserId);

            if (_dbContext.Teams.Any(t => t.Id != team.Id && t.RegionId == team.RegionId && t.TeamCode == team.TeamCode))
            {
                throw new Exception("A team code must be unique for a region");
            }

            team.FoundedDate = team.FoundedDate ?? DateTime.Now;
            _dbContext.Teams.Add(team);

            var playerTeam = new PlayerTeam()
            {
                PlayerId = player.Id,
                TeamId = team.Id,
                TeamRole = TeamRole.Captain,
                IsPending = false,
                JoinDate = DateTime.Now
            };
            _dbContext.PlayerTeams.Add(playerTeam);

            _dbContext.SaveChanges();
        }

        public void UpdateTeam(Team team)
        {
            var existingTeam = _dbContext.Teams.Single(t => t.Id == team.Id);

            if (_dbContext.Teams.Any(t => t.Id != team.Id && t.RegionId == team.RegionId && t.TeamCode == team.TeamCode))
            {
                throw new Exception("A team code must be unique for a region");
            }

            existingTeam.BadgeEmote = team.BadgeEmote;
            existingTeam.BadgeImageId = team.BadgeImageId;
            existingTeam.Color = team.Color;
            existingTeam.FoundedDate = team.FoundedDate;
            existingTeam.Inactive = team.Inactive;
            existingTeam.KitEmote = team.KitEmote;
            existingTeam.Name = team.Name;
            existingTeam.RegionId = team.RegionId;
            existingTeam.TeamCode = team.TeamCode;
            existingTeam.TeamType = team.TeamType;
            existingTeam.GuildId = team.GuildId;
            existingTeam.UpdatedDate = DateTime.Now;

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
