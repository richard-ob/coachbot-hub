using CoachBot.Database;
using CoachBot.Domain.Model;
using CoachBot.Model;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoachBot.Domain.Services
{
    public class SubstitutionService
    {
        private readonly CoachBotContext _coachBotContext;
        private readonly ServerService _serverService;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly DiscordNotificationService _discordNotificationService;

        public SubstitutionService(CoachBotContext coachBotContext, ServerService serverService, DiscordSocketClient discordSocketClient, DiscordNotificationService discordNotificationService)
        {
            _coachBotContext = coachBotContext;
            _serverService = serverService;
            _discordSocketClient = discordSocketClient;
            _discordNotificationService = discordNotificationService;
        }

        public async Task CreateRequest(int channelId, int serverId, string positionName, string requestedBy)
        {
            var token = DateTime.UtcNow.Ticks.ToString().GetHashCode().ToString("x");
            var channel = _coachBotContext.Channels.Single(c => c.Id == channelId);
            var request = new SubstitutionRequest()
            {
                Token = token,
                CreatedDate = DateTime.UtcNow,
                ServerId = serverId,
                ChannelId = channelId
            };

            var embed = new EmbedBuilder()
                .WithDescription($":sos: A substitute{(string.IsNullOrEmpty(positionName) ? "" : " **" + positionName.ToUpper() + "**")} is required urgently. Type **!acceptsub {token}** to sub in.")
                .WithCurrentTimestamp()
                .WithColor(new Color(254, 254, 254))
                .WithFooter(new EmbedFooterBuilder().WithText("Requested by " + requestedBy))
                .Build();

            var discordSearchMessage = await _discordNotificationService.SendChannelMessage(channel.DiscordChannelId, embed);
            request.DiscordMessageId = discordSearchMessage;

            _coachBotContext.SubstitutionRequests.Add(request);
            _coachBotContext.SaveChanges();
        }

        public ServiceResponse AcceptSubstitution(string requestToken, Player player)
        {
            var request = _coachBotContext.SubstitutionRequests.Include(s => s.Channel).Include(s => s.Server).FirstOrDefault(s => s.Token == requestToken);

            if (request == null) return new ServiceResponse(ServiceResponseStatus.Failure, $"No such subtitution request exists, {player.DiscordUserMention}. Check the unique token and try again.");
            if (request.AcceptedDate != null) return new ServiceResponse(ServiceResponseStatus.Failure, $"This substitution request has already been accepted, {player.DiscordUserMention}.");

            request.AcceptedDate = DateTime.UtcNow;
            request.AcceptedById = player.Id;
            _coachBotContext.SaveChanges();

            if (request.DiscordMessageId > 0)
            {
                var channel = _discordSocketClient.GetChannel(request.Channel.DiscordChannelId) as ITextChannel;
                var message = channel.GetMessageAsync(request.DiscordMessageId).Result;
                if (message != null) message.DeleteAsync().Wait();
            }

            return new ServiceResponse(ServiceResponseStatus.Success, $":repeat: {player.DiscordUserMention} comes off the bench on {request.Server.Name} (steam://connect/{request.Server.Address})");
        }

        public ServiceResponse CancelSubstitution(string requestToken, Player player)
        {
            var request = _coachBotContext.SubstitutionRequests.Find(requestToken);

            if (request == null) return new ServiceResponse(ServiceResponseStatus.Failure, $"No such subtitution request exists, {player.DiscordUserMention}. Check the unique token and try again.");
            if (request.AcceptedDate != null) return new ServiceResponse(ServiceResponseStatus.Failure, $"Too late. This substitution request has already been accepted, {player.DiscordUserMention}.");

            return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, $"Substitution request cancelled, {player.DiscordUserMention}");
        }
        
        public Server GetServerForSubstitionRequest(string requestToken)
        {
            var request = _coachBotContext.SubstitutionRequests.Include(s => s.Server).Single(s => s.Token == requestToken);

            return request.Server;
        }
    }
}