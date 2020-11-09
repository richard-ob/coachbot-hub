using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using CoachBot.Shared.Model;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoachBot.Domain.Services
{
    public class DiscordService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly DiscordRestClient _discordRestClient;
        private readonly Config _config;

        public DiscordService(IServiceProvider serviceProvider, DiscordSocketClient discordSocketClient, DiscordRestClient discordRestClient, Config config)
        {
            _serviceProvider = serviceProvider;
            _discordSocketClient = discordSocketClient;
            _discordRestClient = discordRestClient;
            _config = config;
        }

        public string CreateEmote(string emoteName, string image)
        {
            var guild = _discordSocketClient.GetGuild(_config.DiscordConfig.OwnerGuildId) as SocketGuild;
            var bytes = Convert.FromBase64String(image.Split(',')[1]); // Remove base64 header info
            var emote = guild.CreateEmoteAsync(emoteName, new Image(new MemoryStream(bytes))).Result;

            return $"<:{emote.Name}:{emote.Id}>";
        }

        public void DeleteEmote(string emoteString)
        {
            try
            {
                var emoteId = ulong.Parse(emoteString.Split(':')[2].Replace(">", ""));
                var guild = _discordSocketClient.GetGuild(_config.DiscordConfig.OwnerGuildId) as SocketGuild;
                var emote = guild.GetEmoteAsync(emoteId).Result;
                guild.DeleteEmoteAsync(emote).Wait();
            }
            catch
            {
                // TODO: Figure out if our error handling will let this fail gracefully..
            }
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

        public DiscordUser GetDiscordUser(ulong userId)
        {
            var user = _discordRestClient.GetUserAsync(userId).Result;

            if (user == null)
            {
                return null;
            }

            return new DiscordUser()
            {
                Id = user.Id,
                Name = user.Username,
                AvatarUrl = user.GetAvatarUrl()
            };
        }

        public IEnumerable<DiscordChannel> GetChannelsForGuild(ulong guildId)
        {
            IEnumerable<DiscordChannel> channels;
            using (var scope = _serviceProvider.CreateScope())
            {
                var coachBotContext = scope.ServiceProvider.GetService<CoachBotContext>();
                var guild = _discordRestClient.GetGuildAsync(guildId).Result;

                channels = guild.GetTextChannelsAsync().Result.Select(c => new DiscordChannel()
                {
                    Id = c.Id,
                    Name = c.Name
                });

                // HACK: If we do this in the original projection, the context seems to have been disposed/not available in the calling scope..
                var matchmakingChannels = coachBotContext.Channels.Where(cc => cc.Team.Guild.DiscordGuildId == guildId).ToList();
                return channels.Select(c => new DiscordChannel()
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsConfigured = matchmakingChannels.Any(cc => cc.DiscordChannelId == c.Id)
                });
            }
        }

        public IEnumerable<DiscordGuild> GetGuildsForUser(ulong steamUserId)
        {
            var guilds = new List<DiscordGuild>();
            using (var scope = _serviceProvider.CreateScope())
            {
                var coachBotContext = scope.ServiceProvider.GetService<CoachBotContext>();

                var discordUserId = coachBotContext.Players.Single(s => s.SteamID == steamUserId).DiscordUserId;
                if (discordUserId == null) return new List<DiscordGuild>();

                foreach(var guild in _discordRestClient.GetGuildsAsync().Result)
                {
                    var guildUser = guild.GetUserAsync((ulong)discordUserId).Result;
                    if (guildUser != null && guildUser.GuildPermissions.Administrator)
                    {
                        var guildWithUserIsAdmin = new DiscordGuild()
                        {
                            Id = guild.Id,
                            Name = guild.Name,
                            Emotes = guild.Emotes.Select(e => new KeyValuePair<string, string>($"<:{e.Name}:{e.Id}>", e.Url)).ToList()
                        };
                        guilds.Add(guildWithUserIsAdmin);
                    }
                }

                // HACK: If we do this in the original projection, the context seems to have been disposed/not available in the calling scope..
                var matchmakingGuilds = coachBotContext.Guilds.Where(g => guilds.Any(dg => dg.Id == g.DiscordGuildId)).ToList();
                return guilds.Select(dg => new DiscordGuild()
                {
                    Id = dg.Id,
                    Name = dg.Name,
                    Emotes = dg.Emotes,
                    IsLinked = matchmakingGuilds.Any(g => g.DiscordGuildId == dg.Id)
                });
            }
        }

        public bool UserIsGuildAdministrator(ulong steamUserId, ulong guildId)
        {
            ulong? discordUserId;
            using (var scope = _serviceProvider.CreateScope())
            {
                var coachBotContext = scope.ServiceProvider.GetService<CoachBotContext>();

                discordUserId = coachBotContext.GetPlayerBySteamId(steamUserId).DiscordUserId;
            }

            return _discordSocketClient.GetGuild(guildId).Users.Any(u => u.Id == discordUserId && u.GuildPermissions.Administrator);
        }

        public bool UserIsPresentOnGuild(ulong steamUserId, ulong guildId)
        {
            ulong? discordUserId;
            using (var scope = _serviceProvider.CreateScope())
            {
                var coachBotContext = scope.ServiceProvider.GetService<CoachBotContext>();

                discordUserId = coachBotContext.GetPlayerBySteamId(steamUserId).DiscordUserId;
            }

            return _discordSocketClient.GetGuild(guildId).Users.Any(u => u.Id == steamUserId);
        }

        public async void StartPersistentConnection()
        {
            if (_discordSocketClient.ConnectionState != ConnectionState.Connected || _discordSocketClient.LoginState != LoginState.LoggedIn)
            {
                await _discordRestClient.LogoutAsync();
                await _discordRestClient.LoginAsync(TokenType.Bot, _config.DiscordConfig.BotToken);

                await _discordSocketClient.LogoutAsync();
                await _discordSocketClient.LoginAsync(TokenType.Bot, _config.DiscordConfig.BotToken);
                await _discordSocketClient.StartAsync();
            }

            Task.Delay(TimeSpan.FromMinutes(5)).Wait();
            StartPersistentConnection();
        }
    }
}
