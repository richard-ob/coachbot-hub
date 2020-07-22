using CoachBot.Services.Matchmaker;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoachBot.Preconditions
{
    public class RequireChannelConfigured : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider map)
        {
            /*var configService = map.GetService<ConfigService>();

            if (!configService.Config.Channels.Any(c => c.Id == context.Channel.Id))
                return Task.FromResult(PreconditionResult.FromError($":wrench: This channel is not yet configured. To configure the channel, please visit http://coachbot.iosoccer.com"));

            if (configService.Config.Channels.First(c => c.Id == context.Channel.Id).RegionId == 0)
                return Task.FromResult(PreconditionResult.FromError($":earth_asia: This channel does not have a region set. To configure the channel, please visit http://coachbot.iosoccer.com"));*/

            // INFO: We can't use this with new Coach running in parallel
            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}