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
                LogLevel = LogSeverity.Debug,
            });

            var serviceProvider = ConfigureServices();
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));

            await _client.LoginAsync(TokenType.Bot, config.BotToken);
            await _client.StartAsync();

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
                .AddSingleton<MatchmakerService>();
            var provider = services.BuildServiceProvider();

            // Autowire and create these dependencies now
            provider.GetService<LogAdaptor>();

            return provider;
        }
    }
}