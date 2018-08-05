using CoachBot.Model;
using Discord;
using Discord.WebSocket;
using System.Linq;

namespace CoachBot.Services.Matchmaker
{
    public class BotStateService
    {
        private DiscordSocketClient _client;
        private ConfigService _configService;

        public BotStateService(DiscordSocketClient client, ConfigService configService)
        {
            _client = client;
            _configService = configService;
        }

        public BotState GetCurrentBotState()
        {
            var botState = new BotState() {
                ConnectionStatus = _client.ConnectionState.ToString(),
                LoginStatus = _client.LoginState.ToString(),
                Guilds = _client.Guilds.Select(g => new Guild() { Name = g.Name, Id = g.Id.ToString() }).ToList()
            };

            return botState;
        }

        public void Reconnect()
        {
            _client.LogoutAsync();
            _client.LoginAsync(TokenType.Bot, _configService.Config.BotToken);
            _client.StartAsync();
        }

        public void LeaveGuild(string id)
        {
            var parsedId = ulong.Parse(id);
            _client.GetGuild(parsedId).LeaveAsync();
        }
    }
}
