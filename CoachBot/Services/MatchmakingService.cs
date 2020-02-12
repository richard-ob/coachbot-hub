using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Factories;
using CoachBot.Model;
using CoachBot.Tools;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoachBot.Services
{
    public class MatchmakingService
    {
        private readonly MatchService _matchService;
        private readonly ChannelService _channelService;
        private readonly ServerService _serverService;
        private readonly SearchService _searchService;
        private readonly PlayerService _playerService;
        private readonly SubstitutionService _substitutionService;
        private readonly CacheService _cacheService;
        private readonly DiscordNotificationService _discordNotificationService;
        private readonly DiscordSocketClient _discordClient;

        public MatchmakingService(
            ChannelService channelService,
            MatchService matchService,
            ServerService serverService,
            SearchService searchService,
            PlayerService playerService,
            SubstitutionService substitutionService,
            CacheService cacheService,
            DiscordSocketClient discordClient,
            DiscordNotificationService discordNotificationService)
        {
            _matchService = matchService;
            _channelService = channelService;
            _serverService = serverService;
            _searchService = searchService;
            _playerService = playerService;
            _substitutionService = substitutionService;
            _cacheService = cacheService;
            _discordNotificationService = discordNotificationService;
            _discordClient = discordClient;
        }

        public Embed Reset(ulong channelId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var response = _matchService.CreateMatch(channel.Id);

            ResetLastMentionTime(channelId);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed AddPlayer(ulong channelId, IUser user, string positionName = null, ChannelTeamType channelTeamType = ChannelTeamType.TeamOne)
        {
            var player = _playerService.GetPlayer(user, createIfNotExists: true);
            var teamType = _channelService.GetTeamTypeForChannelTeamType(channelTeamType, channelId);
            var response = _matchService.AddPlayerToTeam(channelId, player, positionName, teamType);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed AddPlayer(ulong channelId, string userName, string positionName = null, ChannelTeamType channelTeamType = ChannelTeamType.TeamOne)
        {
            var player = _playerService.GetPlayer(userName, createIfNotExists: true);
            var teamType = _channelService.GetTeamTypeForChannelTeamType(channelTeamType, channelId);
            var response = _matchService.AddPlayerToTeam(channelId, player, positionName, teamType);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed RemovePlayer(ulong channelId, IUser user)
        {
            var player = _playerService.GetPlayer(user);
            var response = _matchService.RemovePlayerFromMatch(channelId, player);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed RemovePlayer(ulong channelId, string userName)
        {
            var player = _playerService.GetPlayer(userName);
            var response = _matchService.RemovePlayerFromMatch(channelId, player);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed ClearPosition(ulong channelId, string position, ChannelTeamType channelTeamType = ChannelTeamType.TeamOne)
        {
            var teamType = _channelService.GetTeamTypeForChannelTeamType(channelTeamType, channelId);
            var response = _matchService.ClearPosition(channelId, position, teamType);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public bool ReadyMatch(ulong channelId, int serverListItemId, out int readiedMatchId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var servers = _serverService.GetServersByRegion((int)channel.RegionId);
            var match = _matchService.GetCurrentMatchForChannel(channelId);
            var server = GetServerFromServerListItemId(serverListItemId, channelId);
            var discordChannel = _discordClient.GetChannel(channelId) as ITextChannel;

            var serviceResponse = _matchService.ReadyMatch(channelId, server.Id, out int matchId);
            readiedMatchId = matchId;

            if (serviceResponse.Status != ServiceResponseStatus.Success)
            {
                discordChannel.SendMessageAsync("", embed: EmbedTools.GenerateEmbedFromServiceResponse(serviceResponse));

                return false;
            }
            else
            {
                SendReadyMessageForTeam(match, match.TeamHome, server);
                if (!match.IsMixMatch) SendReadyMessageForTeam(match, match.TeamAway, server);
                SendReadyMessageForPlayers(match, match.SignedPlayers, server);

                return true;
            }
        }

        public List<Embed> GenerateTeamList(ulong channelId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var match = _matchService.GetCurrentMatchForChannel(channelId);

            if (!channel.UseClassicLineup) return GenerateTeamSheet(channelId);

            var teamType = _channelService.GetTeamTypeForChannelTeamType(ChannelTeamType.TeamOne, channelId);
            var teamLists = new List<Embed>() {
                TeamListEmbedFactory.GenerateEmbed(channel, match, teamType)
            };

            if (match.IsMixMatch)
            {
                var awayTeamList = TeamListEmbedFactory.GenerateEmbed(channel, match, TeamType.Away);
                teamLists.Add(awayTeamList);
            }

            return teamLists;
        }

        private List<Embed> GenerateTeamSheet(ulong channelId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var match = _matchService.GetCurrentMatchForChannel(channelId);

            if (channel.UseClassicLineup) return GenerateTeamList(channelId);

            var teamType = _channelService.GetTeamTypeForChannelTeamType(ChannelTeamType.TeamOne, channelId);
            var teamSheets = new List<Embed>() {
                TeamSheetEmbedFactory.GenerateEmbed(channel, match, teamType)
            };

            if (match.IsMixMatch)
            {
                var awayTeamSheet = TeamSheetEmbedFactory.GenerateEmbed(channel, match, TeamType.Away);
                teamSheets.Add(awayTeamSheet);
            }

            return teamSheets;
        }

        public async Task<Embed> Search(ulong channelId, string challengerMention)
        {
            var challenger = _channelService.GetChannelByDiscordId(channelId);
            var response = await _searchService.Search(challenger.Id, challengerMention);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public async Task<Embed> StopSearch(ulong channelId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var response = await _searchService.StopSearch(channel.Id);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed Challenge(ulong challengerChannelId, string teamCode, string challengerMention)
        {
            var challenger = _channelService.GetChannelByDiscordId(challengerChannelId);
            var opposition = _channelService.GetChannelByTeamCode(teamCode);

            if (opposition == null) return EmbedTools.GenerateEmbed($"{teamCode} is not a valid team code", ServiceResponseStatus.Failure);

            var response = _matchService.Challenge(challengerChannelId, opposition.DiscordChannelId, challengerMention);

            if (response.Status != ServiceResponseStatus.Success) return EmbedTools.GenerateEmbedFromServiceResponse(response);

            var acceptMessage = $":handshake: {challenger.Name} have accepted the challenge! Contact {challengerMention} to arrange further.";
            (_discordClient.GetChannel(opposition.DiscordChannelId) as SocketTextChannel).SendMessageAsync("", embed: EmbedTools.GenerateSimpleEmbed(acceptMessage));

            (_discordClient.GetChannel(challengerChannelId) as SocketTextChannel).SendMessageAsync("", embed: GenerateTeamList(challengerChannelId).First());
            (_discordClient.GetChannel(opposition.DiscordChannelId) as SocketTextChannel).SendMessageAsync("", embed: GenerateTeamList(opposition.DiscordChannelId).First());            

            return EmbedTools.GenerateSimpleEmbed($":handshake: You have successfully challenged {opposition.Name}. `!ready` will send both teams to the server");
        }

        public Embed ListChallenges(ulong challengerChannelId)
        {
            var channel = _channelService.GetChannelByDiscordId(challengerChannelId);
            var searches = _searchService.GetSearches().Where(s => s.Channel.RegionId == channel.RegionId && s.ChannelId != channel.Id);
            var mixChallenges = _channelService.GetChannels().Where(c => c.IsMixChannel && c.RegionId == channel.RegionId);

            var embedBuilder = new EmbedBuilder();

            embedBuilder.WithTitle($"Available Challenges");

            var teamList = new StringBuilder();
            foreach (var search in searches)
            {
                var match = _matchService.GetCurrentMatchForChannel(search.Channel.DiscordChannelId);
                if (!string.IsNullOrEmpty(search.Channel.BadgeEmote)) teamList.Append($"{search.Channel.BadgeEmote} ");
                teamList.Append($"**{search.Channel.TeamCode}** ");
                if (!match.TeamHome.HasGk) teamList.Append("(No GK)");
                teamList.AppendLine("");
                teamList.AppendLine(GenerateFormEmoteListForChannel(search.Channel.DiscordChannelId));
                var searchMinutesAgo = search.CreatedDate.Subtract(DateTime.Now).TotalMinutes.ToString("0");
                if (searchMinutesAgo == "0" || searchMinutesAgo == "1")
                {
                    teamList.AppendLine($"*Search started just now*");
                }
                else
                {
                    teamList.AppendLine($"*Searching for {DateTime.Now.Subtract(search.CreatedDate).TotalMinutes.ToString("0")} minutes*");
                }
                teamList.AppendLine($"");
            }
            embedBuilder.AddField("**Teams**", teamList.ToString().NullIfEmpty() ?? "*There are no teams currently searching*");

            var mixTeamList = new StringBuilder();
            foreach (var mix in mixChallenges)
            {
                var match = _matchService.GetCurrentMatchForChannel(mix.DiscordChannelId);
                if (match.IsMixMatch)
                {
                    if (!string.IsNullOrEmpty(match.TeamHome.Channel.BadgeEmote)) mixTeamList.Append($"{match.TeamHome.Channel.BadgeEmote} ");
                    mixTeamList.Append($"**{mix.TeamCode}** ");
                    if (string.IsNullOrEmpty(match.TeamHome.Channel.BadgeEmote)) mixTeamList.Append($"({match.TeamHome.Channel.Guild}) ");
                    if (!match.TeamHome.HasGk) mixTeamList.Append(" (No GK)");
                    mixTeamList.AppendLine("");
                    mixTeamList.AppendLine(GenerateFormEmoteListForChannel(match.TeamHome.Channel.DiscordChannelId));
                    mixTeamList.AppendLine($"*{match.SignedPlayersAndSubs.Count} players currently signed*");
                    mixTeamList.AppendLine($"");
                }
            }
            embedBuilder.AddField("**Mixes**", mixTeamList.ToString().NullIfEmpty() ?? "*There are no mix teams currently available*");

            return embedBuilder.WithDefaultColour().WithRequestedBy().Build();
        }

        public async Task<Embed> Unchallenge(ulong challengerChannelId, string challengerMention)
        {
            var match = _matchService.GetCurrentMatchForChannel(challengerChannelId);
            var homeChannel = match.TeamHome?.Channel;
            var awayChannel = match.TeamAway?.Channel;

            var response = _matchService.Unchallenge(challengerChannelId);

            if (response.Status == ServiceResponseStatus.NegativeSuccess)
            {
                var unchallengeMessage = $"The game between **{homeChannel.Name}** & **{awayChannel.Name}** has been called off by **{challengerMention}**";
                var embed = EmbedTools.GenerateSimpleEmbed(unchallengeMessage, ":thunder_cloud_rain: Match Abandoned!");
                await _discordNotificationService.SendChannelMessage(homeChannel.DiscordChannelId, embed: embed);
                await _discordNotificationService.SendChannelMessage(awayChannel.DiscordChannelId, embed: embed);
            }

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed AddSub(ulong channelId, IUser user)
        {
            var player = _playerService.GetPlayer(user);
            var teamType = _channelService.GetTeamTypeForChannelTeamType(ChannelTeamType.TeamOne, channelId);

            var response = _matchService.AddSubsitutePlayerToTeam(channelId, player, teamType);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed RemoveSub(ulong channelId, IUser user)
        {
            var player = _playerService.GetPlayer(user);

            var response = _matchService.RemoveSubstitutePlayerFromMatch(channelId, player);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed RemoveSub(ulong channelId, string userName)
        {
            var player = _playerService.GetPlayer(userName);

            var response = _matchService.RemoveSubstitutePlayerFromMatch(channelId, player);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed RequestSub(ulong channelId, int serverListItemId, string positionName, IUser user)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var server = _serverService.GetServersByRegion((int)channel.RegionId)[serverListItemId - 1];

            var response = _substitutionService.CreateRequest(channel.Id, server.Id, positionName);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed AcceptSubRequest(string requestToken, IUser user)
        {
            var player = _playerService.GetPlayer(user);

            var response = _substitutionService.AcceptSubstitution(requestToken, player);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed CancelSubRequest(string requestToken, IUser user)
        {
            var player = _playerService.GetPlayer(user);

            var response = _substitutionService.CancelSubstitution(requestToken, player);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed GenerateRecentMatchList(ulong channelId)
        {
            var recentMatches = _matchService.GetMatchesForChannel(channelId, true, 10);
            var embedBuilder = new EmbedBuilder().WithTitle(":calendar_spiral: Recent Matches");
            if (recentMatches == null || !recentMatches.Any()) return new EmbedBuilder().WithDescription(":information_source: No matches have been played yet. Chill.").Build();
            foreach (var recentMatch in recentMatches)
            {
                var playerList = "";
                if (recentMatch.TeamHome.Channel.DiscordChannelId == channelId) playerList = string.Join(", ", recentMatch.TeamHome.PlayerTeamPositions.Select(ptp => ptp.Player.Name));
                if (recentMatch.TeamAway.Channel.DiscordChannelId == channelId) playerList = $"{(playerList == "" ? "" : ", ")} {string.Join(", ", recentMatch.TeamAway.PlayerTeamPositions.Select(ptp => ptp.Player.Name))}";
                if (playerList == "") playerList = "No player data available";

                embedBuilder.AddField($"**{recentMatch.TeamHome.Channel.TeamCode}** vs **{recentMatch.TeamAway.Channel.TeamCode}** - {recentMatch.ReadiedDate.ToString()}", playerList);
            }

            return embedBuilder.WithRequestedBy().WithDefaultColour().Build();
        }

        private void ResetLastMentionTime(ulong channelId)
        {
            _cacheService.Remove(CacheService.CacheItemType.LastMention, channelId.ToString());
        }

        private Server GetServerFromServerListItemId(int serverListItemId, ulong channelId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId, false);
            var servers = _serverService.GetServersByRegion((int)channel.RegionId);

            return servers[serverListItemId - 1];
        }

        private async void SendReadyMessageForPlayers(Match match, List<Player> players, Server server)
        {
            var message = $":soccer: Match ready! **{match.TeamHome.Channel.DisplayName}** vs **{match.TeamAway.Channel.DisplayName}** - Please join **{server.Name}** (steam://connect/{server.Address}) as soon as possible.";
            foreach(var player in players.Where(p => p.DiscordUserId != null && !p.DisableDMNotifications))
            {
               await _discordNotificationService.SendUserMessage((ulong)player.DiscordUserId, message);
            }
        }

        private async void SendReadyMessageForTeam(Match match, Team team, Server server)
        {
            var discordChannel = _discordClient.GetChannel(team.Channel.DiscordChannelId) as ITextChannel;

            var embed = EmbedTools.GenerateSimpleEmbed($"Please join **{server.Name}** (steam://connect/{server.Address}) as soon as possible.", $":soccer: Kick Off! **{match.TeamHome.Channel.DisplayName}** vs **{match.TeamAway.Channel.DisplayName}**");
            await discordChannel.SendMessageAsync("", embed: embed);

            var highlightMessage = string.Join(", ", team.PlayerTeamPositions.Where(ptp => ptp.Player.DiscordUserId != null).Select(ptp => ptp.Player.DisplayName));
            if (!string.IsNullOrEmpty(highlightMessage)) await discordChannel.SendMessageAsync(highlightMessage);
        }

        private string GenerateFormEmoteListForChannel(ulong channelId)
        {
            const string formWinEmote = "<:form_win_sm:676578014188011520>";
            const string formLossEmote = "<:form_loss_sm:676578012438855692>";
            const string formDrawEmote = "<:form_draw_sm:676578012380135434>";
            const string formUnknownEmote = "<:form_unknown_sm:676936524998377492>";

            var formList = _matchService.GetFormForChannel(channelId, 5);
            var sb = new StringBuilder();

            foreach(var formItem in formList)
            {
                switch (formItem)
                {
                    case MatchOutcomeType.Win:
                        sb.Append($"{formWinEmote} ");
                        break;
                    case MatchOutcomeType.Loss:
                        sb.Append($"{formLossEmote} ");
                        break;
                    case MatchOutcomeType.Draw:
                        sb.Append($"{formDrawEmote} ");
                        break;
                }
            }

            if (formList.Count() < 5)
            {                
                for(var unknownItems = 5 - formList.Count(); unknownItems > 0; unknownItems--)
                {
                    sb.Append($"{formUnknownEmote} ");
                }
            }

            return sb.ToString();
        }
    }
}
