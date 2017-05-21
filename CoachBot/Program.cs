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

        private async Task RunAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            var serviceProvider = ConfigureServices();

            var configService = serviceProvider.GetService<ConfigService>();
            var matchmakerService = serviceProvider.GetService<MatchmakerService>();
            Console.WriteLine("Connecting..");
            await _client.LoginAsync(TokenType.Bot, configService.config.BotToken);
            await _client.StartAsync();
            _client.Connected += () =>
            {
                Console.WriteLine("Connected!");
                return Task.CompletedTask;
            };
            _client.Ready += () => 
            {
                Console.WriteLine("Ready!");
                foreach (var server in _client.Guilds)
                {
                    foreach (var channel in server.Channels)
                    {
                        var textChannel = _client.GetChannel(channel.Id) as ITextChannel;
                        if (textChannel != null && configService.config.Channels.Any(c => c.Id == channel.Id))
                        {
                            textChannel.SendMessageAsync("If you aren't putting in a shift in training, you aren't going to make the team! Sign up for a match!");
                            textChannel.SendMessageAsync(matchmakerService.GenerateTeamList(channel.Id));
                        }
                    }
                }
                return Task.CompletedTask;
            };

            _client.UserLeft += (SocketGuildUser user) =>
            {
                Console.WriteLine(user.Nickname);
                Console.WriteLine(user.Username);
                return Task.CompletedTask;
            };

            _handler = new CommandHandler(serviceProvider);
            await _handler.ConfigureAsync();

            await Task.Delay(-1);
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
                .AddSingleton<MatchmakerService>();
            var provider = services.BuildServiceProvider();

            // Autowire and create these dependencies now
            provider.GetService<LogAdaptor>();
            provider.GetService<ConfigService>();

            return provider;
        }
    }
}