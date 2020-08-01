using CoachBot.Model;
using CoachBot.Shared.Model;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoachBot.Domain.Services
{
    public class DiscordNotificationService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly Config _config;

        public DiscordNotificationService(DiscordSocketClient discordSocketClient, Config config)
        {
            _discordSocketClient = discordSocketClient;
            _config = config;
        }

        public async Task<ulong> SendChannelMessage(ulong discordChannelId, Embed embed)
        {
            if (_discordSocketClient.GetChannel(discordChannelId) is ITextChannel channel)
            {
                var result = await channel.SendMessageAsync("", embed: embed);

                return result.Id;
            }

            return 0;
        }

        public async Task<ulong> SendChannelMessage(ulong discordChannelId, string message)
        {
            return await SendChannelMessage(discordChannelId, new EmbedBuilder().WithDescription(message).Build());
        }

        public async Task<ulong> SendChannelTextMessage(ulong discordChannelId, string message)
        {
            if (_discordSocketClient.GetChannel(discordChannelId) is ITextChannel channel)
            {
                var result = await channel.SendMessageAsync(message);

                return result.Id;
            }

            return 0;
        }

        public async Task<Dictionary<ulong, ulong>> SendChannelMessage(List<ulong> discordChannelIds, Embed embed)
        {
            var messageIds = new Dictionary<ulong, ulong>();

            foreach (var discordChannelId in discordChannelIds)
            {
                var discordMessageId = await SendChannelMessage(discordChannelId, embed);
                if (discordMessageId != 0) messageIds.Add(discordChannelId, discordMessageId);
            }

            return messageIds;
        }

        public async Task<Dictionary<ulong, ulong>> SendChannelMessage(List<ulong> discordChannelIds, string message)
        {
            var messageIds = new Dictionary<ulong, ulong>();

            foreach (var discordChannelId in discordChannelIds)
            {
                var discordMessageId = await SendChannelMessage(discordChannelId, message);
                if (discordMessageId != 0) messageIds.Add(discordChannelId, discordMessageId);
            }

            return messageIds;
        }

        public async Task<ulong> SendUserMessage(ulong discordUserId, string message)
        {
            if (await _discordSocketClient.GetUser(discordUserId).GetOrCreateDMChannelAsync() is IDMChannel dmChannel)
            {
                var result = await dmChannel.SendMessageAsync(message);

                return result.Id;
            }

            return 0;
        }

        public async Task SendAuditChannelMessage(string message)
        {
            await SendChannelMessage(_config.DiscordConfig.AuditChannelId, message);
        }

        public async Task SendAuditChannelMessage(Embed embed)
        {
            await SendChannelMessage(_config.DiscordConfig.AuditChannelId, embed);
        }
    }
}