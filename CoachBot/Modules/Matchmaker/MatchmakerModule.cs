using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using CoachBot.Services.Matchmaker;

namespace CoachBot.Modules.Matchmaker
{
    public class MatchmakerModule : ModuleBase
    {
        private readonly MatchmakerService _service;

        public MatchmakerModule(MatchmakerService service)
        {
            _service = service;
        }

        [Command("sign")]
        [Alias("s")]
        [Priority(1000)]
        public async Task SignAsync(string position)
        {
            _service.AddPlayer(Context.Channel.Id, Context.Message.Author, position);
            await ReplyAsync($"Signed {Context.Message.Author.Username} in {position}");
            await ReplyAsync(_service.GenerateTeamList(Context.Channel.Id));
        }

        [Command("sign")]
        [Alias("s")]
        [Priority(1000)]
        public async Task CounterSignAsync(string position, [Remainder]string name)
        {
            await ReplyAsync($"Signed {name} in {position}");
        }

        [Command("whodis")]
        [Priority(1000)]
        public async Task TestDatAsync()
        {
            await ReplyAsync(Context.Message.Author.Username);
        }

        [Command("configure")]
        [Priority(1000)]
        public async Task ConfigureChannelAsync(params string[] positions)
        {
            _service.ConfigureChannel(Context.Message.Channel.Id, positions.ToList());
            await ReplyAsync("Configuring..");
        }
    }
}
