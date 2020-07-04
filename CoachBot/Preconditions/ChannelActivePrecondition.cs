using CoachBot.Domain.Services;
using CoachBot.Services;
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
                var configService = scope.ServiceProvider.GetService<ConfigService>();

                var channel = channelService.GetChannelByDiscordId(context.Channel.Id);
                if (channel != null && channel.Inactive == true)
                    return Task.FromResult(PreconditionResult.FromError($":wrench: This channel marked as inactive. To change this, please visit {configService.Config.ClientUrl}"));

                if (channel != null && channel.Team != null && channel.Team.Inactive == true)
                    return Task.FromResult(PreconditionResult.FromError($":wrench: This team is marked as inactive. To change this, please visit {configService.Config.ClientUrl}"));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
