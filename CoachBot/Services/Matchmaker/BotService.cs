using CoachBot.Model;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Services.Matchmaker
{
    public class BotService
    {
        private DiscordSocketClient _client;
        private ConfigService _configService;

        public BotService(DiscordSocketClient client, ConfigService configService)
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

        public List<Channel> GetChannelsForUser(ulong userId, bool unconfiguredChannels, bool hasAdmin = true)
        {
            var channels = new List<Channel>();
            foreach(var guild in _client.Guilds.Where(g => g.Users.Any(u => u.Id == userId)))
            {
                var userIsAdmin = guild.Users.FirstOrDefault(u => u.Id == userId).GuildPermissions.Administrator;
                if (userIsAdmin || (!hasAdmin && guild.Users.Any(u => u.Id == userId)) || userId == 166153339610857472)
                {
                    foreach (var channel in guild.Channels)
                    {
                        var channelIsConfigured = _configService.Config.Channels.Any(c => c.Id == channel.Id);
                        if (channelIsConfigured && !unconfiguredChannels)
                        {
                            var tmpChannel = _configService.Config.Channels.FirstOrDefault(c => c.Id == channel.Id);
                            tmpChannel.Name = channel.Name;
                            channels.Add(tmpChannel);
                        }
                        if (!channelIsConfigured && unconfiguredChannels)
                        {
                            channels.Add(new Channel() { Id = channel.Id });
                        }
                    }
                }
            }
            channels.Reverse();
            return channels;
        }

        public bool UserIsOwningGuildAdmin(ulong userId)
        {
            var owningGuild = _client.Guilds.First(g => g.Id == _configService.Config.OwnerGuildId);
            return owningGuild.Users.FirstOrDefault(u => u.Id == userId).GuildPermissions.Administrator;
        }
    }
}
