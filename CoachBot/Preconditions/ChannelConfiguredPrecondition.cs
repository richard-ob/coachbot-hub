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
                return Task.FromResult(PreconditionResult.FromError($":wrench: This channel is not yet configured. To configure the channel, please use the !configure command in the following format: {Environment.NewLine} **!configurebasic <team name> <positions>** {Environment.NewLine} *e.g. !configurebasic BB false GK RB CB LB RW CM LW CF* {Environment.NewLine} For more advanced configuration (such as using the teamsheet view) please type !help for further information."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}