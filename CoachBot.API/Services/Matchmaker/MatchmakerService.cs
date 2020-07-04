using Discord;
using System.Collections.Generic;
using CoachBot.Model;
using System;
using Discord.WebSocket;
using System.Threading.Tasks;
using CoachBot.Domain.Services;
using CoachBot.Domain.Repositories;
using CoachBot.Domain.Model;

namespace CoachBot.Services.Matchmaker
{
    public class MatchmakerService
    {
        private readonly ConfigService _configService;
        private readonly StatisticsService _statisticsService;
        private readonly ServerService _serverService;
        private readonly ChannelRepository _channelRepository;
        private readonly DiscordSocketClient _client;

        public MatchmakerService(ConfigService configService, StatisticsService statisticsService, DiscordSocketClient client, ServerService serverService, ChannelRepository channelRepository)
        {
            _configService = configService;
            _statisticsService = statisticsService;
            _serverService = serverService;
            _channelRepository = channelRepository;
            _client = client;
        }

        public string AddPlayer(ulong channelId, IUser user, string position = null, TeamType teamType = TeamType.Home)
        {
            return "";
        }

        public string AddPlayer(ulong channelId, string playerName, string position, TeamType teamType = TeamType.Home)
        {
            return "";
        }

        public string RemovePlayer(ulong channelId, IUser user)
        {
            return "";
        }

        public string RemovePlayer(ulong channelId, string playerName)
        {
            return "";
        }

        public string AddSub(ulong channelId, IUser user)
        {
            return "";
        }

        public string RemoveSub(ulong channelId, IUser user)
        {
            return "";
        }

        public string RemoveSub(ulong channelId, string playerName)
        {
            return "";
        }

        public string ChangeOpposition(ulong channelId, Team team)
        {
            return "";
        }

        public void ResetMatch(ulong channelId)
        {
           
        }

        public async Task ReadyMatchAsync(ulong channelId, int? serverId = null, bool ignorePlayerCounts = false)
        {
            throw new NotImplementedException();
        }

        public async Task UnsignPlayers(Channel channel)
        {
           
        }

        public string UnreadyMatch(ulong channelId)
        {
            return ":no entry: This functionality has not yet been implemented";
        }

        public async Task SingleKeeperAsync(ulong channelId, int serverId, bool enable)
        {
           
        }

        public string Search(ulong channelId, string challengerMention)
        {
          
            return ":white_check_mark: Searching for opposition.. To cancel, type **!stopsearch**";
        }

        public async Task TimeoutSearch(ulong channelId, List<IMessage> searchMessages)
        {
           
        }

        public string StopSearch(ulong channelId)
        {
           
            return ":negative_squared_cross_mark: Cancelled search for opposition";
        }

        public string Challenge(ulong challengerChannelId, ulong oppositionId, string challengerMention)
        {
         
            return "";
        }

        public string Unchallenge(ulong challengerChannelId, string unchallenger)
        {
           
            return "";
        }

        public string MentionHere(ulong channelId)
        {
            return "";
        }

        public Embed GenerateTeamList(ulong channelId, TeamType teamType = TeamType.Home)
        {
            return new EmbedBuilder().Build();
        }

        public Embed GenerateTeamListVintage(ulong channelId, TeamType teamType = TeamType.Home)
        {
            return new EmbedBuilder().Build();
        }
    }
}
