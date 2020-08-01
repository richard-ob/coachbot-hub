using CoachBot.Domain.Services;
using CoachBot.Services;
using CoachBot.Shared.Model;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CoachBot.Bot.Preconditions
{
    public class RequireChannelAndTeamActive : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider map)
        {
            using (var scope = map.CreateScope())
            {
                var channelService = scope.ServiceProvider.GetService<ChannelService>();
                var config = scope.ServiceProvider.GetService<Config>();

                var channel = channelService.GetChannelByDiscordId(context.Channel.Id);
                if (channel != null && channel.Inactive == true)
                    return Task.FromResult(PreconditionResult.FromError($":wrench: This channel marked as inactive. To change this, please visit {config.WebServerConfig.ClientUrl}"));

                if (channel != null && channel.Team != null && channel.Team.Inactive == true)
                    return Task.FromResult(PreconditionResult.FromError($":wrench: This team is marked as inactive. To change this, please visit {config.WebServerConfig.ClientUrl}"));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}