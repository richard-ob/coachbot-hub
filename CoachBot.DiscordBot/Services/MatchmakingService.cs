using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Factories;
using CoachBot.Model;
using CoachBot.Shared.Extensions;
using CoachBot.Shared.Model;
using CoachBot.Shared.Services;
using CoachBot.Tools;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChannelType = CoachBot.Domain.Model.ChannelType;

namespace CoachBot.Services
{
    public class MatchmakingService
    {
        private readonly MatchupService _matchupService;
        private readonly ChannelService _channelService;
        private readonly ServerService _serverService;
        private readonly SearchService _searchService;
        private readonly PlayerService _playerService;
        private readonly SubstitutionService _substitutionService;
        private readonly ServerManagementService _serverManagementServiceService;
        private readonly CacheService _cacheService;
        private readonly DiscordNotificationService _discordNotificationService;
        private readonly Config _config;
        private readonly DiscordSocketClient _discordClient;

        public MatchmakingService(
            ChannelService channelService,
            MatchupService matchupService,
            ServerService serverService,
            SearchService searchService,
            PlayerService playerService,
            SubstitutionService substitutionService,
            ServerManagementService serverManagementServiceService,
            CacheService cacheService,
            DiscordSocketClient discordClient,
            DiscordNotificationService discordNotificationService,
            Config config)
        {
            _matchupService = matchupService;
            _channelService = channelService;
            _serverService = serverService;
            _searchService = searchService;
            _playerService = playerService;
            _substitutionService = substitutionService;
            _serverManagementServiceService = serverManagementServiceService;
            _cacheService = cacheService;
            _discordNotificationService = discordNotificationService;
            _config = config;
            _discordClient = discordClient;
        }

        public Embed Reset(ulong channelId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var currentMatchup = _matchupService.GetCurrentMatchupForChannel(channelId);

            if ((currentMatchup.LineupHome != null && currentMatchup.LineupHome.ChannelId.HasValue && currentMatchup.LineupHome.ChannelId != channel.Id)
                || (currentMatchup.LineupAway != null && currentMatchup.LineupAway.ChannelId.HasValue && currentMatchup.LineupAway.ChannelId != channel.Id))
            {
                return DiscordEmbedHelper.GenerateEmbed("Cannot reset the lineup as a challenge is in progress. Use `!unchallenge` to abandon.", ServiceResponseStatus.Failure);
            }

            var response = _matchupService.CreateMatchup(channel.Id);

            ResetLastMentionTime(channelId);

            return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);
        }

