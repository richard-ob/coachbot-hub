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

        public void UpdateChannel(Channel channel)
        {
            var existingChannel = channel;

            existingChannel.DisableSearchNotifications = channel.DisableSearchNotifications;
            existingChannel.DiscordChannelId = channel.DiscordChannelId;
            existingChannel.DiscordChannelName = channel.DiscordChannelName;
            existingChannel.DuplicityProtection = channel.DuplicityProtection;
            existingChannel.Formation = channel.Formation;
            existingChannel.Inactive = channel.Inactive;
            existingChannel.IsMixChannel = channel.IsMixChannel;
            existingChannel.SearchIgnoreList = channel.SearchIgnoreList;
            existingChannel.SubTeamName = channel.SubTeamName;
            existingChannel.UseClassicLineup = channel.UseClassicLineup;
            existingChannel.UpdatedDate = DateTime.Now;

            if (channel.ChannelPositions.GroupBy(cp => cp.Position.Name).Select(s => s.Count()).Max() >  1)
                throw new Exception("Positions must be unique");

            if (channel.ChannelPositions.GroupBy(cp => cp.Ordinal).Select(s => s.Count()).Max() > 1)
                throw new Exception("Position ordinals must be unique");

            if (!channel.ChannelPositions.Any())
                throw new Exception("No positions provided");

            if (channel.ChannelPositions.Any(cp => string.IsNullOrWhiteSpace(cp.Position.Name)))
                throw new Exception("No position name provided");

            // Remove deleted positions
            var deletedPositions = _dbContext.ChannelPositions
                .Where(c => c.ChannelId == channel.Id)
                .Where(cp => !channel.ChannelPositions.Any(cpt => cpt.PositionId == cp.PositionId));
            if (deletedPositions.Any())
            {
                _dbContext.ChannelPositions.RemoveRange(deletedPositions);
            }

            // Add new positions
            var newChannelPositions = channel.ChannelPositions.Where(c => c.PositionId <= 0);
            foreach(var newChannelPosition in newChannelPositions)
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
                    ChannelId = channel.Id,
                    PositionId = position.Id,
                    Ordinal = newChannelPosition.Ordinal
                };
                _dbContext.ChannelPositions.Add(channelPositionToAdd);
            }

            _dbContext.SaveChanges();
        }

        public void CreateChannel(Channel channel)
        {
            if (_dbContext.Channels.Any(c => c.DiscordChannelId == channel.DiscordChannelId))
                throw new Exception("A Discord channel can only be to one team");

            channel.UpdatedDate = DateTime.UtcNow;
            channel.CreatedDate = DateTime.UtcNow;
            _dbContext.Channels.Add(channel);
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

        public List<Channel> GetChannelsByRegion(int regionId)
        {
            return GetChannels().Where(c => c.Team.RegionId == regionId).ToList();
        }

        public MatchTeamType GetTeamTypeForChannelTeamType(ChannelTeamType channelTeamType, ulong channelId)
        {
            var match = _dbContext.GetCurrentMatchForChannel(channelId);

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
    }
}
