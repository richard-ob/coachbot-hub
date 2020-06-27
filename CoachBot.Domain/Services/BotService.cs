using CoachBot.Database;
using CoachBot.Domain.Model;
using CoachBot.Model;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.IO;

namespace CoachBot.Services.Matchmaker
{
    public class BotService
    {
        private DiscordSocketClient _client;
        private Config _config;
        private CoachBotContext _dbContext;

        public BotService(DiscordSocketClient client, CoachBotContext dbContext)
        {
            _client = client;
            _dbContext = dbContext;
            _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));
        }

        public BotState GetCurrentBotState()
        {
            var botState = new BotState() {
                ConnectionStatus = _client.ConnectionState.ToString(),
                LoginStatus = _client.LoginState.ToString()
            };

            return botState;
        }

        public void Reconnect()
        {
            _client.LogoutAsync();
            _client.LoginAsync(TokenType.Bot, _config.BotToken);
            _client.StartAsync();
        }

        public void LeaveGuild(string id)
        {
            var parsedId = ulong.Parse(id);
            _client.GetGuild(parsedId).LeaveAsync();
        }
    }
}
