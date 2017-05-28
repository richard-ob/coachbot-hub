using System.Collections.Generic;
using CoachBot.Model;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System;
using System.Linq;
using Discord;

namespace CoachBot.Services.Matchmaker
{
    public class StatisticsService
    {
        public List<Match> MatchHistory = new List<Match>();

        public StatisticsService()
        {
            if (File.Exists(@"history.json"))
            {
                MatchHistory = JsonConvert.DeserializeObject<List<Match>>(File.ReadAllText(@"history.json"));
            }
            else
            {
                File.WriteAllText(@"history.json", JsonConvert.SerializeObject(MatchHistory));
            }
        }

        private void Save()
        {
            File.WriteAllText(@"history.json", JsonConvert.SerializeObject(MatchHistory));
        }

        public void AddMatch(Channel channel)
        {
            var match = new Match()
            {
                ChannelId = channel.Id,
                Team1Name = channel.Team1.Name,
                Team2Name = channel.Team2.Name ?? "Mix",
                MatchDate = DateTime.Now,
                Players = channel.SignedPlayers.ToList()
            };
            MatchHistory.Add(match);
            Save();
        }

        public Embed RecentMatches(ulong channelId)
        {
            var recentMatches = MatchHistory.Where(m => m.ChannelId == channelId).OrderByDescending(d => d.MatchDate).Take(10);
            var embedBuilder = new EmbedBuilder().WithTitle(":calendar_spiral: Recent Matches");
            if (recentMatches == null || !recentMatches.Any()) return new EmbedBuilder().WithDescription(":information_source: No matches have been played yet. Chill.").Build();
            foreach (var recentMatch in recentMatches)
            {
                var sb = new StringBuilder();
                foreach (var player in recentMatch.Players)
                {
                    if (recentMatch.Players.Last() != player)
                    {
                        sb.Append($"{player.Name}, ");
                    }
                    else
                    {
                        sb.Append($"{player.Name}");
                    }
                    
                }
                embedBuilder.AddField($"**{recentMatch.Team1Name}** vs **{recentMatch.Team2Name}** - {recentMatch.MatchDate.ToString()}", sb.ToString());
            }
            return embedBuilder.Build();
        }

        public string AppearanceLeaderboard(ulong channelId)
        {
            var recentMatches = MatchHistory.Where(m => m.ChannelId == channelId);
            if (recentMatches == null || !recentMatches.Any()) return ":information_source: No matches have been played yet. Chill.";
            var appearances = new List<Player>();
            foreach (var match in recentMatches)
            {
                appearances.AddRange(match.Players.Where(p => p.DiscordUserId > 0 && p.DiscordUserId != null));
            }
            var leaderboard = new List<Tuple<Player, int>>();
            foreach(var player in appearances)
            {
                var playerAppearances = appearances.Where(a => a.DiscordUserId == player.DiscordUserId).Count();
                if (!leaderboard.Any(p => p.Item1.DiscordUserId == player.DiscordUserId))
                {
                    leaderboard.Add(new Tuple<Player, int>(player, playerAppearances));
                }
            }
            var top10 = leaderboard.OrderByDescending(p => p.Item2).Take(10);
            var sb = new StringBuilder();
            sb.AppendLine(":trophy: **Leaderboard**");
            var rank = 1;
            foreach(var player in top10)
            {
                sb.AppendLine($"**#{rank}** {player.Item1.Name} - *{player.Item2} appearances*");
                rank++;
            }
            return sb.ToString();
        }
    }
}
