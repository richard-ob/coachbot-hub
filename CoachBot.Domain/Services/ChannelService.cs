using CoachBot.Database;
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
        private readonly Config _config;

        public ChannelService(Config config, CoachBotContext dbContext, DiscordSocketClient discordClient)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
            _config = config;
        }

        public void Update(Channel channel)
        {
            channel.UpdatedDate = DateTime.UtcNow;
            _dbContext.Update(channel);

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
                .Include(c => c.Region)
                .Include(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
                .ToList();
        }

        public List<Channel> GetChannelsByRegion(int regionId)
        {
            return GetChannels().Where(c => c.RegionId == regionId).ToList();
        }

        public List<Channel> GetChannelsForUser(ulong userId, bool unconfiguredChannels, bool hasAdmin = true)
        {
            var channels = new List<Channel>();
            foreach (var guild in _discordClient.Guilds.Where(g => g.Users.Any(u => u.Id == userId || userId == 166153339610857472)))
            {
                var userIsAdmin = guild.Users.FirstOrDefault(u => u.Id == userId)?.GuildPermissions.Administrator ?? false;
                if (userIsAdmin || (!hasAdmin && guild.Users.Any(u => u.Id == userId)) || userId == 166153339610857472)
                {
                    foreach (var channel in guild.TextChannels)
                    {
                        var existingChannel = GetChannelByDiscordId(channel.Id);
                        if (existingChannel != null && !unconfiguredChannels)
                        {
                            existingChannel.DiscordChannelName = channel.Name;
                            existingChannel.Region = _dbContext.Regions.FirstOrDefault(r => r.RegionId == existingChannel.RegionId);
                            channels.Add(existingChannel);
                        }
                        if (existingChannel is null && unconfiguredChannels)
                        {
                            var unconfiguredChannel = 
                                new Channel()
                                {
                                    DiscordChannelId = channel.Id,
                                    Name = channel.Name,
                                    Guild = new Guild()
                                    {
                                        Name = channel.Guild.Name,
                                        DiscordGuildId = channel.Guild.Id
                                    }
                                };
                            channels.Add(unconfiguredChannel);
                        }
                    }
                }
            }
            channels.Reverse();
            return channels;
        }

        public bool UserIsOwningGuildAdmin(ulong userId)
        {
            var owningGuild = _discordClient.Guilds.First(g => g.Id == _config.OwnerGuildId);
            return owningGuild.Users.FirstOrDefault(u => u.Id == userId).GuildPermissions.Administrator;
        }

        public bool ChannelHasPosition(ulong channelId, string position)
        {
            var channel = GetChannelByDiscordId(channelId);

            return channel != null && channel.ChannelPositions.Any(cp => position.ToUpper().StartsWith(cp.Position.Name.ToUpper()));
        }

        public Channel GetChannelByDiscordId(ulong id, bool withForeignKeys = true)
        {
            if (!withForeignKeys) return _dbContext.Channels.FirstOrDefault(c => c.DiscordChannelId == id);

            return _dbContext.Channels
                .Include(c => c.Region)
                .Include(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
                .Where(c => c.DiscordChannelId == id)
                .FirstOrDefault();
        }

        public Channel GetChannelByTeamCode(string teamCode)
        {
            return _dbContext.Channels
                .Include(c => c.Region)
                .Include(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
                .FirstOrDefault(c => c.TeamCode == teamCode);
        }
    }
}
