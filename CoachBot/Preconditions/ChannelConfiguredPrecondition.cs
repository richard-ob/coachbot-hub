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
            var matchmakerService = map.GetService<MatchmakerService>();

            if (!matchmakerService.Channels.Any(c => c.Id == context.Channel.Id))
                return Task.FromResult(PreconditionResult.FromError($":wrench: This channel is not yet configured. To configure the channel, please use the !configure command in the following format: {Environment.NewLine} **!configure <team name> <is a mix channel [true/false]> <use formation on team sheet [true/false]> <positions>** {Environment.NewLine} *e.g. !configure BB false true GK RB CB LB RW CM LW CF*"));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}