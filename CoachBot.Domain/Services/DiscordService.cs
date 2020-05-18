using CoachBot.Database;
using CoachBot.Domain.Extensions;
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

        public DiscordGuild GetDiscordGuild(ulong guildId)
        {
            var guild = _discordSocketClient.GetGuild(guildId);

            if (guild == null)
            {
                throw new Exception("No access to this Discord guild");
            }

            return new DiscordGuild()
            {
                Name = guild.Name,
                Id = guild.Id
            };
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

        public IEnumerable<DiscordGuild> GetGuildsForUser(ulong steamUserId)
        {
            var discordUserId = _coachBotContext.Players.Single(s => s.SteamID == steamUserId).DiscordUserId;
            return _discordSocketClient.Guilds
                .Where(g => g.Users.Any(u => u.Id == discordUserId && u.GuildPermissions.Administrator))
                .Select(g => new DiscordGuild()
                {
                    Id = g.Id,
                    Name = g.Name,
                    IsLinked = _coachBotContext.Guilds.Any(cg => cg.DiscordGuildId == g.Id),
                    Emotes = g.Emotes.Select(e => new KeyValuePair<string, string>($"<:{e.Name}:{e.Id}>", e.Url)).ToList()
                });
        }

        public bool UserIsOwningGuildAdmin(ulong steamUserId)
        {
            var player = _coachBotContext.GetPlayerBySteamId(steamUserId);
            var owningGuild = _discordSocketClient.GetGuild(_config.OwnerGuildId);

            return owningGuild.Users.FirstOrDefault(u => u.Id == player.DiscordUserId).GuildPermissions.Administrator;
        }

        public bool UserIsGuildAdministrator(ulong steamUserId, ulong guildId)
        {
            var discordUserId = _coachBotContext.GetPlayerBySteamId(steamUserId).DiscordUserId;
            return _discordSocketClient.GetGuild(guildId).Users.Any(u => u.Id == discordUserId && u.GuildPermissions.Administrator);
        }

        public bool UserIsPresentOnGuild(ulong steamUserId, ulong guildId)
        {
            var discordUserId = _coachBotContext.GetPlayerBySteamId(steamUserId).DiscordUserId;
            return _discordSocketClient.GetGuild(guildId).Users.Any(u => u.Id == steamUserId);
        }
    }
}
