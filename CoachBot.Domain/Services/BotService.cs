﻿using CoachBot.Database;
using CoachBot.Model;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CoachBot.Services.Matchmaker
{
    public class BotService
    {
        private DiscordSocketClient _client;
        private Config _config;

        public BotService(DiscordSocketClient client)
        {
            _client = client;
            _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config-dev.json"));
        }

        public BotState GetCurrentBotState()
        {
            var botState = new BotState()
            {
                ConnectionStatus = _client.ConnectionState.ToString(),
                LoginStatus = _client.LoginState.ToString()
            };

            return botState;
        }

        public async Task Reconnect()
        {
            Console.WriteLine("Logging out..");
            await _client.LogoutAsync();
            Console.WriteLine("Logging in..");
            await _client.LoginAsync(TokenType.Bot, _config.BotToken);
            Console.WriteLine("Starting session..");
            await _client.StartAsync();
        }

        public async Task Disconnect()
        {
            Console.WriteLine("Logging out..");
            await _client.LogoutAsync();
        }

        public async Task Connect()
        {
            Console.WriteLine("Logging in..");
            await _client.LoginAsync(TokenType.Bot, _config.BotToken);
            Console.WriteLine("Starting session..");
            await _client.StartAsync();
        }

        public void LeaveGuild(string id)
        {
            var parsedId = ulong.Parse(id);
            _client.GetGuild(parsedId).LeaveAsync();
        }
    }
}