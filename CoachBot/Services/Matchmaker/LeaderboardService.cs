using CoachBot.Model;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Services.Matchmaker
{
    public class LeaderboardService
    {
        private DiscordSocketClient _client;
        private StatisticsService _statisticsService;

        public LeaderboardService(DiscordSocketClient client, StatisticsService statisticsService)
        {
            _client = client;
            _statisticsService = statisticsService;
        }     

        public IEnumerable<KeyValuePair<string, int>> GetLeaderboardForPlayers()
        {
            var recentMatches = _statisticsService.MatchHistory;
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
    }
}
