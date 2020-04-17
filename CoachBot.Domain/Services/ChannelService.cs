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

        public void Update(Channel channel)
        {
            channel.UpdatedDate = DateTime.UtcNow;
            _dbContext.Update(channel);

            if (channel.ChannelPositions.Any(cp => channel.ChannelPositions.Any(cpd => cp.Position.Name == cpd.Position.Name && cp.PositionId != cpd.PositionId)))
                throw new Exception("Positions must be unique");

            if (!channel.ChannelPositions.Any())
                throw new Exception("No positions provided");

            var deletedPositions = _dbContext.ChannelPositions
                .Where(c => c.ChannelId == channel.Id)
                .Where(cp => !channel.ChannelPositions.Any(cpt => cpt.PositionId == cp.PositionId));
            if (deletedPositions.Any()) _dbContext.ChannelPositions.RemoveRange(deletedPositions);

            _dbContext.SaveChanges();
        }

        public void Create(Channel channel)
        {
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
