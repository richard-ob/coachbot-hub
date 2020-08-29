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
        private readonly DiscordService _discordService;

        public TeamService(CoachBotContext dbContext, DiscordService discordService)
        {
            _dbContext = dbContext;
            _discordService = discordService;
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
                .OrderBy(t => t.TeamType).ThenBy(t => t.Name)
                .ToList();
        }

        public void CreateTeam(Team team, ulong captainSteamUserId, string token)
        {
            var player = _dbContext.GetPlayerBySteamId(captainSteamUserId);

            if (!_dbContext.Regions.Any(r => r.RegionId == team.RegionId && r.CreateTeamToken == token))
            {
                throw new Exception("Invalid token provided for team creation");
            }

            if (_dbContext.Teams.Any(t => t.Id != team.Id && t.RegionId == team.RegionId && t.TeamCode == team.TeamCode))
            {
                throw new Exception("A team code must be unique for a region");
            }

            if (team.BadgeImageId.HasValue)
            {
                var emoteName = $"{team.TeamCode}_{team.RegionId}";
                var badgeImage = _dbContext.AssetImages.Single(i => i.Id == team.BadgeImageId);
                team.BadgeEmote = _discordService.CreateEmote(emoteName, badgeImage.Base64EncodedImage);
            }

            team.FoundedDate = team.FoundedDate ?? DateTime.UtcNow;
            team.Inactive = false;
            _dbContext.Teams.Add(team);

            var playerTeam = new PlayerTeam()
            {
                PlayerId = player.Id,
                TeamId = team.Id,
                TeamRole = TeamRole.Captain,
                IsPending = false,
                JoinDate = DateTime.UtcNow
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

            if (existingTeam.BadgeImageId != team.BadgeImageId && team.BadgeImageId.HasValue || existingTeam.RegionId != team.RegionId || existingTeam.TeamCode != team.TeamCode)
            {
                var emoteName = $"{team.TeamCode}_{team.RegionId}";
                var badgeImage = _dbContext.AssetImages.Single(i => i.Id == team.BadgeImageId);
                if (existingTeam.BadgeImageId != team.BadgeImageId && existingTeam.BadgeEmote != null)
                {
                    _discordService.DeleteEmote(existingTeam.BadgeEmote);
                }
                existingTeam.BadgeEmote = _discordService.CreateEmote(emoteName, badgeImage.Base64EncodedImage);
            }

            if (!existingTeam.Inactive && team.Inactive)
            {
                var playerTeams = _dbContext.PlayerTeams.Where(pt => pt.LeaveDate == null && pt.TeamId == existingTeam.Id);
                foreach(var playerTeam in playerTeams)
                {
                    playerTeam.LeaveDate = DateTime.UtcNow;
                }
            }

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
            existingTeam.UpdatedDate = DateTime.UtcNow;

            _dbContext.SaveChanges();
        }

        public void DeleteTeam(int teamId)
        {
            if (_dbContext.TeamMatchStatistics.Any(t => t.TeamId == teamId))
            {
                throw new Exception("Cannot delete teams who have played matches. Please mark as inactive.");
            }

            _dbContext.PlayerTeams.RemoveRange(_dbContext.PlayerTeams.Where(pt => pt.TeamId == teamId));
            _dbContext.SaveChanges();

            _dbContext.Channels.RemoveRange(_dbContext.Channels.Where(c => c.TeamId == teamId));
            _dbContext.SaveChanges();

            _dbContext.Teams.Remove(_dbContext.Teams.Single(t => t.Id == teamId));
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