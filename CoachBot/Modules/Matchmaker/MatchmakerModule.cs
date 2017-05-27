using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using CoachBot.Services.Matchmaker;
using CoachBot.Model;
using CoachBot.Preconditions;
using System.Collections.Generic;
using System.Text;

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
            await ReplyAsync(_service.AddPlayer(Context.Channel.Id, Context.Message.Author));
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
            await ReplyAsync(_service.AddPlayer(Context.Channel.Id, Context.Message.Author, position));
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
                await ReplyAsync(_service.AddPlayer(Context.Channel.Id, user, position));
            }
            else
            {
                await ReplyAsync(_service.AddPlayer(Context.Channel.Id, name, position));
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
            await ReplyAsync(_service.AddPlayer(Context.Channel.Id, Context.Message.Author, null, Teams.Team2));
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
            await ReplyAsync(_service.AddPlayer(Context.Channel.Id, Context.Message.Author, position, Teams.Team2));
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
            await ReplyAsync(_service.AddPlayer(Context.Channel.Id, name, position, Teams.Team2));
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
            await ReplyAsync(_service.RemovePlayer(Context.Channel.Id, Context.Message.Author));
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
            await ReplyAsync(_service.RemovePlayer(Context.Channel.Id, name));
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
            await ReplyAsync(_service.ChangeOpposition(Context.Channel.Id, team));
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
            await ReplyAsync(_service.ChangeOpposition(Context.Channel.Id, team));
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_service.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!configure")]
        [Priority(1000)]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task ConfigureChannelAsync(string teamName, bool isMixChannel = false, bool useFormation = true, params string[] positions)
        {
            await ReplyAsync(_service.ConfigureChannel(Context.Message.Channel.Id, teamName, positions.ToList(), isMixChannel));
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
            await ReplyAsync("Match reset..");
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
            await ReplyAsync(_service.ReadyMatch(Context.Message.Channel.Id));
        }

        [Command("!unready")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task UnreadyAsync()
        {
            await ReplyAsync(_service.UnreadyMatch(Context.Message.Channel.Id));
        }

        [Command("!ready")]
        [Priority(1000)]
        [RequireChannelConfigured]
        public async Task ReadyAsync(int serverId)
        {
            await ReplyAsync(_service.ReadyMatch(Context.Message.Channel.Id, serverId));
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
            await ReplyAsync(_configService.ReadServerList());
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
            await ReplyAsync(_configService.AddServer(server));
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
            await ReplyAsync(_configService.RemoveServer(server));
        }

        [Command("!recentmatches")]
        [Priority(1000)]
        public async Task RecentMatchesAsync()
        {
            await ReplyAsync(_statisticsService.RecentMatches(Context.Channel.Id));
        }

        [Command("!leaderboard")]
        [Priority(1000)]
        public async Task AppearanceLeaderboardAsync()
        {
            await ReplyAsync(_statisticsService.AppearanceLeaderboard(Context.Channel.Id));
        }

        [Command("!help")]
        [Priority(1000)]
        public async Task HelpAsync()
        {
            var sb = new StringBuilder();
            sb.AppendLine(":keyboard: Commands:");
            sb.AppendLine("**!sign** - *Sign yourself in the first available position*");
            sb.AppendLine("**!sign <position>** - *Sign yourself in the specified position*");
            sb.AppendLine("**!sign <position> <name>** - *Sign on behalf of someone not in Discord*");
            sb.AppendLine("**!sign2** - *Sign yourself in the first available position to Team 2*");
            sb.AppendLine("**!sign2 <position>** - *Sign in specified position to Team 2*");
            sb.AppendLine("**!sign2 <position> <name>** - *Sign on behalf of someone not in Discord to Team 2*");
            sb.AppendLine("**!unsign** - *Unsign from the match*");
            sb.AppendLine("**!unsign <name>** - *Unsign the person specified from the match*");
            sb.AppendLine("**!vs <team>** - *Set the opposition team for the current match*");
            sb.AppendLine("**!vsmix** - *Set the opposition team to a managed mix for the current match*");
            sb.AppendLine("**!ready** - *Send all players to server*");
            sb.AppendLine("**!ready <server id>** - *Send all players to the server provided*");
            sb.AppendLine("**!reset** - *Manually reset the match*");
            sb.AppendLine("**!servers** - *See the full available server list*");
            sb.AppendLine("**!addserver <ip:port> <name>** - *Add a server to the server list*");
            sb.AppendLine("**!removeserver <ip:port> <name>** - *Remove specified server to the server list*");
            sb.AppendLine("**!recentmatches** - *See a list of recent matches played*");
            sb.AppendLine("**!leaderboard** - *See the appearance rankings for this channel*");
            sb.AppendLine("**!configure <team name> <is a mix channel> <use formation on team sheet> <positions>** ***e.g. !configure BB false true GK RB CB LB RW CM LW CF*** - *Configure the current channel's matchmaking settings*");
            await ReplyAsync(sb.ToString());

        }
    }
}
