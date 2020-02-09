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
                SendReadyMessageForTeam(match, match.TeamAway, server);

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

        public Embed Search(ulong channelId, string challengerMention)
        {
            var challenger = _channelService.GetChannelByDiscordId(channelId);
            var response = _searchService.Search(challenger.Id);

            var embed = new EmbedBuilder()
                .WithTitle($":mag: {challenger.BadgeEmote ?? challenger.Name} are searching for a team to face")
                .WithDescription($"To challenge **{challenger.Name}** type **!challenge {challenger.TeamCode}** and contact {challengerMention} for more information")
                .WithCurrentTimestamp()
                .WithColor(challenger.SystemColor);

            var regionChannels = _channelService.GetChannelsByRegion((int)challenger.RegionId)
                .Where(c => !c.DisableSearchNotifications && c.Id != challenger.Id)
                .Select(c => c.DiscordChannelId).ToList();
            _discordNotificationService.SendChannelMessage(regionChannels, embed);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed StopSearch(ulong channelId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var response = _searchService.StopSearch(channel.Id);

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

            var regionName = channel.Region.RegionName + (channel.Region.RegionName[channel.Region.RegionName.Length - 1] == 'e' ? "an" : "n");
            embedBuilder.WithTitle($":flag_eu: Available {regionName} Challenges ");
            if (searches.Any())
            {
                var teamList = new StringBuilder();
                foreach (var search in searches)
                {
                    teamList.AppendLine($"• { search.Channel.BadgeEmote ?? search.Channel.Name} [{search.Channel.TeamCode}] - *searching since {search.CreatedDate}*");
                }
                embedBuilder.AddField("**Teams**", teamList.ToString());
            }
            else
            {
                embedBuilder.AddField("**Teams**", "*There are no teams currently searching*");
            }

            if (mixChallenges.Any())
            {
                var mixTeamList = new StringBuilder();
                foreach (var mix in mixChallenges)
                {
                    var match = _matchService.GetCurrentMatchForChannel(mix.DiscordChannelId);
                    mixTeamList.AppendLine($"• {mix.BadgeEmote} {mix.Name} [{mix.TeamCode}] - *{match.SignedPlayersAndSubs.Count} players currently signed*");
                }
                embedBuilder.AddField("**Mixes**", mixTeamList.ToString());
            }
            else
            {
                embedBuilder.AddField("**Mixes**", "*There are no mix teams currently searching*");
            }

            embedBuilder.WithFooter(new EmbedFooterBuilder().WithText("To accept the challenge of any teams below, use the !challenge <team code> command, e.g. !challenge BB"));

            return embedBuilder.WithDefaultColour().Build();
        }

        public Embed Unchallenge(ulong challengerChannelId, string challengerMention)
        {
            var match = _matchService.GetCurrentMatchForChannel(challengerChannelId);
            var homeChannel = match.TeamHome?.Channel;
            var awayChannel = match.TeamAway?.Channel;

            var response = _matchService.Unchallenge(challengerChannelId);

            if (response.Status == ServiceResponseStatus.NegativeSuccess)
            {
                var unchallengeMessage = $"The game between **{homeChannel.Name}** & **{awayChannel.Name}** has been called off by **{challengerMention}**";
                var embed = EmbedTools.GenerateSimpleEmbed(unchallengeMessage, ":thunder_cloud_rain: Match Abandoned!");
                _discordNotificationService.SendChannelMessage(homeChannel.DiscordChannelId, embed: embed);
                _discordNotificationService.SendChannelMessage(awayChannel.DiscordChannelId, embed: embed);
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

        private void SendReadyMessageForTeam(Match match, Team team, Server server)
        {
            var sb = new StringBuilder();

            sb.Append($":checkered_flag: Match Ready! {Environment.NewLine} Join {server.Name} steam://connect/{server.Address} ");

            foreach (var playerTeamPosition in team.PlayerTeamPositions)
            {
                sb.Append($"{playerTeamPosition.Player.DiscordUserMention ?? playerTeamPosition.Player.Name} ");
                if (playerTeamPosition.Player.DiscordUserId != null)
                {
                    var message = $":stadium: Match ready! {match.TeamHome.Channel.Name} vs {match.TeamAway.Channel.Name} - Please join {server.Name} (steam://connect/{server.Address}) as soon as possible.";
                    _discordNotificationService.SendUserMessage((ulong)playerTeamPosition.Player.DiscordUserId, message);
                }
            }

            var discordChannel = _discordClient.GetChannel(team.Channel.DiscordChannelId) as ITextChannel;
            discordChannel.SendMessageAsync(sb.ToString());
        }
    }
}
