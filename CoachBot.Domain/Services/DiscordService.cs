using CoachBot.Database;
using CoachBot.Domain.Model;
using CoachBot.Model;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoachBot.Domain.Services
{
    public class DiscordService
    {
        private readonly CoachBotContext _coachBotContext;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly Config _config;

        public DiscordService(CoachBotContext coachBotContext, DiscordSocketClient discordSocketClient, Config config)
        {
            _discordSocketClient = discordSocketClient;
            _coachBotContext = coachBotContext;
            _config = config;
        }

        public IEnumerable<DiscordChannel> GetChannelsForGuild(ulong guildId)
        {
            var guild = _discordSocketClient.GetGuild(guildId);

            return guild.Channels.Select(c => new DiscordChannel()
            {
                Id = c.Id,
                Name = c.Name,
                IsConfigured = _coachBotContext.Channels.Any(cc => cc.DiscordChannelId == c.Id)
            });
        }

        public IEnumerable<DiscordGuild> GetGuildsForUser(ulong userId)
        {
            return _discordSocketClient.Guilds
                .Where(g => g.Users.Any(u => u.Id == userId && u.GuildPermissions.Administrator))
                .Select(g => new DiscordGuild()
                {
                    Id = g.Id,
                    Name = g.Name,
                    IsLinked = _coachBotContext.Guilds.Any(cg => cg.DiscordGuildId == g.Id)
                });
        }

        public bool UserIsOwningGuildAdmin(ulong userId)
        {
            var owningGuild = _discordSocketClient.GetGuild(_config.OwnerGuildId);

            return owningGuild.Users.FirstOrDefault(u => u.Id == userId).GuildPermissions.Administrator;
        }

        public bool UserIsGuildAdministrator(ulong userId, ulong guildId)
        {
            return _discordSocketClient.GetGuild(guildId).Users.Any(u => u.Id == userId && u.GuildPermissions.Administrator);
        }

        public bool UserIsPresentOnGuild(ulong userId, ulong guildId)
        {
            return _discordSocketClient.GetGuild(guildId).Users.Any(u => u.Id == userId);
        }
    }
}
