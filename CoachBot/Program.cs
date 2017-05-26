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
            _client.UserUpdated += UserOffline;

            _handler = new CommandHandler(serviceProvider);
            await _handler.ConfigureAsync();

            await Task.Delay(-1);
        }

        private Task UserOffline(SocketUser userPre, SocketUser userPost)
        {
            if (userPre.Status == UserStatus.Online && userPost.Status == UserStatus.Offline)
            {
                foreach (var channel in _matchmakerService.Channels)
                {
                    var player = channel.SignedPlayers.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                    if (player != null)
                    {
                        channel.Team1.Players.Remove(player);
                        channel.Team2.Players.Remove(player);
                        var textChannel = _client.GetChannel(channel.Id) as ITextChannel;
                        textChannel.SendMessageAsync($"Removed {player.DiscordUserMention ?? player.Name} from the line-up as they have gone offline");
                    }
                }
            }
            return Task.CompletedTask;
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
                        textChannel.SendMessageAsync("If you aren't putting in a shift in training, you aren't going to make the team! Sign up for a match!");
                        textChannel.SendMessageAsync("", embed: _matchmakerService.GenerateTeamList(channel.Id));
                        Console.WriteLine($"{channel.Name} on {server.Name}");
                    }
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