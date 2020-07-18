using CoachBot.Domain.Services;
using CoachBot.Shared.Model;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CoachBot.Preconditions
{
    public class RequireChannelConfigured : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider map)
        {
            using (var scope = map.CreateScope())
            {
                var channelService = scope.ServiceProvider.GetService<ChannelService>();
                var config = scope.ServiceProvider.GetService<Config>();

                if (channelService.GetChannelByDiscordId(context.Channel.Id) is null)
                    return Task.FromResult(PreconditionResult.FromError($":wrench: This channel is not yet configured. To configure the channel, please visit {config.ClientUrl}"));

                if (channelService.GetChannelByDiscordId(context.Channel.Id).Team.Region is null)
                    return Task.FromResult(PreconditionResult.FromError($":earth_asia: This channel does not have a region set. To configure the channel, please visit {config.ClientUrl}"));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}