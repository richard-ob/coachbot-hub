using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using CoachBot.Model;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class ChannelService
    {
        private readonly CoachBotContext _dbContext;
        private readonly DiscordSocketClient _discordClient;

        public ChannelService(CoachBotContext dbContext, DiscordSocketClient discordClient)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
        }

        public void CreateChannel(Channel channel)
        {
            if (_dbContext.Channels.Any(c => c.DiscordChannelId == channel.DiscordChannelId))
            {
                throw new Exception("A Discord channel can only be attached to one team");
            }

            if (channel.DiscordChannelId < 1)
            {
                throw new Exception("A valid Discord channel ID must be provided");
            }

            if (string.IsNullOrWhiteSpace(channel.SubTeamCode) && _dbContext.Channels.Any(c => c.TeamId == channel.TeamId && c.ChannelPositions.Count == channel.ChannelPositions.Count))
            {
                throw new Exception("Secondary channels must have a sub team code");
            }

            if (!string.IsNullOrWhiteSpace(channel.SubTeamCode)
                && _dbContext.Channels.Any(c => c.SubTeamCode.ToUpper() == channel.SubTeamCode.ToUpper() && c.TeamId == channel.TeamId && c.ChannelPositions.Count == channel.ChannelPositions.Count))
            {
                throw new Exception("Secondary channels must have a unique sub team code");
            }

            var channelPositions = channel.ChannelPositions;

            channel.ChannelPositions = null;
            channel.Team = null;
            channel.UpdatedDate = DateTime.UtcNow;
            channel.SubTeamCode = channel.SubTeamCode?.ToUpper();
            _dbContext.Channels.Add(channel);
            _dbContext.SaveChanges();

            UpdatePositions(channelPositions, channel.Id);
            _dbContext.SaveChanges();
        }

        public void UpdateChannel(Channel channel)
        {
            var existingChannel = _dbContext.Channels.Find(channel.Id);

            if (string.IsNullOrWhiteSpace(channel.SubTeamCode) && _dbContext.Channels.Any(c => c.TeamId == channel.TeamId && c.TeamId == channel.TeamId && channel.Id != c.Id && c.ChannelPositions.Count == channel.ChannelPositions.Count && string.IsNullOrWhiteSpace(c.SubTeamCode)))
            {
                throw new Exception("Secondary channels must have a sub team code");
            }

            if (!string.IsNullOrWhiteSpace(channel.SubTeamCode)
                && _dbContext.Channels.Any(c => c.SubTeamCode.ToUpper() == channel.SubTeamCode.ToUpper() && c.TeamId == channel.TeamId && channel.Id != c.Id && c.ChannelPositions.Count == channel.ChannelPositions.Count))
            {
                throw new Exception("Secondary channels must have a unique sub team code");
            }

            existingChannel.DisableSearchNotifications = channel.DisableSearchNotifications;
            existingChannel.DiscordChannelId = channel.DiscordChannelId;
            existingChannel.DiscordChannelName = channel.DiscordChannelName;
            existingChannel.DuplicityProtection = channel.DuplicityProtection;
            existingChannel.Formation = channel.Formation;
            existingChannel.Inactive = channel.Inactive;
            existingChannel.ChannelType = channel.ChannelType;
            existingChannel.SearchIgnoreList = channel.SearchIgnoreList;
            existingChannel.SubTeamCode = channel.SubTeamCode?.ToUpper();
            existingChannel.SubTeamName = channel.SubTeamName;
            existingChannel.UseClassicLineup = channel.UseClassicLineup;
            existingChannel.UpdatedDate = DateTime.UtcNow;

            UpdatePositions(channel.ChannelPositions, channel.Id);

            _dbContext.SaveChanges();
        }

        public List<Channel> GetChannels()
        {
            return _dbContext.Channels
                .Include(c => c.Team)
                .ThenInclude(t => t.Region)
                .Include(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
                .ToList();
        }

        public List<Channel> GetChannelsForGuild(ulong discordGuildId)
        {
            return _dbContext.Channels
                .Include(c => c.Team)
                .ThenInclude(t => t.Region)
                .Include(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
                .Where(c => c.Team.Guild.DiscordGuildId == discordGuildId)
                .ToList();
        }

        public List<Channel> GetChannelsForTeamGuild(ulong discordGuildId, int teamId)
        {
            var channels = _dbContext.Channels
                .Include(c => c.Team)
                .ThenInclude(t => t.Region)
                .Include(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
                .Where(c => c.Team.Guild.DiscordGuildId == discordGuildId && c.TeamId == teamId)
                .ToList();

            return channels;
        }

        public List<Channel> GetChannelsByRegion(int regionId)
        {
            return GetChannels().Where(c => c.Team.RegionId == regionId).ToList();
        }

        public MatchTeamType GetTeamTypeForChannelTeamType(ChannelTeamType channelTeamType, ulong channelId)
        {
            var match = _dbContext.GetCurrentMatchupForChannel(channelId);

            if (match.IsMixMatch && channelTeamType == ChannelTeamType.TeamOne)
            {
                return MatchTeamType.Home;
            }
            else if (match.IsMixMatch && channelTeamType == ChannelTeamType.TeamTwo)
            {
                return MatchTeamType.Away;
            }
            else if (match.LineupHome.Channel.DiscordChannelId == channelId)
            {
                return MatchTeamType.Home;
            }
            else
            {
                return MatchTeamType.Away;
            }
        }

        public bool ChannelHasPosition(ulong channelId, string position, ChannelTeamType channelTeamType = ChannelTeamType.TeamOne)
        {
            var channel = GetChannelByDiscordId(channelId);

            if (channel == null) return false;

            if (channelTeamType == ChannelTeamType.TeamTwo) return channel.ChannelPositions.Any(cp => position.ToUpper().Equals($"{cp.Position.Name.ToUpper()}2"));

            return channel.ChannelPositions.Any(cp => position.ToUpper().Equals(cp.Position.Name.ToUpper()));
        }

        public Channel GetChannel(int id)
        {
            return _dbContext.Channels
                  .Include(c => c.Team)
                      .ThenInclude(t => t.Region)
                  .Include(c => c.ChannelPositions)
                      .ThenInclude(cp => cp.Position)
                  .FirstOrDefault(c => c.Id == id);
        }

        public Channel GetChannelByDiscordId(ulong id, bool withForeignKeys = true)
        {
            if (!withForeignKeys) return _dbContext.Channels.FirstOrDefault(c => c.DiscordChannelId == id);

            return _dbContext.Channels
                .Include(c => c.Team)
                    .ThenInclude(t => t.Region)
                .Include(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
                .Where(c => c.DiscordChannelId == id)
                .FirstOrDefault();
        }

        public Channel GetChannelByTeamCode(string teamCode)
        {
            return _dbContext.Channels
                .Include(c => c.Team)
                    .ThenInclude(t => t.Region)
                .Include(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
                .FirstOrDefault(c => c.Team.TeamCode == teamCode);
        }

        public Channel GetChannelBySearchTeamCode(string searchTeamCode, MatchFormat format)
        {
            return _dbContext.Channels
                .Include(c => c.Team)
                    .ThenInclude(t => t.Region)
                .Include(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
                .Where(c => c.ChannelPositions.Count == (int)format)
                .FirstOrDefault(c => c.Team.TeamCode + (c.SubTeamCode ?? "") == searchTeamCode);
        }

        public bool ChannelExists(ulong channelId)
        {
            return _dbContext.Channels.Any(c => c.DiscordChannelId == channelId);
        }

        private void UpdatePositions(ICollection<ChannelPosition> channelPositions, int channelId)
        {
            if (channelPositions.GroupBy(cp => cp.Position.Name).Select(s => s.Count()).Max() > 1)
                throw new Exception("Positions must be unique");

            if (channelPositions.GroupBy(cp => cp.Ordinal).Select(s => s.Count()).Max() > 1)
                throw new Exception("Position ordinals must be unique");

            if (!channelPositions.Any())
                throw new Exception("No positions provided");

            if (channelPositions.Any(cp => string.IsNullOrWhiteSpace(cp.Position.Name)))
                throw new Exception("No position name provided");

            // Remove deleted positions
            var deletedPositions = _dbContext.ChannelPositions
                .Where(c => c.ChannelId == channelId)
                .Where(cp => !channelPositions.Any(cpt => cpt.PositionId == cp.PositionId));
            if (deletedPositions.Any())
            {
                _dbContext.ChannelPositions.RemoveRange(deletedPositions);
            }

            // Add new positions
            var newChannelPositions = channelPositions.Where(c => c.PositionId <= 0);
            foreach (var newChannelPosition in newChannelPositions)
            {
                var position = _dbContext.Positions.FirstOrDefault(p => p.Name.ToUpper() == newChannelPosition.Position.Name.ToUpper());
                if (position == null)
                {
                    position = new Position()
                    {
                        Name = newChannelPosition.Position.Name
                    };
                    _dbContext.Positions.Add(position);
                }
                var channelPositionToAdd = new ChannelPosition()
                {
                    ChannelId = channelId,
                    PositionId = position.Id,
                    Ordinal = newChannelPosition.Ordinal
                };
                _dbContext.ChannelPositions.Add(channelPositionToAdd);
            }
        }

    }
}