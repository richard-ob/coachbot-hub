using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Services;
using CoachBot.Shared.Model;
using CoachBot.Shared.Services;
using CoachBot.Tools;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoachBot.Bot
{
    public class BotInstance
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _client;
        private readonly DiscordRestClient _discordRestClient;
        private readonly CacheService _cacheService;
        private readonly Config _config;
        private CommandHandler _handler;

        public BotInstance(
            IServiceProvider serviceProvider,
            DiscordSocketClient client,
            CacheService cacheService,
            Config config,
            DiscordRestClient discordRestClient
        )
        {
            _serviceProvider = serviceProvider;
            _client = client;
            _cacheService = cacheService;
            _config = config;
            _discordRestClient = discordRestClient;
        }

        public async void Startup()
        {
            Console.WriteLine("Connecting..");
            await _discordRestClient.LoginAsync(TokenType.Bot, _config.DiscordConfig.BotToken);

            await _client.LoginAsync(TokenType.Bot, _config.DiscordConfig.BotToken);
            await _client.StartAsync();

            _client.Connected += Connected;
            _client.Disconnected += Disconnected;
            _client.Ready += BotReady;

            if (!_config.BotConfig.BotStealthMode)
            {
                await _client.SetGameAsync("IOSoccer", "http://iosoccer.com");
                _client.ChannelDestroyed += ChannelDestroyed;
                _client.LeftGuild += GuildDestroyed;
                _client.GuildMemberUpdated += (userPre, userPost) => { return UserUpdated(userPre, userPost); };
            }

            _handler = new CommandHandler(_serviceProvider);
            await _handler.ConfigureAsync();

            EnsureConnected();
        }

        private async void EnsureConnected()
        {
            Task.Delay(TimeSpan.FromMinutes(5)).Wait();

            if (_client.ConnectionState != ConnectionState.Connected || _client.LoginState != LoginState.LoggedIn)
            {
                Console.WriteLine("Attempting reconnection");
                await _client.LogoutAsync();
                await _client.LoginAsync(TokenType.Bot, _config.DiscordConfig.BotToken);
                await _client.StartAsync();
            }

            EnsureConnected();
        }

        private Task Connected()
        {
            Console.WriteLine("Connected!");
            return Task.CompletedTask;
        }

        private Task Disconnected(Exception arg)
        {
            Console.WriteLine("Disconnected");
            return Task.CompletedTask;
        }

        private Task BotReady()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var channelService = scope.ServiceProvider.GetService<ChannelService>();
                var channels = channelService.GetChannels();
                Console.WriteLine("Ready!");
                Console.WriteLine("Matchmaking in:");

                foreach (var server in _client.Guilds)
                {
                    foreach (var channel in server.Channels)
                    {
                        if (channels.Any(c => c.DiscordChannelId == channel.Id))
                        {
                            Console.WriteLine($"{channel.Name} on {server.Name}");
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        private Task GuildDestroyed(SocketGuild guild)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var channelService = scope.ServiceProvider.GetService<ChannelService>();
                var matchmakingGuild = channelService.GetChannels().Where(c => c.Team.Guild.DiscordGuildId == guild.Id);
                if (matchmakingGuild != null)
                {
                    Console.WriteLine($"Guild has been destroyed: {guild.Name}");
                    foreach (var matchmakingChannel in matchmakingGuild)
                    {
                        Console.WriteLine($"Channel has been destroyed: {matchmakingChannel.Team.Name} on {guild.Name}");
                        matchmakingChannel.Inactive = true;
                        channelService.UpdateChannel(matchmakingChannel);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private Task ChannelDestroyed(SocketChannel channel)
        {
            if (channel is SocketTextChannel textChannel)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var channelService = scope.ServiceProvider.GetService<ChannelService>();
                    var matchmakingChannel = channelService.GetChannelByDiscordId(channel.Id);
                    if (matchmakingChannel != null)
                    {
                        Console.WriteLine($"Channel has been destroyed: {textChannel.Name} on {textChannel.Guild.Name}");
                        matchmakingChannel.Inactive = true;
                        channelService.UpdateChannel(matchmakingChannel);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private Task UserUpdated(SocketGuildUser userPre, SocketGuildUser userPost)
        {
            if (userPost.Status.Equals(UserStatus.Online)) return Task.CompletedTask;

            var lastUserStatusCheck = (DateTime?)_cacheService.Get(CacheService.CacheItemType.LastUserStatusChangeCheck, userPost.Id.ToString());
            if (lastUserStatusCheck != null && lastUserStatusCheck.Value > DateTime.UtcNow.AddMinutes(-1)) return Task.CompletedTask;
            _cacheService.Set(CacheService.CacheItemType.LastUserStatusChangeCheck, userPost.Id.ToString(), DateTime.UtcNow);

            var playerSigned = false;
            using (var scope = _serviceProvider.CreateScope())
            {
                playerSigned = scope.ServiceProvider.GetService<MatchupService>().IsPlayerSigned(userPost.Id);
            }

            if (!playerSigned) return Task.CompletedTask;

            if (userPre.Status != UserStatus.Offline && userPost.Status == UserStatus.Offline) // User has gone offline
            {
                Task.Factory.StartNew(() => UserOffline(userPre, userPost));
            }

            if ((userPre.Status != UserStatus.AFK && userPre.Status != UserStatus.Idle) && (userPost.Status == UserStatus.AFK || userPost.Status == UserStatus.Idle)) // User has gone AFK/Idle
            {
                Task.Factory.StartNew(() => UserAway(userPre, userPost));
            }

            return Task.CompletedTask;
        }

        private async Task UserOffline(SocketGuildUser userPre, SocketGuildUser userPost)
        {
            await Task.Delay(TimeSpan.FromMinutes(5));
            var currentState = _discordRestClient.GetUserAsync(userPre.Id).Result;
            if (!currentState.Status.Equals(UserStatus.Offline)) return; // User is no longer offline

            using (var scope = _serviceProvider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger>();
                Console.WriteLine("Removing {userPost.Nickname} ({userPost.Username}) from possible lineups as they've gone offline");
                var matchupService = scope.ServiceProvider.GetService<MatchupService>();
                matchupService.RemovePlayerGlobally(userPre.Id, true);
            }
        }

        private async Task UserAway(SocketGuildUser userPre, SocketGuildUser userPost)
        {
            Task.Delay(TimeSpan.FromMinutes(15)).Wait(); // When user goes away, wait 15 minutes before notifying others
            var currentState = _discordRestClient.GetUserAsync(userPre.Id).Result;
            if (currentState.Status.Equals(UserStatus.Online)) return; // User is no longer AFK/Idle

            using (var scope = _serviceProvider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger>();
                Console.WriteLine($"Flagging {userPost.Nickname} ({userPost.Username}) as away in channels were player may be signed");

                var matchupService = scope.ServiceProvider.GetService<MatchupService>();
                foreach (var channel in matchupService.GetSignedChannelsForPlayer(userPost.Id))
                {
                    var matchup = matchupService.GetCurrentMatchupForChannel(channel.DiscordChannelId);
                    if (_client.GetChannel(channel.DiscordChannelId) is ITextChannel discordChannel)
                    {
                        if ((matchup.LineupHome.PlayerLineupPositions.Any(plp => plp.Player.DiscordUserId == userPost.Id) && matchup.LineupHome.ChannelId == channel.Id) 
                            || (matchup.LineupAway != null && matchup.LineupAway.PlayerLineupPositions.Any(plp => plp.Player.DiscordUserId == userPost.Id) && matchup.LineupAway.ChannelId == channel.Id))
                        {
                            var player = matchup.SignedPlayers.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                            await discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":clock1: {player.DisplayName} might be AFK. Keep your eyes peeled.").WithColor(new Color(254, 254, 254)).WithCurrentTimestamp().Build());
                        }

                        if ((matchup.LineupHome.PlayerSubstitutes.Any(ps => ps.Player.DiscordUserId == userPost.Id) && matchup.LineupHome.ChannelId == channel.Id) 
                            || (matchup.LineupAway != null && matchup.LineupAway.PlayerSubstitutes.Any(ps => ps.Player.DiscordUserId == userPost.Id) && matchup.LineupAway.ChannelId == channel.Id))
                        {
                            var sub = matchup.SignedSubstitutes.FirstOrDefault(s => s.DiscordUserId == userPost.Id);
                            await discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":clock1: {sub.DisplayName} might be AFK. Keep your eyes peeled.").WithColor(new Color(254, 254, 254)).WithCurrentTimestamp().Build());
                        }
                    }
                }
            }
        }
    }
}