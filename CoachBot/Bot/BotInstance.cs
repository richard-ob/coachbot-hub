using CoachBot.Domain.Services;
using CoachBot.Services;
using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoachBot.Bot
{
    public class BotInstance
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _client;
        private readonly ConfigService _configService;
        private readonly MatchmakingService _matchmakingService;
        private readonly ChannelService _channelService;
        private readonly MatchService _matchService;
        private readonly DiscordNotificationService _discordNotificationService;
        private CommandHandler _handler;
        private ulong _lastAfkCheckUser = 0;

        public BotInstance(
            IServiceProvider serviceProvider,
            DiscordSocketClient client,
            ConfigService configService,
            MatchmakingService matchmakingService,
            ChannelService channelService,
            MatchService matchService,
            DiscordNotificationService discordNotificationService
        )
        {
            _serviceProvider = serviceProvider;
            _client = client;
            _configService = configService;
            _matchmakingService = matchmakingService;
            _channelService = channelService;
            _matchService = matchService;
            _discordNotificationService = discordNotificationService;
            Startup();
        }

        public async void Startup()
        {
            Console.WriteLine("Connecting..");
            await _client.LoginAsync(TokenType.Bot, _configService.Config.BotToken);
            await _client.StartAsync();

            _client.Connected += Connected;
            _client.Disconnected += Disconnected;
            _client.Ready += BotReady;
            _client.ChannelDestroyed += ChannelDestroyed;
            _client.LeftGuild += GuildDestroyed;
            _client.GuildMemberUpdated += (userPre, userPost) => { return UserUpdated(userPre, userPost); };

            _handler = new CommandHandler(_serviceProvider);
            await _handler.ConfigureAsync();
        }

        private Task Connected()
        {
            Console.WriteLine("Connected!");
            return Task.CompletedTask;
        }

        private Task Disconnected(Exception arg)
        {
            Console.WriteLine("Disconnected");
            _client.LogoutAsync();
            _client.LoginAsync(TokenType.Bot, _configService.Config.BotToken);
            _client.StartAsync();

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

            return Task.CompletedTask;
        }

        private Task GuildDestroyed(SocketGuild guild)
        {
            var matchmakingGuild = _channelService.GetChannels().Where(c => c.Guild.DiscordGuildId == guild.Id);
            if (matchmakingGuild != null)
            {
                Console.WriteLine($"Guild has been destroyed: {guild.Name}");
                foreach(var matchmakingChannel in matchmakingGuild)
                {
                    Console.WriteLine($"Channel has been destroyed: {matchmakingChannel.Name} on {guild.Name}");
                    matchmakingChannel.Inactive = true;
                    _channelService.Update(matchmakingChannel);
                }
            }

            return Task.CompletedTask;
        }

        private Task ChannelDestroyed(SocketChannel channel)
        {
            if (channel is SocketTextChannel textChannel)
            {
                var matchmakingChannel = _channelService.GetChannelByDiscordId(channel.Id);
                if (matchmakingChannel != null)
                {
                    Console.WriteLine($"Channel has been destroyed: {textChannel.Name} on {textChannel.Guild.Name}");
                    matchmakingChannel.Inactive = true;
                    _channelService.Update(matchmakingChannel);
                }
            }

            return Task.CompletedTask;
        }

        public Task UserUpdated(SocketGuildUser userPre, SocketGuildUser userPost)
        {
            if (_lastAfkCheckUser == userPost.Id) return Task.CompletedTask;
            _lastAfkCheckUser = userPost.Id;
            if (userPost.Status.Equals(UserStatus.Online)) return Task.CompletedTask;

            var playerSigned = false;
            foreach (var channel in _channelService.GetChannels())
            {
                var match = _matchService.GetCurrentMatchForChannel(channel.DiscordChannelId);
                var player = match.SignedPlayersAndSubs.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                if (player != null)
                {
                    playerSigned = true;
                    break;
                }
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

        public Task UserOffline(SocketGuildUser userPre, SocketGuildUser userPost)
        {
            Task.Delay(TimeSpan.FromMinutes(10)).Wait();
            var currentState = _client.GetUser(userPre.Id);
            if (!currentState.Status.Equals(UserStatus.Offline)) return Task.CompletedTask; // User is no longer offline
            foreach (var channel in _channelService.GetChannels())
            {
                var match = _matchService.GetCurrentMatchForChannel(channel.DiscordChannelId);
                if (_client.GetChannel(channel.DiscordChannelId) is ITextChannel discordChannel)
                {
                    var player = match.SignedPlayers.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                    if (player != null)
                    {
                        discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":warning: Removed {player.DisplayName} from the line-up as they have gone offline").WithCurrentTimestamp().Build());
                        discordChannel.SendMessageAsync("", embed: _matchmakingService.RemovePlayer(channel.DiscordChannelId, userPre));
                        foreach (var teamEmbed in _matchmakingService.GenerateTeamList(channel.DiscordChannelId))
                        {
                            discordChannel.SendMessageAsync("", embed: teamEmbed);
                        }
                        if (player.DiscordUserId != null)
                        {
                            _discordNotificationService.SendUserMessage((ulong)player.DiscordUserId, $"You've been unsigned from the line-up in {discordChannel.Name} as you have gone offline. Sorry champ.");
                        }
                    }

                    var sub = match.SignedSubstitutes.FirstOrDefault(s => s.DiscordUserId == userPost.Id);
                    if (sub != null)
                    {
                        _discordNotificationService.SendChannelMessage(channel.DiscordChannelId, _matchmakingService.RemoveSub(channel.DiscordChannelId, userPre));
                        _discordNotificationService.SendChannelMessage(channel.DiscordChannelId, $":warning: Removed {sub.DisplayName} from the subs bench as they have gone offline");
                    }
                }
            }
            return Task.CompletedTask;
        }

        public Task UserAway(SocketGuildUser userPre, SocketGuildUser userPost)
        {
            Task.Delay(TimeSpan.FromMinutes(15)).Wait(); // When user goes away, wait 15 minutes before unsigning them
            var currentState = _client.GetUser(userPre.Id);
            if (currentState.Status.Equals(UserStatus.Online)) return Task.CompletedTask; // User is no longer AFK/Idle

            foreach (var channel in _channelService.GetChannels())
            {
                var match = _matchService.GetCurrentMatchForChannel(channel.DiscordChannelId);
                if (_client.GetChannel(channel.DiscordChannelId) is ITextChannel discordChannel)
                {
                    var player = match.SignedPlayers.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                    if (player != null)
                    {
                        discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":clock1: {player.DisplayName} might be AFK. Keep your eyes peeled.").WithCurrentTimestamp().Build());
                    }

                    var sub = match.SignedSubstitutes.FirstOrDefault(s => s.DiscordUserId == userPost.Id);
                    if (sub != null)
                    {
                        discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":clock1: {sub.DisplayName} might be AFK. Keep your eyes peeled.").WithCurrentTimestamp().Build());
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
