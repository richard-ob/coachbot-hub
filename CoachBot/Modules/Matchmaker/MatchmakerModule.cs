using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using CoachBot.Services.Matchmaker;
using CoachBot.Model;
using CoachBot.Preconditions;
using System.Collections.Generic;
using System.Text;
using Discord;

namespace CoachBot.Modules.Matchmaker
{
    public class MatchmakerModule : ModuleBase
    {
        private readonly MatchmakerService _service;
        private readonly ConfigService _configService;
        private readonly StatisticsService _statisticsService;

        public MatchmakerModule(MatchmakerService service, ConfigService configService, StatisticsService statisticsService)
        {
            _service = service;
            _configService = configService;
            _statisticsService = statisticsService;
        }

        [Command("!sign")]
        [Alias("!s")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task SignAsync()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.AddPlayer(Context.Channel.Id, Context.Message.Author)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_service.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!sign")]
        [Alias("!s")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task SignAsync(string position)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.AddPlayer(Context.Channel.Id, Context.Message.Author, position)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_service.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!sign")]
        [Alias("!s")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task CounterSignAsync(string position, [Remainder]string name)
        {
            if (name.StartsWith("<@") && name.EndsWith(">"))
            {
                var user = await Context.Guild.GetUserAsync(ulong.Parse(name.Replace("<", string.Empty).Replace("@", string.Empty).Replace(">", string.Empty)));
                await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.AddPlayer(Context.Channel.Id, user, position)).WithCurrentTimestamp().Build());
            }
            else
            {
                await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.AddPlayer(Context.Channel.Id, name, position)).WithCurrentTimestamp().Build());
            }
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_service.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!sign2")]
        [Alias("!s2")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task SignTeam2Async()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.AddPlayer(Context.Channel.Id, Context.Message.Author, null, Teams.Team2)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_service.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!sign2")]
        [Alias("!s2")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task SignTeam2Async(string position)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.AddPlayer(Context.Channel.Id, Context.Message.Author, position, Teams.Team2)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_service.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!sign2")]
        [Alias("!s2")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task CounterSignTeam2Async(string position, [Remainder]string name)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.AddPlayer(Context.Channel.Id, name, position, Teams.Team2)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_service.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!unsign")]
        [Alias("!us", "!u", "!r", "!remove")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task UnsignAsync()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.RemovePlayer(Context.Channel.Id, Context.Message.Author)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_service.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!unsign")]
        [Alias("!us", "!u", "!r", "!remove")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task UnsignAsync(string name)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.RemovePlayer(Context.Channel.Id, name)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_service.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!vsmix")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task VsMixAsync()
        {
            var team = new Team()
            {
                Name = _service.Channels.First(c => c.Id == Context.Message.Channel.Id).Team1.Name == "Mix" ? "Mix #2" : "Mix",
                IsMix = true,
                Players = new Dictionary<Player, string>()
            };
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.ChangeOpposition(Context.Channel.Id, team)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_service.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!vs")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task VsAsync(string teamName = null)
        {
            var team = new Team()
            {
                Name = teamName,
                IsMix = false,
                Players = new Dictionary<Player, string>()
            };
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.ChangeOpposition(Context.Channel.Id, team)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_service.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!configure")]
        [Priority(1000)]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task ConfigureChannelAsync(string teamName, bool isMixChannel = false, bool useFormation = true, bool classicLineup = true, params string[] positions)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.ConfigureChannel(Context.Message.Channel.Id, teamName, positions.ToList(), isMixChannel, useFormation, classicLineup)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_service.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!reset")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task ResetChannelAsync(params string[] positions)
        {
            _service.ResetMatch(Context.Message.Channel.Id);
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(":negative_squared_cross_mark: Match reset..").WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_service.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!ready")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task ReadyAsync()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.ReadyMatch(Context.Message.Channel.Id)).WithCurrentTimestamp().Build());
        }

        [Command("!unready")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task UnreadyAsync()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.UnreadyMatch(Context.Message.Channel.Id)).WithCurrentTimestamp().Build());
        }

        [Command("!ready")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task ReadyAsync(int serverId)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.ReadyMatch(Context.Message.Channel.Id, serverId)).WithCurrentTimestamp().Build());
        }

        [Command("!list")]
        [Alias("!teamlist", "!teamsheet", "!teamlists", "!teamsheets", "!lineup!")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task ListAsync(params string[] positions)
        {
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_service.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!servers")]
        [Priority(1000)]
        public async Task ServersAsync()
        {
            await ReplyAsync("", embed: _configService.ReadServerList());
        }

        [Command("!addserver")]
        [Alias("!addsv")]
        [Priority(1000)]
        public async Task AddServerAsync(string ip, [Remainder]string name)
        {
            var server = new Server()
            {
                Address = ip,
                Name = name
            };
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_configService.AddServer(server)).Build());
        }

        [Command("!removeserver")]
        [Alias("!rmsv")]
        [Priority(1000)]
        public async Task RemoveServerAsync(string ip, [Remainder]string name)
        {
            var server = new Server()
            {
                Address = ip,
                Name = name
            };
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_configService.RemoveServer(server)).Build());
        }

        [Command("!recentmatches")]
        [Priority(1000)]
        public async Task RecentMatchesAsync()
        {
            await ReplyAsync("", embed: _statisticsService.RecentMatches(Context.Channel.Id));
        }

        [Command("!leaderboard")]
        [Priority(1000)]
        public async Task AppearanceLeaderboardAsync()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_statisticsService.AppearanceLeaderboard(Context.Channel.Id)).Build());
        }

        [Command("!help")]
        [Priority(1000)]
        public async Task HelpAsync()
        {
            await ReplyAsync("", embed: _configService.ListCommands());
        }
    }
}
