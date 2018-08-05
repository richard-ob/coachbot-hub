using Discord;
using Discord.WebSocket;

namespace CoachBot.Services.Matchmaker
{
    public class AnnouncementService
    {
        private DiscordSocketClient _client;

        public AnnouncementService(DiscordSocketClient client)
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
