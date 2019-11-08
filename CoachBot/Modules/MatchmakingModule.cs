using Discord.Commands;
using System.Threading.Tasks;
using CoachBot.Preconditions;
using CoachBot.Services;
using CoachBot.Model;
using CoachBot.Tools;
using Discord;
using Discord.Addons.Interactive;
using System;
using CoachBot.Bot.Criteria;
using Discord.WebSocket;

namespace CoachBot.Modules.Matchmaker
{
    public class MatchmakingModule : InteractiveBase
    {
        private readonly DiscordMatchService _channelMatchService;
        private readonly DiscordServerService _channelServerService;
        private readonly InteractiveService _interactiveService;
        private bool sendUpdatedTeams = true;

        public MatchmakingModule(DiscordMatchService channelMatchService, DiscordServerService channelServerService, InteractiveService interactiveService)
        {
            _channelMatchService = channelMatchService;
            _channelServerService = channelServerService;
            _interactiveService = interactiveService;
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);
            Context.Message.AddReactionAsync(new Emoji("✅"));
        }

        protected override void AfterExecute(CommandInfo command)
        {
            base.AfterExecute(command);
            Context.Message.DeleteAsync();
            var coachEmote = "<:coach:481112227902914561>";
            if (Emote.TryParse(coachEmote, out var emote))
            {
                Context.Message.AddReactionAsync(emote);
            }
            if (this.sendUpdatedTeams)
            {
                foreach(var teamEmbed in _channelMatchService.GenerateTeamList(Context.Channel.Id))
                {
                    ReplyAsync("", embed: teamEmbed);
                }
            }
        }

        [Command("!sign")]
        [Alias("!s")]
        [RequireChannelConfigured]
        public async Task SignAsync(string positionName)
        {
            var response = _channelMatchService.AddPlayer(Context.Message.Channel.Id, Context.Message.Author, positionName);
            await ReplyAsync("", embed: response);
        }

        [Command("!sign")]
        [Alias("!s")]
        [RequireChannelConfigured]
        public async Task SignAsync()
        {
            var response = _channelMatchService.AddPlayer(Context.Message.Channel.Id, Context.Message.Author);
            await ReplyAsync("", embed: response);
        }

        [Command("!sign")]
        [Alias("!s")]
        [RequireChannelConfigured]
        public async Task CounterSignAsync(string positionName, [Remainder]string name)
        {
            if (name.StartsWith("<@") && name.EndsWith(">"))
            {
                var user = Context.Guild.GetUser(ulong.Parse(name.Replace("<", string.Empty).Replace("@", string.Empty).Replace(">", string.Empty)));
                var response = _channelMatchService.AddPlayer(Context.Message.Channel.Id, user, positionName);
                await ReplyAsync("", embed: response);
            }
            else
            {
                var response = _channelMatchService.AddPlayer(Context.Message.Channel.Id, name, positionName);
                await ReplyAsync("", embed: response);
            }
        }

        [Command("!sign2")]
        [Alias("!s2")]
        [RequireChannelConfigured]
        public async Task SignTeam2Async()
        {
            var response = _channelMatchService.AddPlayer(Context.Message.Channel.Id, Context.Message.Author, null, TeamType.Away);
            await ReplyAsync("", embed: response);
        }

        [Command("!sign2")]
        [Alias("!s2")]
        [RequireChannelConfigured]
        public async Task SignTeam2Async(string positionName)
        {
            var response = _channelMatchService.AddPlayer(Context.Message.Channel.Id, Context.Message.Author, positionName, TeamType.Away);
            await ReplyAsync("", embed: response);
        }

        [Command("!sign2")]
        [Alias("!s2")]
        [RequireChannelConfigured]
        public async Task CounterSignTeam2Async(string position, [Remainder]string name)
        {
            if (name.StartsWith("<@") && name.EndsWith(">"))
            {
                var user = Context.Guild.GetUser(ulong.Parse(name.Replace("<", string.Empty).Replace("@", string.Empty).Replace(">", string.Empty)));
                var response = _channelMatchService.AddPlayer(Context.Message.Channel.Id, user, null, TeamType.Away);
                await ReplyAsync("", embed: response);
            }
            else
            {
                var response = _channelMatchService.AddPlayer(Context.Message.Channel.Id, name, null, TeamType.Away);
                await ReplyAsync("", embed: response);
            }
        }

        [Command("!unsign")]
        [Alias("!us", "!u", "!r", "!remove")]
        [RequireChannelConfigured]
        public async Task UnsignAsync()
        {
            var response = _channelMatchService.RemovePlayer(Context.Message.Channel.Id, Context.Message.Author);
            await ReplyAsync("", embed: response);
        }

        [Command("!unsign")]
        [Alias("!us", "!u", "!r", "!remove")]
        [RequireChannelConfigured]
        public async Task UnsignAsync([Remainder]string name)
        {
            if (name.StartsWith("<@") && name.EndsWith(">"))
            {
                var user = Context.Guild.GetUser(ulong.Parse(name.Replace("<", string.Empty).Replace("@", string.Empty).Replace(">", string.Empty)));
                var response = _channelMatchService.RemovePlayer(Context.Message.Channel.Id, user);
                await ReplyAsync("", embed: response);
            }
            else
            {
                var response = _channelMatchService.RemovePlayer(Context.Message.Channel.Id, name);
                await ReplyAsync("", embed: response);
            }
        }

        [Command("!unsignpos")]
        [Alias("!up", "!clearpos")]
        [RequireChannelConfigured]
        public async Task UnsignPositionHomeTeamAsync(string position)
        {
            var response = _channelMatchService.RemovePlayer(Context.Message.Channel.Id, position);
            await ReplyAsync("", embed: response);
        }