        public Embed AddPlayer(ulong channelId, IUser user, string positionName = null, ChannelTeamType channelTeamType = ChannelTeamType.TeamOne)
        {
            var player = _playerService.GetPlayer(user, createIfNotExists: true);
            var teamType = _channelService.GetTeamTypeForChannelTeamType(channelTeamType, channelId);
            var response = _matchupService.AddPlayerToLineup(channelId, player, positionName, teamType);

            return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);
        }

        public Embed AddPlayer(ulong channelId, string userName, string positionName = null, ChannelTeamType channelTeamType = ChannelTeamType.TeamOne)
        {
            var player = _playerService.GetPlayer(userName, createIfNotExists: true);
            var teamType = _channelService.GetTeamTypeForChannelTeamType(channelTeamType, channelId);
            var response = _matchupService.AddPlayerToLineup(channelId, player, positionName, teamType);

            return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);
        }

        public Embed RemovePlayer(ulong channelId, int playerId)
        {
            var player = _playerService.GetPlayer(playerId);
            var response = _matchupService.RemovePlayerFromMatch(channelId, player);

            return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);
        }

        public Embed RemovePlayer(ulong channelId, IUser user)
        {
            var player = _playerService.GetPlayer(user);
            var response = _matchupService.RemovePlayerFromMatch(channelId, player);

            return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);
        }

        public Embed RemovePlayer(ulong channelId, string userName)
        {
            var player = _playerService.GetPlayer(userName);

            if (player == null)
            {

                return DiscordEmbedHelper.GenerateEmbed($"**{userName}** is not signed", ServiceResponseStatus.Failure);
            }

            var response = _matchupService.RemovePlayerFromMatch(channelId, player);

            return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);
        }

        public Embed ClearPosition(ulong channelId, string position, ChannelTeamType channelTeamType = ChannelTeamType.TeamOne)
        {
            var teamType = _channelService.GetTeamTypeForChannelTeamType(channelTeamType, channelId);
            var response = _matchupService.ClearPosition(channelId, position, teamType);

            return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);
        }

        public bool ReadyMatch(ulong channelId, int serverListItemId, out int readiedMatchupId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var matchup = _matchupService.GetCurrentMatchupForChannel(channelId);
            var server = GetServerFromServerListItemId(serverListItemId, channelId);
            var discordChannel = _discordClient.GetChannel(channelId) as ITextChannel;

            var serviceResponse = _matchupService.ReadyMatch(channelId, server.Id, out int matchId);
            readiedMatchupId = matchup.Id;

            if (serviceResponse.Status != ServiceResponseStatus.Success)
            {
                discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbedFromServiceResponse(serviceResponse));

                return false;
            }
            else
            {
                SendReadyMessageForTeam(matchup, matchup.LineupHome, server);
                if (!matchup.IsMixMatch)
                {
                    SendReadyMessageForTeam(matchup, matchup.LineupAway, server);
                }
                else
                {
                    SendReadyMessageForTeam(matchup, matchup.LineupAway, server, false);
                }
                SendReadyMessageForPlayers(matchup, matchup.SignedPlayers, server);

                if (matchup.IsMixMatch && !channel.ChannelPositions.Any(c => c.Position.Name.ToUpper() == "GK"))
                {
                    SendSuggestedLineups(matchup, discordChannel);
                }

                return true;
            }
        }

        public List<Embed> GenerateTeamList(ulong channelId)
        {
            return _matchupService.GenerateTeamList(channelId);
        }

        public async Task<Embed> Search(ulong channelId, string challengerMention)
        {
            var challenger = _channelService.GetChannelByDiscordId(channelId);
            var response = await _searchService.Search(challenger.Id, challengerMention);

            return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);
        }

        public async Task<Embed> StopSearch(ulong channelId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var response = await _searchService.StopSearch(channel.Id);

            return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);
        }

        public Embed Challenge(ulong challengerChannelId, string teamCode, string challengerMention)
        {
            var challenger = _channelService.GetChannelByDiscordId(challengerChannelId);
            var opposition = _channelService.GetChannelBySearchTeamCode(teamCode, (MatchFormat)challenger.ChannelPositions.Count);

            if (challenger.IsMixChannel) return DiscordEmbedHelper.GenerateEmbed($"Mix channels cannot challenge teams", ServiceResponseStatus.Failure);
            if (opposition == null) return DiscordEmbedHelper.GenerateEmbed($"**{teamCode}** is not a valid team code", ServiceResponseStatus.Failure);

            var response = _matchupService.Challenge(challengerChannelId, opposition.DiscordChannelId, challengerMention);

            if (response.Status != ServiceResponseStatus.Success) return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);

            var acceptMessage = $":handshake: {challenger.Team.DisplayName} have accepted the challenge! Contact {challengerMention} to arrange further.";
            (_discordClient.GetChannel(opposition.DiscordChannelId) as SocketTextChannel).SendMessageAsync("", embed: DiscordEmbedHelper.GenerateSimpleEmbed(acceptMessage));

            (_discordClient.GetChannel(challengerChannelId) as SocketTextChannel).SendMessageAsync("", embed: GenerateTeamList(challengerChannelId).First());
            (_discordClient.GetChannel(opposition.DiscordChannelId) as SocketTextChannel).SendMessageAsync("", embed: GenerateTeamList(opposition.DiscordChannelId).First());

            return DiscordEmbedHelper.GenerateEmbed($":handshake: You have successfully challenged {opposition.Team.BadgeEmote}{opposition.Team.Name}. `!ready` will send both teams to the server", ServiceResponseStatus.Success);
        }

        public Embed ListChallenges(ulong challengerChannelId)
        {
            var channel = _channelService.GetChannelByDiscordId(challengerChannelId, true);
            var searches = _searchService.GetSearches().Where(s => s.Channel.Team.RegionId == channel.Team.RegionId && s.ChannelId != channel.Id && channel.ChannelType == ChannelType.Team && channel.Format == s.Channel.Format);
            var mixChallenges = _channelService.GetChannels().Where(c => c.ChannelType == ChannelType.PublicMix && c.Team.RegionId == channel.Team.RegionId && channel.Format == c.Format);

            var embedBuilder = new EmbedBuilder();

            embedBuilder.WithTitle($":mag: Available Challenges");

            var teamList = new StringBuilder();
            foreach (var search in searches)
            {
                var match = _matchupService.GetCurrentMatchupForChannel(search.Channel.DiscordChannelId);
                if (!string.IsNullOrEmpty(search.Channel.Team.BadgeEmote)) teamList.Append($"{search.Channel.Team.BadgeEmote} ");
                teamList.Append($"**{search.Channel.SearchTeamCode}** {search.Channel.Team.Name} ");
                if (!match.LineupHome.HasGk) teamList.Append("(No GK)");
                teamList.AppendLine("");
                //if (_config.BotConfig.EnableBotHubIntegration) teamList.AppendLine(GenerateFormEmoteListForChannel(search.Channel.DiscordChannelId));
                var searchMinutesAgo = DateTime.UtcNow.Subtract(search.CreatedDate).TotalMinutes.ToString("0");
                if (searchMinutesAgo == "0" || searchMinutesAgo == "1")
                {
                    teamList.AppendLine($"*Search started just now*");
                }
                else
                {
                    teamList.AppendLine($"*Searching for {searchMinutesAgo} minutes*");
                }
                teamList.AppendLine($"");
            }
            embedBuilder.AddField("**Teams**", teamList.ToString().NullIfEmpty() ?? "*There are no teams currently searching*");

            var mixTeamList = new StringBuilder();
            foreach (var mix in mixChallenges)
            {
                var matchup = _matchupService.GetCurrentMatchupForChannel(mix.DiscordChannelId);
                if (matchup.IsMixMatch)
                {
                    if (!string.IsNullOrEmpty(matchup.LineupHome.Channel.Team.BadgeEmote)) mixTeamList.Append($"{matchup.LineupHome.Channel.Team.BadgeEmote} ");
                    mixTeamList.Append($"**{mix.SearchTeamCode}** {mix.Team.Name} ");
                    if (!matchup.LineupHome.HasGk) mixTeamList.Append(" (No GK)");
                    mixTeamList.AppendLine("");
                    if (matchup.SignedPlayers.Any())
                    {
                        mixTeamList.AppendLine("*" + string.Join(", ", matchup.SignedPlayers.Select(p => p.Name)) + "*");
                    }
                    else
                    {
                        mixTeamList.AppendLine($"*No players currently signed*");
                    }
                    //if (_config.BotConfig.EnableBotHubIntegration) mixTeamList.AppendLine(GenerateFormEmoteListForChannel(matchup.LineupHome.Channel.DiscordChannelId));
                    mixTeamList.AppendLine($"");
                }
            }
            embedBuilder.AddField("**Mixes**", mixTeamList.ToString().NullIfEmpty() ?? "*There are no mix teams currently available*");

            var footer = new EmbedFooterBuilder().WithText("To challenge a team type !challenge teamcode").WithIconUrl("https://www.iosoccer.com/info.png");

            return embedBuilder.WithDefaultColour().WithFooter(footer).Build();
        }

        public async Task<Embed> Unchallenge(ulong challengerChannelId, string challengerMention)
        {
            var matchup = _matchupService.GetCurrentMatchupForChannel(challengerChannelId);
            var homeChannel = matchup.LineupHome?.Channel;
            var awayChannel = matchup.LineupAway?.Channel;

            var response = _matchupService.Unchallenge(challengerChannelId);

            if (response.Status == ServiceResponseStatus.NegativeSuccess)
            {
                var unchallengeMessage = $"The game between **{homeChannel.Team.Name}** & **{awayChannel.Team.Name}** has been called off by **{challengerMention}**";
                var embed = DiscordEmbedHelper.GenerateSimpleEmbed(unchallengeMessage, ":thunder_cloud_rain: Match Abandoned!");
                await _discordNotificationService.SendChannelMessage(homeChannel.DiscordChannelId, embed: embed);
                await _discordNotificationService.SendChannelMessage(awayChannel.DiscordChannelId, embed: embed);
            }

            return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);
        }

        public Embed AddSub(ulong channelId, IUser user)
        {
            var player = _playerService.GetPlayer(user);
            var teamType = _channelService.GetTeamTypeForChannelTeamType(ChannelTeamType.TeamOne, channelId);

            var response = _matchupService.AddSubsitutePlayerToLineup(channelId, player, teamType);

            return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);
        }

        public Embed RemoveSub(ulong channelId, IUser user)
        {
            var player = _playerService.GetPlayer(user);

            var response = _matchupService.RemoveSubstitutePlayerFromMatch(channelId, player);

            return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);
        }

        public Embed RemoveSub(ulong channelId, string userName)
        {
            var player = _playerService.GetPlayer(userName);

            var response = _matchupService.RemoveSubstitutePlayerFromMatch(channelId, player);

            return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);
        }

        public async Task RequestSub(ulong channelId, int serverListItemId, string positionName, IUser user)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var server = _serverService.GetServersByRegion((int)channel.Team.RegionId).Where(t => t.DeactivatedDate == null).ToList()[serverListItemId - 1];

            await _substitutionService.CreateRequest(channel.Id, server.Id, positionName, user.Username);
        }

        public Embed AcceptSubRequest(string requestToken, IUser user)
        {
            var player = _playerService.GetPlayer(user);

            var response = _substitutionService.AcceptSubstitution(requestToken, player);

            if (response.Status == ServiceResponseStatus.Success)
            {
                var server = _substitutionService.GetServerForSubstitionRequest(requestToken);
                var message = $"{user.Username} is coming to the server as a substitute";
                _serverManagementServiceService.SendServerMessage(server.Id, message);
            }

            return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);
        }

        public Embed CancelSubRequest(string requestToken, IUser user)
        {
            var player = _playerService.GetPlayer(user);

            var response = _substitutionService.CancelSubstitution(requestToken, player);

            return DiscordEmbedHelper.GenerateEmbedFromServiceResponse(response);
        }

        public Embed GenerateRecentMatchList(ulong channelId)
        {
            var recentMatches = _matchupService.GetMatchupsForChannel(channelId, true, 10);
            var embedBuilder = new EmbedBuilder().WithTitle(":calendar_spiral: Recent Matches");
            if (recentMatches == null || !recentMatches.Any()) return new EmbedBuilder().WithDescription(":information_source: No matches have been played yet. Chill.").Build();
            foreach (var recentMatch in recentMatches)
            {
                var playerList = "";
                if (recentMatch.LineupHome.Channel.DiscordChannelId == channelId) playerList = string.Join(", ", recentMatch.LineupHome.PlayerLineupPositions.Select(ptp => ptp.Player.Name));
                if (recentMatch.LineupAway.Channel.DiscordChannelId == channelId) playerList = $"{(playerList == "" ? "" : ", ")} {string.Join(", ", recentMatch.LineupAway.PlayerLineupPositions.Select(ptp => ptp.Player.Name))}";
                if (playerList == "") playerList = "No player data available";

                string matchInfo;
                string matchDetail = null;

                if (_config.BotConfig.EnableBotHubIntegration && recentMatch.Match.MatchStatistics != null)
                {
                    var matchDetailBuilder = new StringBuilder().AppendLine(playerList).AppendLine($"https://{_config.WebServerConfig.ClientUrl}match-overview/{recentMatch.Match.Id}");
                    matchDetail = matchDetailBuilder.ToString();
                    matchInfo = $"**{recentMatch.LineupHome.Channel.Team.Name}** {recentMatch.LineupHome.Channel.Team.BadgeEmote} `{recentMatch.Match.MatchStatistics.MatchGoalsHome}` - `{recentMatch.Match.MatchStatistics.MatchGoalsAway}` {recentMatch.LineupAway.Channel.Team.BadgeEmote} **{recentMatch.LineupAway.Channel.Team.Name}** - `{recentMatch.ReadiedDate.ToString()}`";
                }
                else
                {
                    matchInfo = $"**{recentMatch.LineupHome.Channel.Team.DisplayName}** vs **{recentMatch.LineupAway.Channel.Team.DisplayName}** - {recentMatch.ReadiedDate.ToString()}";
                }

                embedBuilder.AddField(matchInfo, matchDetail ?? playerList);
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
            var servers = _serverService.GetServersByRegion((int)channel.Team.RegionId).Where(t => t.DeactivatedDate == null).ToList();

            return servers[serverListItemId - 1];
        }

        private async void SendReadyMessageForPlayers(Matchup matchup, List<Player> players, Server server)
        {
            var message = $":soccer: Match ready! **{matchup.LineupHome.Channel.Team.Name} {matchup.LineupHome.Channel.Team.BadgeEmote}** vs **{matchup.LineupAway.Channel.Team.BadgeEmote} {matchup.LineupAway.Channel.Team.Name}** - Please join **{server.Name}** (steam://connect/{server.Address}) as soon as possible.";
            foreach (var player in players.Where(p => p.DiscordUserId != null && !p.DisableDMNotifications))
            {
                await _discordNotificationService.SendUserMessage((ulong)player.DiscordUserId, message);
            }
        }

        private async void SendReadyMessageForTeam(Matchup matchup, Lineup team, Server server, bool sendChannelKickOffMessage = true)
        {
            var discordChannel = _discordClient.GetChannel(team.Channel.DiscordChannelId) as ITextChannel;

            if (sendChannelKickOffMessage)
            {
                var embed = DiscordEmbedHelper.GenerateSimpleEmbed($"Please join **{server.Name}** (steam://connect/{server.Address}) as soon as possible. If you need to use a different server, use `!setupserver <server id> {matchup.Id}`", $"**{matchup.LineupHome.Channel.Team.Name} {matchup.LineupHome.Channel.Team.BadgeEmote}** vs **{matchup.LineupAway.Channel.Team.DisplayName} {matchup.LineupAway.Channel.Team.Name}**");
                await discordChannel.SendMessageAsync("", embed: embed);
            }

            var highlightMessage = string.Join(", ", team.PlayerLineupPositions.Where(ptp => ptp.Player.DiscordUserId != null).Select(ptp => ptp.Player.DisplayName));
            if (!string.IsNullOrEmpty(highlightMessage)) await discordChannel.SendMessageAsync(highlightMessage);
        }

        private async void SendSuggestedLineups(Matchup matchup, ITextChannel discordChannel)
        {
            var homeTeam = new List<Player>();
            var awayTeam = new List<Player>();
            var orderedPlayers = matchup.SignedPlayers.OrderByDescending(p => p.Rating);

            if (orderedPlayers.Count() < 6) return;

            homeTeam.Add(orderedPlayers.ElementAt(0));
            awayTeam.Add(orderedPlayers.ElementAt(1));
            awayTeam.Add(orderedPlayers.ElementAt(2));
            homeTeam.Add(orderedPlayers.ElementAt(3));
            awayTeam.Add(orderedPlayers.ElementAt(4));

            var isHomeTeam = true;
            foreach (var player in orderedPlayers.Where(p => !homeTeam.Any(t => t.Id == p.Id) && !awayTeam.Any(t => t.Id == p.Id)))
            {
                if (isHomeTeam)
                {
                    homeTeam.Add(player);
                }
                else
                {
                    awayTeam.Add(player);
                }
                isHomeTeam = !isHomeTeam;
            }

            var embed = DiscordEmbedHelper.GenerateSimpleEmbed($"**Mix #1:** {string.Join(", ", homeTeam.Select(p => p.Name))} **Mix #2:** {string.Join(", ", awayTeam.Select(p => p.Name))}", $"**Suggested Teams**");
            await discordChannel.SendMessageAsync("", embed: embed);
        }

        private string GenerateFormEmoteListForChannel(ulong channelId)
        {
            const string formWinEmote = "<:form_win_sm:676578014188011520>";
            const string formLossEmote = "<:form_loss_sm:676578012438855692>";
            const string formDrawEmote = "<:form_draw_sm:676578012380135434>";
            const string formUnknownEmote = "<:form_unknown_sm:676936524998377492>";

            var formList = _matchupService.GetFormForChannel(channelId, 5);
            var sb = new StringBuilder();

            foreach (var formItem in formList)
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
                for (var unknownItems = 5 - formList.Count(); unknownItems > 0; unknownItems--)
                {
                    sb.Append($"{formUnknownEmote} ");
                }
            }

            return sb.ToString();
        }
    }
}