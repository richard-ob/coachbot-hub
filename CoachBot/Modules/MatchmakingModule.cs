using Discord.Commands;
using System.Threading.Tasks;
using CoachBot.Preconditions;
using CoachBot.Services;
using CoachBot.Model;
using Discord;
using System;
using CoachBot.Tools;

namespace CoachBot.Modules.Matchmaker
{
    public class MatchmakingModule : ModuleBase
    {
        private readonly MatchmakingService _channelMatchService;
        private readonly ServerManagementService _channelServerService;
        private readonly CacheService _cacheService;
        private bool sendUpdatedTeams = true;

        public MatchmakingModule(MatchmakingService channelMatchService, ServerManagementService channelServerService, CacheService cacheService)
        {
            _channelMatchService = channelMatchService;
            _channelServerService = channelServerService;
            _cacheService = cacheService;
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
            if (DiscordTools.IsMention(name))
            {
                var user = await Context.Guild.GetUserAsync(DiscordTools.ConvertMentionToUserID(name));
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
            if (DiscordTools.IsMention(name))
            {
                var user = await Context.Guild.GetUserAsync(DiscordTools.ConvertMentionToUserID(name));
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
            if (DiscordTools.IsMention(name))
            {
                var user = await Context.Guild.GetUserAsync(DiscordTools.ConvertMentionToUserID(name));
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
            if (_channelServerService.ValidateServer(Context.Channel.Id, serverListItemId))
            {
                var serverAvailable = await _channelServerService.ValidateServerAvailability(serverListItemId, Context.Message.Channel.Id);
                if (serverAvailable)
                {
                    var success = _channelMatchService.ReadyMatch(Context.Message.Channel.Id, serverListItemId, out int readiedMatchId);
                    if (success)
                    {
                        _channelServerService.PrepareServer(serverListItemId, Context.Channel.Id, readiedMatchId);
                    }
                }
            }
            else
            {
                await ReplyAsync("", embed: EmbedTools.GenerateSimpleEmbed(":no_entry: Invalid server ID provided"));
            }

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
            const int HERE_INTERVAL = 10;
            var lastMention = _cacheService.Get(CacheService.CacheItemType.LastMention, Context.Channel.Id.ToString()) as DateTime?;
            if (lastMention != null && lastMention.Value.AddMinutes(HERE_INTERVAL) > DateTime.Now)
            {
                var timeRemaining = lastMention.Value.AddMinutes(HERE_INTERVAL).Subtract(DateTime.Now).TotalMinutes;
                if (timeRemaining >= 1)
                {
                    await ReplyAsync("", embed: EmbedTools.GenerateSimpleEmbed($"The last highlight was less than {HERE_INTERVAL} minutes ago. Please try again in {timeRemaining.ToString("0")} minutes."));
                }
                else
                {
                    await ReplyAsync("", embed: EmbedTools.GenerateSimpleEmbed($"The last highlight was less than {HERE_INTERVAL} minutes ago. Please try again in 1 minute."));
                }
            }
            else
            {
                await ReplyAsync("@here");
                _cacheService.Set(CacheService.CacheItemType.LastMention, Context.Channel.Id.ToString(), DateTime.Now);
            }
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
        [Alias("!vs")]
        [RequireChannelConfigured]
        public async Task ChallengeAsync()
        {
            await ReplyAsync("You must provide a valid team code, or type !challenges to see the currently available challenges");
            this.sendUpdatedTeams = false;
        }

        [Command("!challenge")]
        [Alias("!vs")]
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
        public async Task RequestSubAsync()
        {
            await ReplyAsync("", embed: new EmbedBuilder().WithDescription($":no_entry: Please provide a server id & required position, e.g. **!requestsub 3 LW**").WithCurrentTimestamp().Build());
            this.sendUpdatedTeams = false;
        }

        [Command("!requestsub")]
        [RequireChannelConfigured]
        public async Task RequestSubAsync(int serverId, [Remainder]string positionName = null)
        {            
            if (_channelServerService.ValidateServer(Context.Channel.Id, serverId))
            {
                await ReplyAsync("", embed: _channelMatchService.RequestSub(Context.Channel.Id, serverId, positionName, Context.Message.Author));
            }
            else
            {
                await ReplyAsync($":no_entry: Please provide a valid server, {Context.Message.Author.Mention}");
            }
            
            this.sendUpdatedTeams = false;
        }

        [Command("!acceptsub")]
        [RequireChannelConfigured]
        public async Task AcceptSubRequestAsync(string subToken = null)
        {
            if (string.IsNullOrWhiteSpace(subToken))
            {
                await ReplyAsync($":no_entry: Please provide the sub request token, e.g. `!acceptsub dea53af`");
            }
            else
            {
                await ReplyAsync("", embed: _channelMatchService.AcceptSubRequest(subToken, Context.Message.Author));
            }

            this.sendUpdatedTeams = false;
        }

        [Command("!cancelsub")]
        [Alias("!cancelrequestsub", "!cancelsubrequest")]
        [RequireChannelConfigured]
        public async Task CancelSubRequestAsync(string subToken = null)
        {
            if (string.IsNullOrWhiteSpace(subToken))
            {
                await ReplyAsync($":no_entry: Please provide the sub request token, e.g. `!acceptsub dea53af`");
            }
            else
            {
                await ReplyAsync("", embed: _channelMatchService.CancelSubRequest(subToken, Context.Message.Author));
            }

            this.sendUpdatedTeams = false;
        }

        [Command("!recentmatches")]
        public async Task RecentMatchesAsync()
        {
            await ReplyAsync("", embed: _channelMatchService.GenerateRecentMatchList(Context.Channel.Id));

            this.sendUpdatedTeams = false;
        }
    }
}
