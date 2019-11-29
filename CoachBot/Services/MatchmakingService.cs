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
            var response = _matchService.Create(channel.Id);

            ResetLastMentionTime(channelId);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed AddPlayer(ulong channelId, IUser user, string positionName = null, TeamType teamType = TeamType.Home)
        {
            var player = _playerService.GetPlayer(user);
            var response = _matchService.AddPlayerToTeam(channelId, player, positionName, teamType);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed AddPlayer(ulong channelId, string userName, string positionName = null, TeamType teamType = TeamType.Home)
        {
            var player = _playerService.GetPlayer(userName);

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

        public Embed ClearPosition(ulong channelId, string position, TeamType teamType = TeamType.Home)
        {
            var response = _matchService.ClearPosition(channelId, position, teamType);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed RemoveOpposition(ulong channelId)
        {
            var response = _matchService.RemoveTeamFromMatch(channelId);

            ResetLastMentionTime(channelId);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed AddOpposition(ulong channelId, string oppositionName)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var team = new Team()
            {
                TeamType = TeamType.Away
            };
            var response = _matchService.AddTeamToMatch(channelId, team);

            ResetLastMentionTime(channelId);

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
                var sb = new StringBuilder();

                sb.Append($":checkered_flag: Match Ready! {Environment.NewLine} Join {server.Name} steam://connect/{server.Address} ");
                foreach (var playerTeamPosition in match.TeamHome.PlayerTeamPositions)
                {
                    sb.Append($"{playerTeamPosition.Player.DiscordUserMention ?? playerTeamPosition.Player.Name} ");
                    if (playerTeamPosition.Player.DiscordUserId != null)
                    {
                        var message = $":stadium: Match ready! {match.TeamHome.Channel.Name} vs {match.TeamAway.Channel.Name} - Please join {server.Name} (steam://connect/{server.Address}) as soon as possible.";
                        _discordNotificationService.SendUserMessage((ulong)playerTeamPosition.Player.DiscordUserId, message);
                    }
                }
                sb.AppendLine();

                discordChannel.SendMessageAsync(sb.ToString());

                return true;
            }
        }

        public List<Embed> GenerateTeamList(ulong channelId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var match = _matchService.GetCurrentMatchForChannel(channelId);

            if (!channel.UseClassicLineup) return GenerateTeamSheet(channelId);

            var teamLists = new List<Embed>() {
                new TeamListEmbedFactory(channel, match, TeamType.Home).GenerateEmbed()
            };

            if (match.IsMixMatch)
            {
                var awayTeamList = new TeamListEmbedFactory(channel, match, TeamType.Away).GenerateEmbed();
                teamLists.Add(awayTeamList);
            }

            return teamLists;
        }

        private List<Embed> GenerateTeamSheet(ulong channelId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var match = _matchService.GetCurrentMatchForChannel(channelId);

            if (channel.UseClassicLineup) return GenerateTeamList(channelId);

            var teamSheets = new List<Embed>() {
                new TeamSheetEmbedFactory(channel, match, TeamType.Home).GenerateEmbed()
            };

            if (match.IsMixMatch)
            {
                var awayTeamSheet = new TeamSheetEmbedFactory(channel, match, TeamType.Away).GenerateEmbed();
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
                .WithDescription($"To challenge {challenger.Name} type **!challenge {challenger.Id}** and contact {challengerMention} for more information")
                .WithCurrentTimestamp();

            if (challenger.Color != null && challenger.Color[0] == '#')
            {
                embed.WithColor(new Color(ColorExtensions.FromHex(challenger.Color).R, ColorExtensions.FromHex(challenger.Color).G, ColorExtensions.FromHex(challenger.Color).B));
            }
            else
            {
                embed.WithColor(new Color(0xFFFFFF));
            }

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed StopSearch(ulong channelId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var response = _searchService.StopSearch(channel.Id);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public string Challenge(ulong challengerChannelId, string teamCode, string challengerMention)
        {
            var challenger = _channelService.GetChannelByDiscordId(challengerChannelId);
            var opposition = _channelService.GetChannelByTeamCode(teamCode);

            if (opposition == null) return $":no_entry: {teamCode} is not a valid team code";

            var response = _matchService.Challenge(challengerChannelId, opposition.DiscordChannelId, challengerMention);

            if (response.Status != Domain.Model.ServiceResponseStatus.Success) return $":no_entry: {response.Message}";

            var acceptMsg = $":handshake: {challenger.Name} have accepted the challenge! Contact {challengerMention} to arrange further.";
            (_discordClient.GetChannel(opposition.DiscordChannelId) as SocketTextChannel).SendMessageAsync("", embed: new EmbedBuilder().WithDescription(acceptMsg).WithCurrentTimestamp().Build());
            (_discordClient.GetChannel(challengerChannelId) as SocketTextChannel).SendMessageAsync("", embed: GenerateTeamList(challengerChannelId).First());
            (_discordClient.GetChannel(opposition.DiscordChannelId) as SocketTextChannel).SendMessageAsync("", embed: GenerateTeamList(opposition.DiscordChannelId).First());            

            return $":handshake: You have successfully challenged {opposition.Name}. !ready will send both teams to the server";
        }

        public Embed ListChallenges(ulong challengerChannelId)
        {
            var channel = _channelService.GetChannelByDiscordId(challengerChannelId);
            var searches = _searchService.GetSearches().Where(s => s.Channel.RegionId == channel.RegionId && s.ChannelId != channel.Id);
            var mixChallenges = _channelService.GetChannels().Where(c => c.IsMixChannel && c.RegionId == channel.RegionId);

            var embedBuilder = new EmbedBuilder();

            var regionName = channel.Region.RegionName + (channel.Region.RegionName[channel.Region.RegionName.Length - 1] == 'e' ? "an" : "n");
            embedBuilder.WithTitle($":flag_eu: Available {regionName} Challenges ");
            embedBuilder.WithDescription("To accept the challenge of any teams below, use the `!challenge <team code>` command, e.g. `!challenge BB`");

            foreach(var search in searches)
            {
                embedBuilder.AddField($"{search.Channel.BadgeEmote ?? ":busts_in_silhouette:"} {search.Channel.Name} [{search.Channel.TeamCode}]", $"*Searching since {search.CreatedDate}*");
            }

            foreach(var mix in mixChallenges)
            {
                var match = _matchService.GetCurrentMatchForChannel(mix.DiscordChannelId);
                embedBuilder.AddField($"{mix.BadgeEmote ?? ":busts_in_silhouette:"} {mix.Name} [{mix.TeamCode}]", $"*Always searching - {match.SignedPlayersAndSubs.Count} players currently signed*");
            }

            if (!searches.Any() && !mixChallenges.Any()) embedBuilder.AddField(":zzz: There are no teams currently available to challenge.", "*To start a search for your team, type `!search`*");

            return embedBuilder.Build();
        }

        public Embed Unchallenge(ulong challengerChannelId, string challengerMention)
        {
            var response = _matchService.Unchallenge(challengerChannelId);

            return EmbedTools.GenerateEmbedFromServiceResponse(response);
        }

        public Embed AddSub(ulong channelId, IUser user)
        {
            var player = _playerService.GetPlayer(user);

            var response = _matchService.AddSubsitutePlayerToTeam(channelId, player);

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
                var sb = new StringBuilder();
                foreach (var player in recentMatch.SignedPlayers)
                {
                    if (recentMatch.SignedPlayers.Last() != player)
                    {
                        sb.Append($"{player.Name}, ");
                    }
                    else
                    {
                        sb.Append($"{player.Name}");
                    }

                }
                embedBuilder.AddField($"**{recentMatch.TeamHome.Channel.TeamCode}** vs **{recentMatch.TeamAway.Channel.TeamCode}** - {recentMatch.ReadiedDate.ToString()}", sb.ToString());
            }

            return embedBuilder.Build();
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
    }
}
