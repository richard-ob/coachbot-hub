using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using CoachBot.Services.Matchmaker;
using CoachBot.Model;
using CoachBot.Preconditions;
using System.Collections.Generic;

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
        [RequireChannelConfigured]
        public async Task SignAsync()
        {
            await ReplyAsync(_service.AddPlayer(Context.Channel.Id, Context.Message.Author));
            await ReplyAsync(_service.GenerateTeamList(Context.Channel.Id));
        }

        [Command("sign")]
        [Alias("s")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task SignAsync(string position)
        {
            await ReplyAsync(_service.AddPlayer(Context.Channel.Id, Context.Message.Author, position));
            await ReplyAsync(_service.GenerateTeamList(Context.Channel.Id));
        }

        [Command("sign")]
        [Alias("s")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task CounterSignAsync(string position, [Remainder]string name)
        {
            await ReplyAsync(_service.AddPlayer(Context.Channel.Id, name, position));
            await ReplyAsync(_service.GenerateTeamList(Context.Channel.Id));
        }

        [Command("unsign")]
        [Alias("us", "u", "r", "remove")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task UnsignAsync()
        {
            _service.RemovePlayer(Context.Channel.Id, Context.Message.Author);
            await ReplyAsync($"Unsigned {Context.Message.Author.Username}");
            await ReplyAsync(_service.GenerateTeamList(Context.Channel.Id));
        }

        [Command("unsign")]
        [Alias("us", "u", "r", "remove")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task UnsignAsync(string name)
        {
            _service.RemovePlayer(Context.Channel.Id, name);
            await ReplyAsync($"Unsigned {Context.Message.Author.Username}");
            await ReplyAsync(_service.GenerateTeamList(Context.Channel.Id));
        }

        [Command("vsmix")]
        [Alias("s")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task VsMixAsync()
        {
            var team = new Team()
            {
                Name = "Mix",
                IsMix = true,
                Players = new Dictionary<Player, string>()
            };
            await ReplyAsync(_service.ChangeOpposition(Context.Channel.Id, team));
            await ReplyAsync(_service.GenerateTeamList(Context.Channel.Id));
        }

        [Command("vs")]
        [Alias("s")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task VsAsync(string teamName)
        {
            var team = new Team()
            {
                Name = teamName,
                IsMix = false
            };
            await ReplyAsync(_service.ChangeOpposition(Context.Channel.Id, team));
            await ReplyAsync(_service.GenerateTeamList(Context.Channel.Id));
        }

        [Command("configure")]
        [Priority(1000)]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task ConfigureChannelAsync(string teamName, bool isMixChannel, params string[] positions)
        {
            await ReplyAsync(_service.ConfigureChannel(Context.Message.Channel.Id, teamName, positions.ToList(), isMixChannel));
        }

        [Command("reset")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task ResetChannelAsync(params string[] positions)
        {
            _service.ResetMatch(Context.Message.Channel.Id);
            await ReplyAsync("Match reset..");
            await ReplyAsync(_service.GenerateTeamList(Context.Channel.Id));
        }

        [Command("ready")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task ReadyAsync()
        {
            await ReplyAsync(_service.ReadyMatch(Context.Message.Channel.Id));
        }

        [Command("list")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task ListAsync(params string[] positions)
        {
            await ReplyAsync(_service.GenerateTeamList(Context.Channel.Id));
            await ReplyAsync(_service.GenerateTeamList(Context.Channel.Id));
        }
    }
}
