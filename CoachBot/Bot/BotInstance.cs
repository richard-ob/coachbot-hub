using CoachBot.Services;
using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace CoachBot.Bot
{
    public class BotInstance
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _client;
        private readonly ConfigService _configService;
        private CommandHandler _handler;
        private ulong _lastAfkCheckUser = 0;

        public BotInstance(IServiceProvider serviceProvider, DiscordSocketClient client, ConfigService configService)
        {
            _serviceProvider = serviceProvider;
            _client = client;
            _configService = configService;
            Startup();
        }

        public async void Startup()
        {
            Console.WriteLine("Connecting..");
            await _client.LoginAsync(TokenType.Bot, _configService.Config.BotToken);
            await _client.StartAsync();

            _client.Connected += Connected;
            _client.Ready += BotReady;
            _client.ChannelDestroyed += ChannelDestroyed;

            _handler = new CommandHandler(_serviceProvider);
            await _handler.ConfigureAsync();
        }

        private Task Connected()
        {
            Console.WriteLine("Connected!");
            return Task.CompletedTask;
        }

        private Task BotReady()
        {
            Console.WriteLine("Ready!");
            Console.WriteLine("Matchmaking in:");
            return Task.CompletedTask;
        }

        private Task ChannelDestroyed(SocketChannel channel)
        {
            var textChannel = channel as SocketTextChannel;
            if (textChannel != null)
            {
                Console.WriteLine($"Channel has been destroyed: {textChannel.Name} on {textChannel.Guild.Name}");
            }
            return Task.CompletedTask;
        }
        
    }
}
