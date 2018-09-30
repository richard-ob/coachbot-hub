using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class LeaderboardController : Controller
    {
        private readonly StatisticsService _statisticsService;
        private readonly LeaderboardService _leaderboardService;
        private readonly DiscordSocketClient _client;

        public LeaderboardController(StatisticsService statisticsService, LeaderboardService leaderboardService, DiscordSocketClient client)
        {
            _statisticsService = statisticsService;
            _leaderboardService = leaderboardService;
            _client = client;
        }

        [HttpGet("channels")]
        public IList<KeyValuePair<string, int>> GetLeaderboardForChannels()
        {
            var matches = _statisticsService.MatchHistory.GroupBy(x => x.ChannelId).Select(g => new KeyValuePair<ulong, int>(g.Key, g.Count())).OrderBy(o => o.Value);
            var matchesWithChannelName = new List<KeyValuePair<string, int>>();
            foreach (var match in matches)
            {
                var channel = (SocketGuildChannel)_client.GetChannel(match.Key);
                var channelName = channel != null ? $"#{channel.Name} ({channel.Guild.Name})" : "Unknown";
                var matchWithChannelName = new KeyValuePair<string, int>(channelName, match.Value);
                matchesWithChannelName.Add(matchWithChannelName);
            }
            return matchesWithChannelName;
        }

        [HttpGet("channel/{id}")]
        public IEnumerable<KeyValuePair<string, int>> GetPlayerLeaderboardForChannel(ulong id)
        {
            var recentMatches = _statisticsService.MatchHistory.Where(m => m.ChannelId == id);
            var appearances = new List<Player>();
            foreach (var match in recentMatches)
            {
                appearances.AddRange(match.Players.Where(p => p.DiscordUserId > 0 && p.DiscordUserId != null));
            }
            var leaderboard = new List<KeyValuePair<string, int>>();
            foreach (var player in appearances.Select(s => s.DiscordUserId).Distinct())
            {
                var playerAppearances = appearances.Where(a => a.DiscordUserId == player).Count();
                leaderboard.Add(new KeyValuePair<string, int>(appearances.Last(a => a.DiscordUserId == player).Name, playerAppearances));
            }
            return leaderboard.OrderByDescending(p => p.Value);
        }

        [HttpGet("players")]
        public IEnumerable<KeyValuePair<string, int>> GetLeaderboardForPlayers()
        {
            return _leaderboardService.GetLeaderboardForPlayers();
        }

        [HttpGet("player/{id}")]
        public IList<KeyValuePair<string, int>> GetLeaderboardForPlayer(ulong id)
        {
            var matches = _statisticsService.MatchHistory.Where(p => p.Players.Any(a => a.DiscordUserId == id)).GroupBy(x => x.ChannelId).Select(g => new KeyValuePair<ulong, int>(g.Key, g.Count())).OrderBy(o => o.Value);
            var matchesWithChannelName = new List<KeyValuePair<string, int>>();
            foreach (var match in matches)
            {
                var channel = (SocketGuildChannel)_client.GetChannel(match.Key);
                if (channel != null)
                {
                    matchesWithChannelName.Add(new KeyValuePair<string, int>(channel.Name, match.Value));
                }
            }
            return matchesWithChannelName.OrderByDescending(x => x.Value).ToList();
        }
    }
}
