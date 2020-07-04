using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace CoachBot.Bot.Criteria
{
    public class EnsureMatchCommandCriterion : ICriterion<SocketMessage>
    {
        private readonly string _commandToMatch;

        public EnsureMatchCommandCriterion(string commandToMatch)
        {
            _commandToMatch = commandToMatch;
        }

        public Task<bool> JudgeAsync(SocketCommandContext sourceContext, SocketMessage parameter)
        {
            bool ok = parameter.Content.StartsWith(_commandToMatch);
            return Task.FromResult(ok);
        }
    }
}