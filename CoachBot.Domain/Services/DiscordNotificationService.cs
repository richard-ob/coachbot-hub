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

        public void SendChannelMessage(ulong discordChannelId, Embed embed)
        {
            if (_discordSocketClient.GetChannel(discordChannelId) is ITextChannel channel)
            {
                channel.SendMessageAsync("", embed: embed);
            }
        }

        public void SendChannelMessage(ulong discordChannelId, string message)
        {
            SendChannelMessage(discordChannelId, new EmbedBuilder().WithDescription(message).Build());
        }

        public void SendChannelMessage(List<ulong> discordChannelIds, Embed embed)
        {
            foreach (var discordChannelId in discordChannelIds)
            {
                SendChannelMessage(discordChannelId, embed);
            }
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
            if (_discordSocketClient.GetUser(discordUserId).GetOrCreateDMChannelAsync() is IDMChannel dmChannel)
            {
                dmChannel.SendMessageAsync(message);
            }
        }
    }
}
