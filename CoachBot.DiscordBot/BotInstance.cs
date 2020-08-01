using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.LegacyImporter;
using CoachBot.Services;
using CoachBot.Shared.Model;
using CoachBot.Shared.Services;
using CoachBot.Tools;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoachBot.Bot
{
    public class BotInstance
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _client;
        private readonly MatchmakingService _matchmakingService;
        private readonly ChannelService _channelService;
        private readonly DiscordNotificationService _discordNotificationService;
        private readonly CacheService _cacheService;
        private readonly Importer _importer;
        private readonly Config _config;
        private CommandHandler _handler;

        public BotInstance(
            IServiceProvider serviceProvider,
            DiscordSocketClient client,
            MatchmakingService matchmakingService,
            ChannelService channelService,
            DiscordNotificationService discordNotificationService,
            CacheService cacheService,
            //Importer importer,
            Config config
        )
        {
            _serviceProvider = serviceProvider;
            _client = client;
            _matchmakingService = matchmakingService;
            _channelService = channelService;
            _discordNotificationService = discordNotificationService;
            _cacheService = cacheService;
            //_importer = importer;
            _config = config;
        }

        public async void Startup()
        {
            Console.WriteLine("Connecting..");
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
            var channels = _channelService.GetChannels();
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
            //_importer.GetTeams();

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
            Task.Delay(TimeSpan.FromMinutes(10)).Wait();
            var currentState = _client.GetUser(userPre.Id);
            if (!currentState.Status.Equals(UserStatus.Offline)) return; // User is no longer offline

            using (var scope = _serviceProvider.CreateScope())
            {
                foreach (var channel in scope.ServiceProvider.GetService<ChannelService>().GetChannels())
                {
                    var matchup = scope.ServiceProvider.GetService<MatchupService>().GetCurrentMatchupForChannel(channel.DiscordChannelId);
                    if (_client.GetChannel(channel.DiscordChannelId) is ITextChannel discordChannel)
                    {
                        var player = matchup.SignedPlayers.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                        if (player != null)
                        {
                            await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed($"Removed {player.DisplayName} from the line-up as they have gone offline", ServiceResponseStatus.Warning));
                            await _discordNotificationService.SendAuditChannelMessage(_matchmakingService.RemovePlayer(channel.DiscordChannelId, player.Id));
                            foreach (var teamEmbed in scope.ServiceProvider.GetService<MatchmakingService>().GenerateTeamList(channel.DiscordChannelId))
                            {
                                await discordChannel.SendMessageAsync("", embed: teamEmbed);
                            }
                            if (player.DiscordUserId != null)
                            {
                                await _discordNotificationService.SendUserMessage((ulong)player.DiscordUserId, $"You've been unsigned from the line-up in **#{discordChannel.Name} ({discordChannel.Guild.Name})** as you've gone offline. Sorry champ.");
                            }
                        }

                        var sub = matchup.SignedSubstitutes.FirstOrDefault(s => s.DiscordUserId == userPost.Id);
                        if (sub != null)
                        {
                            await _discordNotificationService.SendChannelMessage(channel.DiscordChannelId, _matchmakingService.RemoveSub(channel.DiscordChannelId, userPre));
                            await _discordNotificationService.SendChannelMessage(channel.DiscordChannelId, embed: DiscordEmbedHelper.GenerateEmbed($"Removed {sub.DisplayName} from the subs bench as they have gone offline", ServiceResponseStatus.Warning));
                        }
                    }
                }
            }

        }

        private async Task UserAway(SocketGuildUser userPre, SocketGuildUser userPost)
        {
            Task.Delay(TimeSpan.FromMinutes(15)).Wait(); // When user goes away, wait 15 minutes before notifying others
            var currentState = _client.GetUser(userPre.Id);
            if (currentState.Status.Equals(UserStatus.Online)) return; // User is no longer AFK/Idle

            using (var scope = _serviceProvider.CreateScope())
            {
                foreach (var channel in scope.ServiceProvider.GetService<ChannelService>().GetChannels())
                {
                    var matchup = scope.ServiceProvider.GetService<MatchupService>().GetCurrentMatchupForChannel(channel.DiscordChannelId);
                    if (_client.GetChannel(channel.DiscordChannelId) is ITextChannel discordChannel)
                    {
                        var player = matchup.SignedPlayers.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                        if (player != null)
                        {
                            await discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":clock1: {player.DisplayName} might be AFK. Keep your eyes peeled.").WithColor(new Color(254, 254, 254)).WithCurrentTimestamp().Build());
                        }

                        var sub = matchup.SignedSubstitutes.FirstOrDefault(s => s.DiscordUserId == userPost.Id);
                        if (sub != null)
                        {
                            await discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":clock1: {sub.DisplayName} might be AFK. Keep your eyes peeled.").WithColor(new Color(254, 254, 254)).WithCurrentTimestamp().Build());
                        }
                    }
                }
            }
        }
    }
}