        [Command("!unsignpos2")]
        [Alias("!up2", "!clearpos2")]
        [RequireChannelConfigured]
        public async Task UnsignPositionAwayTeamAsync(string position)
        {
            var response = _channelMatchService.RemovePlayer(Context.Message.Channel.Id, position);
            await ReplyAsync("", embed: response);
        }

        [Command("!reset")]
        [RequireChannelConfigured]
        public async Task ResetChannelAsync(params string[] positions)
        {
            var response = _channelMatchService.Reset(Context.Message.Channel.Id);
            await ReplyAsync("", embed: response);
        }

        [Command("!ready")]
        [RequireChannelConfigured]
        public async Task ReadyAsync(int serverListItemId)
        {
            _channelMatchService.ReadyMatch(Context.Message.Channel.Id, serverListItemId);
            this.sendUpdatedTeams = false;
        }

        [Command("!list")]
        [Alias("!lineup")]
        [RequireChannelConfigured]
        public async Task ListAsync()
        {
        }

        [Command("!here")]
        [Alias("!highlight")]
        [RequireChannelConfigured]
        public async Task MentionHereAsync()
        {
            await ReplyAsync("@here");
        }

        [Command("!search")]
        [RequireChannelConfigured]
        public async Task SearchOppositionAsync()
        {
            await ReplyAsync("", embed: _channelMatchService.Search(Context.Channel.Id, Context.User.Mention));
            this.sendUpdatedTeams = false;
        }

        [Command("!stopsearch")]
        [RequireChannelConfigured]
        public async Task StopSearchOppositionAsync()
        {
            await ReplyAsync("", embed: _channelMatchService.StopSearch(Context.Message.Channel.Id));
            this.sendUpdatedTeams = false;
        }

        [Command("!challenge")]
        [RequireChannelConfigured]
        public async Task ChallengeAsync()
        {
            await ReplyAsync("You must provide a valid team code, or type !challenges to see the currently available challenges");
            this.sendUpdatedTeams = false;
        }

        [Command("!challenge")]
        [RequireChannelConfigured]
        public async Task ChallengeAsync(string teamCode)
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription(_channelMatchService.Challenge(Context.Channel.Id, teamCode, Context.User.Mention)).Build());
            this.sendUpdatedTeams = false;
        }

        [Command("!challenges")]
        [Alias("!listchallenges", "!challengelist", "!challenges")]
        [RequireChannelConfigured]
        public async Task ListChallengesAsync()
        {
            await ReplyAsync("", embed: _channelMatchService.ListChallenges(Context.Channel.Id));
            this.sendUpdatedTeams = false;
        }

        [Command("!unchallenge")]
        [RequireChannelConfigured]
        public async Task UnchallengeAsync()
        {
            await ReplyAsync("", embed: _channelMatchService.Unchallenge(Context.Channel.Id, Context.User.Mention));
        }

        [Command("!sub")]
        public async Task SubAsync()
        {
            await ReplyAsync("", embed: _channelMatchService.AddSub(Context.Channel.Id, Context.User));
        }

        [Command("!unsub")]
        public async Task UnsubAsync()
        {
            await ReplyAsync("", embed: _channelMatchService.RemoveSub(Context.Channel.Id, Context.User));
        }

        [Command("!unsub")]
        public async Task UnsubAsync([Remainder]string playerName)
        {
            await ReplyAsync("", embed: _channelMatchService.RemoveSub(Context.Channel.Id, playerName));
        }

        [Command("!requestsub")]
        [RequireChannelConfigured]
        public async Task CallSubAsync()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription($":no_entry: Please provide a server id & required position, e.g. **!requestsub 3 LW**").WithCurrentTimestamp().Build());
            this.sendUpdatedTeams = false;
        }

        [Command("!requestsub", RunMode = RunMode.Async)]
        [RequireChannelConfigured]
        public async Task VsAsync(int serverId, [Remainder]string positionName = null)
        {
            
            if (_channelServerService.ValidateServer(Context.Channel.Id, serverId))
            {
                var ticks = DateTime.Now.Ticks.ToString().GetHashCode().ToString("x");
                await ReplyAsync("@here");
                var messageContents = $"A substitute{(string.IsNullOrEmpty(positionName) ? "" : " **" + positionName + "**")} is required urgently. Type **!acceptsub {ticks}** to sub in.";
                var message = await ReplyAsync("", embed: EmbedTools.GenerateSimpleEmbed(":bell: " + messageContents));

                var acceptSubCriterion = new EnsureMatchCommandCriterion($"!acceptsub {ticks}") as ICriterion<SocketMessage>;
                var subResponse = await NextMessageAsync(acceptSubCriterion, timeout: new TimeSpan(0, 10, 0));
                if (subResponse != null)
                {
                    var response = _channelMatchService.MakeSub(Context.Message.Channel.Id, serverId, subResponse.Author);
                    await ReplyAsync("", embed: response);
                    //await ReplyAsync("", embed: EmbedTools.GenerateSimpleEmbed($":repeat: {subResponse.Author.Mention} comes off the bench on {"));
                    await message.ModifyAsync(m => m.Embed = EmbedTools.GenerateSimpleEmbed($":no_bell: ~~{messageContents}~~"));
                }
                else
                {
                    await ReplyAsync($":timer: Your substitution search has timed out, {message.Author.Mention}");
                }
            } 
            else
            {
                await ReplyAsync($":no_entry: Please provide a valid server");
            }
            
            this.sendUpdatedTeams = false;
        }
    }
}
