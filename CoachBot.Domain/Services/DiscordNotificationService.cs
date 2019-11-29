using Discord;
using Discord.WebSocket;
using System.Collections.Generic;

namespace CoachBot.Domain.Services
{
    public class DiscordNotificationService
    {
        private readonly DiscordSocketClient _discordSocketClient;

        public DiscordNotificationService(DiscordSocketClient discordSocketClient)
        {
            _discordSocketClient = discordSocketClient;
        }

        public void SendChannelMessage(ulong discordChannelId, string message)
        {
            var channel = _discordSocketClient.GetChannel(discordChannelId) as ITextChannel;
            channel.SendMessageAsync(message);
        }

        public void SendChannelMessage(List<ulong> discordChannelIds, string message)
        {
            foreach(var discordChannelId in discordChannelIds)
            {
                SendChannelMessage(discordChannelId, message);
            }
        }

        public void SendUserMessage(ulong discordUserId, string message)
        {
            var dmChannel = _discordSocketClient.GetUser(discordUserId).GetOrCreateDMChannelAsync() as IDMChannel;
            dmChannel.SendMessageAsync(message);
        }
    }
}
