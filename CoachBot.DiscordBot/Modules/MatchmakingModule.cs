using CoachBot.Bot.Preconditions;
using CoachBot.Domain.Model;
using CoachBot.Extensions;
using CoachBot.Preconditions;
using CoachBot.Services;
using CoachBot.Tools;
using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoachBot.Modules.Matchmaker
{
    [RequireChannelAndTeamActive]
    public class MatchmakingModule : ModuleBase
    {
        private readonly MatchmakingService _channelMatchService;
        private readonly ServerManagementService _channelServerService;
        private readonly CacheService _cacheService;

        public MatchmakingModule(MatchmakingService channelMatchService, ServerManagementService channelServerService, CacheService cacheService)
        {
            _channelMatchService = channelMatchService;
            _channelServerService = channelServerService;
            _cacheService = cacheService;
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);
            Context.Message.AddReactionAsync(new Emoji("⚙️"));
            CallContext.SetData(CallContextDataType.DiscordUser, Context.Message.Author.Username);
        }

        protected override void AfterExecute(CommandInfo command)
        {
            base.AfterExecute(command);

            Context.Message.AddReactionAsync(new Emoji("✅"));

            if (command.Attributes.Any(a => a.GetType() == typeof(SendLineupMessage)))
            {
                foreach (var teamEmbed in _channelMatchService.GenerateTeamList(Context.Channel.Id))
                {
                    ReplyAsync("", embed: teamEmbed);
                }
            }
        }

        [Command("!sign")]
        [Alias("!s")]
        [RequireChannelConfigured]
        [SendLineupMessage]
        public async Task SignAsync(string positionName)
        {
            var response = _channelMatchService.AddPlayer(Context.Message.Channel.Id, Context.Message.Author, positionName);
            await ReplyAsync("", embed: response);
        }

        [Command("!sign")]
        [Alias("!s")]
        [RequireChannelConfigured]
        [SendLineupMessage]
        public async Task SignAsync()
        {
            var response = _channelMatchService.AddPlayer(Context.Message.Channel.Id, Context.Message.Author);
            await ReplyAsync("", embed: response);
        }

        [Command("!sign")]
        [Alias("!s")]
        [RequireChannelConfigured]
        [SendLineupMessage]
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
        [SendLineupMessage]
        public async Task SignTeam2Async()
        {
            var response = _channelMatchService.AddPlayer(Context.Message.Channel.Id, Context.Message.Author, null, ChannelTeamType.TeamTwo);
            await ReplyAsync("", embed: response);
        }

        [Command("!sign2")]
        [Alias("!s2")]
        [RequireChannelConfigured]
        [SendLineupMessage]
        public async Task SignTeam2Async(string positionName)
        {
            var response = _channelMatchService.AddPlayer(Context.Message.Channel.Id, Context.Message.Author, positionName, ChannelTeamType.TeamTwo);
            await ReplyAsync("", embed: response);
        }

        [Command("!sign2")]
        [Alias("!s2")]
        [RequireChannelConfigured]
        [SendLineupMessage]
        public async Task CounterSignTeam2Async(string position, [Remainder]string name)
        {
            if (DiscordTools.IsMention(name))
            {
                var user = await Context.Guild.GetUserAsync(DiscordTools.ConvertMentionToUserID(name));
                var response = _channelMatchService.AddPlayer(Context.Message.Channel.Id, user, position, ChannelTeamType.TeamTwo);
                await ReplyAsync("", embed: response);
            }
            else
            {
                var response = _channelMatchService.AddPlayer(Context.Message.Channel.Id, name, position, ChannelTeamType.TeamTwo);
                await ReplyAsync("", embed: response);
            }
        }

        [Command("!unsign")]
        [Alias("!us", "!u", "!r", "!remove")]
        [RequireChannelConfigured]
        [SendLineupMessage]
        public async Task UnsignAsync()
        {
            var response = _channelMatchService.RemovePlayer(Context.Message.Channel.Id, Context.Message.Author);
            await ReplyAsync("", embed: response);
        }

        [Command("!unsign")]
        [Alias("!us", "!u", "!r", "!remove")]
        [RequireChannelConfigured]
        [SendLineupMessage]
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
        [SendLineupMessage]
        public async Task UnsignPositionHomeTeamAsync(string position)
        {
            var response = _channelMatchService.ClearPosition(Context.Message.Channel.Id, position, ChannelTeamType.TeamOne);
            await ReplyAsync("", embed: response);
        }

        [Command("!unsignpos2")]
        [Alias("!up2", "!clearpos2")]
        [RequireChannelConfigured]
        [SendLineupMessage]
        public async Task UnsignPositionAwayTeamAsync(string position)
        {
            var response = _channelMatchService.ClearPosition(Context.Message.Channel.Id, position, ChannelTeamType.TeamTwo);
            await ReplyAsync("", embed: response);
        }

        [Command("!reset")]
        [RequireChannelConfigured]
        [SendLineupMessage]
        public async Task ResetChannelAsync(params string[] positions)
        {
            var response = _channelMatchService.Reset(Context.Message.Channel.Id);
            await ReplyAsync("", embed: response);
        }

        [Command("!ready")]
        [RequireChannelConfigured]
        public async Task ReadyAsync()
        {
            await ReplyAsync("", embed: EmbedTools.GenerateEmbed("Invalid server provided. Please try again in the format `!ready server-id-here`, e.g. `!ready 5`. Type `!servers` for the server list.", ServiceResponseStatus.Failure));
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
                await ReplyAsync("", embed: EmbedTools.GenerateEmbed("Invalid server provided. Please try again in the format `!ready server-id-here`, e.g. `!ready 5`. Type `!servers` for the server list.", ServiceResponseStatus.Failure));
            }
        }

        [Command("!setupserver")]
        [RequireChannelConfigured]
        public async Task SetupServerAsync(int serverListItemId, int matchId)
        {
            if (_channelServerService.ValidateServer(Context.Channel.Id, serverListItemId))
            {
                _channelServerService.PrepareServer(serverListItemId, Context.Channel.Id, matchId);
            }
            else
            {
                await ReplyAsync("", embed: EmbedTools.GenerateEmbed("Invalid server provided. Please try again in the format `!ready server-id-here`, e.g. `!ready 5`. Type `!servers` for the server list.", ServiceResponseStatus.Failure));
            }
        }

        [Command("!list")]
        [Alias("!lineup")]
        [RequireChannelConfigured]
        [SendLineupMessage]
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
                    await ReplyAsync("", embed: EmbedTools.GenerateEmbed($"The last highlight was less than {HERE_INTERVAL} minutes ago. Please try again in {timeRemaining.ToString("0")} minutes.", ServiceResponseStatus.Failure));
                }
                else
                {
                    await ReplyAsync("", embed: EmbedTools.GenerateEmbed($"The last highlight was less than {HERE_INTERVAL} minutes ago. Please try again in 1 minute.", ServiceResponseStatus.Failure));
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
            await ReplyAsync("", embed: await _channelMatchService.Search(Context.Channel.Id, Context.User.Mention));
        }

        [Command("!stopsearch")]
        [RequireChannelConfigured]
        public async Task StopSearchOppositionAsync()
        {
            await ReplyAsync("", embed: await _channelMatchService.StopSearch(Context.Message.Channel.Id));
        }

        [Command("!challenge")]
        [Alias("!vs")]
        [RequireChannelConfigured]
        public async Task ChallengeAsync()
        {
            await ReplyAsync("", embed: EmbedTools.GenerateEmbed("You must provide a valid team code, or type !challenges to see the currently available challenges", ServiceResponseStatus.Failure));
        }

        [Command("!challenge")]
        [Alias("!vs")]
        [RequireChannelConfigured]
        public async Task ChallengeAsync([Remainder]string teamCode)
        {
            await ReplyAsync("", embed: _channelMatchService.Challenge(Context.Channel.Id, teamCode, Context.User.Mention));
        }

        [Command("!challenges")]
        [Alias("!listchallenges", "!challengelist", "!challenges", "!searches")]
        [RequireChannelConfigured]
        public async Task ListChallengesAsync()
        {
            await ReplyAsync("", embed: _channelMatchService.ListChallenges(Context.Channel.Id));
        }

        [Command("!unchallenge")]
        [RequireChannelConfigured]
        [SendLineupMessage]
        public async Task UnchallengeAsync()
        {
            await ReplyAsync("", embed: await _channelMatchService.Unchallenge(Context.Channel.Id, Context.User.Mention));
        }

        [Command("!sub")]
        [SendLineupMessage]
        public async Task SubAsync()
        {
            await ReplyAsync("", embed: _channelMatchService.AddSub(Context.Channel.Id, Context.User));
        }

        [Command("!unsub")]
        [SendLineupMessage]
        public async Task UnsubAsync()
        {
            await ReplyAsync("", embed: _channelMatchService.RemoveSub(Context.Channel.Id, Context.User));
        }

        [Command("!unsub")]
        [SendLineupMessage]
        public async Task UnsubAsync([Remainder]string playerName)
        {
            await ReplyAsync("", embed: _channelMatchService.RemoveSub(Context.Channel.Id, playerName));
        }

        [Command("!requestsub")]
        [RequireChannelConfigured]
        public async Task RequestSubAsync()
        {
            await ReplyAsync("", embed: EmbedTools.GenerateEmbed("Please provide a server id & required position, e.g. **!requestsub 3 LW**", ServiceResponseStatus.Failure));
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
                await ReplyAsync("", embed: EmbedTools.GenerateEmbed($"Please provide a valid server. Use `!servers` to see the full server list.", ServiceResponseStatus.Failure));
            }
        }

        [Command("!acceptsub")]
        [RequireChannelConfigured]
        public async Task AcceptSubRequestAsync(string subToken = null)
        {
            if (string.IsNullOrWhiteSpace(subToken))
            {
                await ReplyAsync("", embed: EmbedTools.GenerateEmbed("Please provide the sub request token, e.g. `!acceptsub dea53af`", ServiceResponseStatus.Failure));
            }
            else
            {
                await ReplyAsync("", embed: _channelMatchService.AcceptSubRequest(subToken, Context.Message.Author));
            }
        }

        [Command("!cancelsub")]
        [Alias("!cancelrequestsub", "!cancelsubrequest")]
        [RequireChannelConfigured]
        public async Task CancelSubRequestAsync(string subToken = null)
        {
            if (string.IsNullOrWhiteSpace(subToken))
            {
                await ReplyAsync("", embed: EmbedTools.GenerateEmbed("Please provide the sub request token, e.g. `!cancelsub dea53af`", ServiceResponseStatus.Failure));
            }
            else
            {
                await ReplyAsync("", embed: _channelMatchService.CancelSubRequest(subToken, Context.Message.Author));
            }
        }

        [Command("!recentmatches")]
        public async Task RecentMatchesAsync()
        {
            await ReplyAsync("", embed: _channelMatchService.GenerateRecentMatchList(Context.Channel.Id));
        }
    }
}