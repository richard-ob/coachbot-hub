using CoachBot.Database;
using CoachBot.Domain.Model;
using CoachBot.Model;
using Discord;
using Discord.WebSocket;
using System;

namespace CoachBot.Domain.Services
{
    public class SubstitutionService
    {
        private readonly CoachBotContext _coachBotContext;
        private readonly ServerService _serverService;
        private readonly DiscordSocketClient _discordSocketClient;

        public SubstitutionService(CoachBotContext coachBotContext, ServerService serverService, DiscordSocketClient discordSocketClient)
        {
            _coachBotContext = coachBotContext;
            _serverService = serverService;
            _discordSocketClient = discordSocketClient;
        }

        public ServiceResponse CreateRequest(int channelId, int serverId, string positionName)
        {
            var token = DateTime.UtcNow.Ticks.ToString().GetHashCode().ToString("x");
            var request = new SubstitutionRequest()
            {
                Token = token,
                CreatedDate = DateTime.UtcNow,
                ServerId = serverId,
                ChannelId = channelId
            };

            _coachBotContext.SubstitutionRequests.Add(request);
            _coachBotContext.SaveChanges();

            return new ServiceResponse(
                ServiceResponseStatus.Success,
                $"A substitute{(string.IsNullOrEmpty(positionName) ? "" : " **" + positionName + "**")} is required urgently. Type **!acceptsub {token}** to sub in."
            );
        }

        public ServiceResponse AcceptSubstitution(string requestToken, Player player)
        {
            var request = _coachBotContext.SubstitutionRequests.Find(requestToken);

            if (request == null) return new ServiceResponse(ServiceResponseStatus.Failure, $"No such subtitution request exists, {player.DiscordUserMention}. Check the unique token and try again.");
            if (request.AcceptedDate != null) return new ServiceResponse(ServiceResponseStatus.Failure, $"This substitution request has already been accepted, {player.DiscordUserMention}.");

            request.AcceptedDate = DateTime.UtcNow;
            request.AcceptedBy = player;
            _coachBotContext.SaveChanges();

            if (request.DiscordMessageId > 0)
            {
                var channel = _discordSocketClient.GetChannel(request.Channel.DiscordChannelId) as ITextChannel;
                var message = channel.GetMessageAsync(request.DiscordMessageId).Result;
                if (message != null) message.DeleteAsync();
            }

            return new ServiceResponse(ServiceResponseStatus.Success, $":repeat: {player.DiscordUserMention} comes off the bench on {request.Server.Name} (steam://{request.Server.Address})");
        }

        public ServiceResponse CancelSubstitution(string requestToken, Player player)
        {
            var request = _coachBotContext.SubstitutionRequests.Find(requestToken);

            if (request == null) return new ServiceResponse(ServiceResponseStatus.Failure, $"No such subtitution request exists, {player.DiscordUserMention}. Check the unique token and try again.");
            if (request.AcceptedDate != null) return new ServiceResponse(ServiceResponseStatus.Failure, $"Too late. This substitution request has already been accepted, {player.DiscordUserMention}.");

            return new ServiceResponse(ServiceResponseStatus.NegativeSuccess, $"Substitution request cancelled, {player.DiscordUserMention}");
        }
    }
}