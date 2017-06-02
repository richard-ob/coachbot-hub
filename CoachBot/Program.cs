using Discord;
using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using System;
using System.Threading.Tasks;
using CoachBot.Services.Logging;
using CoachBot.Services.Matchmaker;
using System.Linq;
using CoachBot.Model;

namespace CoachBot
{
    internal class Program
    {
        private static void Main(string[] args) =>
            new Program().RunAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandHandler _handler;
        private MatchmakerService _matchmakerService;
        private ConfigService _configService;

        private ulong _lastAfkCheckUser = 0;

        private async Task RunAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            var serviceProvider = ConfigureServices();

            _configService = serviceProvider.GetService<ConfigService>();
            _matchmakerService = serviceProvider.GetService<MatchmakerService>();
            Console.WriteLine("Connecting..");
            await _client.LoginAsync(TokenType.Bot, _configService.Config.BotToken);
            await _client.StartAsync();
            _client.Connected += () =>
            {
                Console.WriteLine("Connected!");
                return Task.CompletedTask;
            };
            _client.Ready += BotReady;
            _client.GuildMemberUpdated += (userPre, userPost) => {
                if (_lastAfkCheckUser == userPost.Id) return Task.CompletedTask;
                _lastAfkCheckUser = userPost.Id;
                if (userPost.Status.Equals(UserStatus.Online)) return Task.CompletedTask;
                var playerSigned = false;
                foreach (var channel in _matchmakerService.Channels)
                {
                    var player = channel.SignedPlayers.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                    if (player == null) player = channel.Team1.Substitutes.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                    if (player != null)
                    {
                        playerSigned = true;
                    }
                }
                if (!playerSigned) return Task.CompletedTask;
                if (userPre.Status != UserStatus.Offline && userPost.Status == UserStatus.Offline)
                {
                    Task.Factory.StartNew(() => UserOffline(userPre, userPost));
                }

                if ((userPre.Status != UserStatus.AFK && userPre.Status != UserStatus.Idle) && (userPost.Status == UserStatus.AFK || userPost.Status == UserStatus.Idle))
                {
                    Task.Factory.StartNew(() => UserAway(userPre, userPost));
                }
                return Task.CompletedTask;
            };

            _handler = new CommandHandler(serviceProvider);
            await _handler.ConfigureAsync();

            await Task.Delay(-1);
        }

        private Task BotReady()
        {
            Console.WriteLine("Ready!");
            Console.WriteLine("Matchmaking in:");
            foreach (var server in _client.Guilds)
            {
                foreach (var channel in server.Channels)
                {
                    var textChannel = _client.GetChannel(channel.Id) as ITextChannel;
                    if (textChannel != null && _configService.Config.Channels.Any(c => c.Id == channel.Id))
                    {
                        textChannel.SendMessageAsync("If you aren't putting in a shift in during the week, you aren't going to make the team this weekend! Sign up for a match!");
                        textChannel.SendMessageAsync("", embed: _matchmakerService.GenerateTeamList(channel.Id));
                        if (_matchmakerService.Channels.First(c => c.Id == textChannel.Id).Team2.IsMix)
                        {
                            textChannel.SendMessageAsync("", embed: _matchmakerService.GenerateTeamList(textChannel.Id, Teams.Team2));
                        }
                        Console.WriteLine($"{channel.Name} on {server.Name}");
                    }
                }
            }
            return Task.CompletedTask;
        }

        public Task UserOffline(SocketGuildUser userPre, SocketGuildUser userPost)
        {
            Task.Delay(TimeSpan.FromMinutes(10)).Wait();
            var currentState = _client.GetUser(userPre.Id);
            if (!currentState.Status.Equals(UserStatus.Offline)) return Task.CompletedTask; // User is no longer offline
            foreach (var channel in _matchmakerService.Channels)
            {
                var player = channel.SignedPlayers.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                var textChannel = _client.GetChannel(channel.Id) as ITextChannel;
                if (player != null)
                {
                    textChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":warning: Removed {player.DiscordUserMention ?? player.Name} from the line-up as they have gone offline").WithCurrentTimestamp().Build());
                    textChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(_matchmakerService.RemovePlayer(channel.Id, userPre)).WithCurrentTimestamp().Build());
                    textChannel.SendMessageAsync("", embed: _matchmakerService.GenerateTeamList(channel.Id));
                    if (_matchmakerService.Channels.First(c => c.Id == channel.Id).Team2.IsMix)
                    {
                        textChannel.SendMessageAsync("", embed: _matchmakerService.GenerateTeamList(channel.Id, Teams.Team2));
                    }
                    if (player.DiscordUserId != null)
                    {
                        var dmChannel = _client.GetUser((ulong)player.DiscordUserId).CreateDMChannelAsync() as IDMChannel;
                        dmChannel.SendMessageAsync($"You've been unsigned from the line-up in {textChannel.Name} as you have gone offline. Sorry champ.");
                    }
                }
                var sub = channel.Team1.Substitutes.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                if (sub != null)
                {
                    textChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(_matchmakerService.RemoveSub(channel.Id, userPre)).WithCurrentTimestamp().Build());
                    textChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":warning: Removed {player.DiscordUserMention ?? player.Name} from the subs bench as they have gone offline").WithCurrentTimestamp().Build());
                }
            }
            return Task.CompletedTask;
        }

        public Task UserAway(SocketGuildUser userPre, SocketGuildUser userPost)
        {
            Task.Delay(TimeSpan.FromMinutes(15)).Wait();
            var currentState = _client.GetUser(userPre.Id);
            if (currentState.Status.Equals(UserStatus.Online)) return Task.CompletedTask; // User is no longer AFK/Idle
            foreach (var channel in _matchmakerService.Channels)
            {
                var player = channel.SignedPlayers.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                var sub = channel.Team1.Substitutes.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                if (player != null)
                {
                    var textChannel = _client.GetChannel(channel.Id) as ITextChannel;
                    textChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":clock1: {player.DiscordUserMention ?? player.Name} might be AFK. Keep your eyes peeled.").WithCurrentTimestamp().Build());
                }
                if (sub != null)
                {
                    var textChannel = _client.GetChannel(channel.Id) as ITextChannel;
                    textChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":clock1: {sub.DiscordUserMention ?? sub.Name} might be AFK. Keep your eyes peeled.").WithCurrentTimestamp().Build());
                }
            }
            return Task.CompletedTask;
        }

        private IServiceProvider ConfigureServices()
        {
            // Configure logging
            var logger = LogAdaptor.CreateLogger();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new SerilogLoggerProvider(logger));
            // Configure services
            var services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false, ThrowOnError = false}))
                .AddSingleton(logger)
                .AddSingleton<LogAdaptor>()
                .AddSingleton<InteractiveService>()
                .AddSingleton<ConfigService>()
                .AddSingleton<MatchmakerService>()
                .AddSingleton<StatisticsService>();
            var provider = services.BuildServiceProvider();

            // Autowire and create these dependencies now
            provider.GetService<LogAdaptor>();
            provider.GetService<ConfigService>();

            return provider;
        }
    }
}