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
        [RequireChannelConfigured]
        public async Task SignAsync()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.AddPlayer(Context.Channel.Id, Context.Message.Author)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!sign")]
        [Alias("!s")]
        [RequireChannelConfigured]
        public async Task SignAsync(string position)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.AddPlayer(Context.Channel.Id, Context.Message.Author, position)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!sign")]
        [Alias("!s")]
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
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!sign2")]
        [Alias("!s2")]
        [RequireChannelConfigured]
        public async Task SignTeam2Async()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.AddPlayer(Context.Channel.Id, Context.Message.Author, null, Teams.Team2)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!sign2")]
        [Alias("!s2")]
        [RequireChannelConfigured]
        public async Task SignTeam2Async(string position)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.AddPlayer(Context.Channel.Id, Context.Message.Author, position, Teams.Team2)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!sign2")]
        [Alias("!s2")]
        [RequireChannelConfigured]
        public async Task CounterSignTeam2Async(string position, [Remainder]string name)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.AddPlayer(Context.Channel.Id, name, position, Teams.Team2)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!unsign")]
        [Alias("!us", "!u", "!r", "!remove")]
        [RequireChannelConfigured]
        public async Task UnsignAsync()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.RemovePlayer(Context.Channel.Id, Context.Message.Author)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!unsign")]
        [Alias("!us", "!u", "!r", "!remove")]
        [RequireChannelConfigured]
        public async Task UnsignAsync([Remainder]string name)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.RemovePlayer(Context.Channel.Id, name)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!vsmix")]
        [RequireChannelConfigured]
        public async Task VsMixAsync()
        {
            var team = new Team()
            {
                Name = _configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team1.Name == "Mix" ? "Mix #2" : "Mix",
                IsMix = true,
                Players = new List<Player>(),
            };
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.ChangeOpposition(Context.Channel.Id, team)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!vs")]
        [Alias("!removevs", "!vsremove")]
        [RequireChannelConfigured]
        public async Task VsAsync([Remainder]string teamName = null)
        {
            var team = new Team()
            {
                Name = teamName,
                IsMix = false,
                Players = new List<Player>()
            };
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.ChangeOpposition(Context.Channel.Id, team)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!configureadvanced")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task ConfigureChannelAsync(string teamName, string kitEmote, string badgeEmote, string color, Formation formation, bool isMixChannel, params string[] positions)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.ConfigureChannel(Context.Message.Channel.Id, teamName, positions.Select(p => new Position() { PositionName = p }).ToList(), kitEmote, badgeEmote, color, isMixChannel, formation, false)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!configuremix")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task ConfigureChannelAsync(params string[] positions)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.ConfigureChannel(Context.Message.Channel.Id, "Mix", positions.Select(p => new Position() { PositionName = p }).ToList(), null, null, null, true, Formation.None, true)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!configure")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task ConfigureChannelAsync(string teamName, params string[] positions)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.ConfigureChannel(Context.Message.Channel.Id, teamName, positions.Select(p => new Position() { PositionName = p }).ToList(), null, null, null, false, Formation.None, true)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!reset")]
        [RequireChannelConfigured]
        public async Task ResetChannelAsync(params string[] positions)
        {
            _service.ResetMatch(Context.Message.Channel.Id);
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(":negative_squared_cross_mark: Team sheet reset").WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!ready")]
        [RequireChannelConfigured]
        public async Task ReadyAsync()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.ReadyMatch(Context.Message.Channel.Id)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!unready")]
        [RequireChannelConfigured]
        public async Task UnreadyAsync()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.UnreadyMatch(Context.Message.Channel.Id)).WithCurrentTimestamp().Build());
        }

        [Command("!ready")]
        [RequireChannelConfigured]
        public async Task ReadyAsync(int serverId)
        {
            await ReplyAsync(_service.ReadyMatch(Context.Message.Channel.Id, serverId));
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!list")]
        [Alias("!teamlist", "!teamsheet", "!teamlists", "!teamsheets", "!lineup!")]
        [RequireChannelConfigured]
        public async Task ListAsync(params string[] positions)
        {
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!search")]
        [RequireChannelConfigured]
        public async Task SearchOppositionAsync()
        {            
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.Search(Context.Channel.Id, Context.User.Mention)).Build());
        }

        [Command("!challenge")]
        [RequireChannelConfigured]
        public async Task ChallengeAsync(ulong oppositionId)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.Challenge(Context.Channel.Id, oppositionId, Context.User.Mention)).Build());
        }

        [Command("!stopsearch")]
        [RequireChannelConfigured]
        public async Task StopSearchOppositionAsync()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.StopSearch(Context.Channel.Id)).Build());
        }

        [Command("!here")]
        [Alias("!highlight")]
        [RequireChannelConfigured]
        public async Task MentionHereAsync()
        {
            await ReplyAsync(_service.MentionHere(Context.Message.Channel.Id));
        }

        [Command("!servers")]
        public async Task ServersAsync()
        {
            await ReplyAsync("", embed: _configService.ReadServerList());
        }

        [Command("!addserver")]
        [Alias("!addsv")]
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
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task RemoveServerAsync(int id)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_configService.RemoveServer(id)).Build());
        }

        [Command("!recentmatches")]
        public async Task RecentMatchesAsync()
        {
            await ReplyAsync("", embed: _statisticsService.RecentMatches(Context.Channel.Id));
        }

        [Command("!leaderboard")]
        public async Task AppearanceLeaderboardAsync()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_statisticsService.AppearanceLeaderboard(Context.Channel.Id)).Build());
        }

        [Command("!formations")]
        public async Task FormationsAsync()
        {
            await ReplyAsync("", embed: _configService.ListFormations());
        }

        [Command("!colours")]
        public async Task ColoursAsync()
        {
            await ReplyAsync("", embed: _configService.ListColours());
        }

        [Command("!help")]
        public async Task HelpAsync()
        {
            IDMChannel dmChannel = await Context.Message.Author.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync("", embed: _configService.ListCommands());
        }

        [Command("!saveconfig")]
        public async Task SaveAsync()
        {
            _configService.Save();
            await ReplyAsync("Config saved");
        }

        [Command("!sub")]
        public async Task SubAsync()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.AddSub(Context.Channel.Id, Context.Message.Author)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!unsub")]
        public async Task UnsubAsync()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.RemoveSub(Context.Channel.Id, Context.Message.Author)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }

        [Command("!unsub")]
        public async Task UnsubAsync([Remainder]string playerName)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_service.RemoveSub(Context.Channel.Id, playerName)).WithCurrentTimestamp().Build());
            await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id));
            if (_configService.Config.Channels.First(c => c.Id == Context.Message.Channel.Id).Team2.IsMix)
            {
                await ReplyAsync("", embed: _service.GenerateTeamList(Context.Channel.Id, Teams.Team2));
            }
        }
    }
}
