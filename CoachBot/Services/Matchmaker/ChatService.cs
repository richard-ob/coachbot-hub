using Discord;
using Discord.WebSocket;

namespace CoachBot.Services.Matchmaker
{
    public class ChatService
    {
        private DiscordSocketClient _client;

        public ChatService(DiscordSocketClient client)
        {
            _client = client;
        }

        public void Say(string message)
        {
            foreach (var server in _client.Guilds)
            {
                foreach (var channel in server.Channels)
                {
                    var textChannel = _client.GetChannel(channel.Id) as ITextChannel;
                    if (textChannel != null)
                    {
                        textChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(message).WithCurrentTimestamp().Build());
                    }
                }
            }
        }
    }
}
