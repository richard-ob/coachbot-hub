using CoachBot.Domain.Model;
using CoachBot.Domain.Models.Dto;
using CoachBot.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CoachBot.Database
{
    public static class CoachBotContextExtensions
    {
        public static void Initialize(this CoachBotContext context)
        {
            try
            {
                context.Searches.RemoveRange(context.Searches); // Clear all searches when restarted as timers will have stopped
                context.Teams.RemoveRange(context.Teams.Where(t => !context.Matches.Any(m => m.TeamHomeId == t.Id || m.TeamAwayId == t.Id))); // Remove orphaned teams
                context.TeamStatisticTotals.RemoveRange(context.TeamStatisticTotals);
                context.PlayerStatisticTotals.RemoveRange(context.PlayerStatisticTotals);
                context.StatisticTotals.RemoveRange(context.StatisticTotals);
                //context.PlayerTeamPositions.RemoveRange(context.PlayerTeamPositions.Where(ptp => ptp.CreatedDate < DateTime.Now.AddDays(-1)).Where(ptp => ptp.Team == null || ptp.Team.Match == null || ptp.Team.Match.ReadiedDate == null)); // Clear any signings older than a day
                context.SaveChanges();
            }
            catch
            {

            }
            context.Database.EnsureCreated();
        }

        public static void SeedMatchData(this CoachBotContext context)
        {
            context.Matches.RemoveRange(context.Matches);
            context.MatchStatistics.RemoveRange(context.MatchStatistics);
            context.SaveChanges();
            string[] files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "2020*.json", SearchOption.AllDirectories);
            foreach(var file in files)
            {
                using (StreamReader r = new StreamReader(file))
                {
                    string json = r.ReadToEnd();
                    MatchStatisticsDto matchStatisticsData = JsonConvert.DeserializeObject<MatchStatisticsDto>(json);
                    var match = new Match()
                    {
                        MatchStatistics = new MatchStatistics()
                        {
                            MatchData = matchStatisticsData.MatchData
                        },
                        ReadiedDate = DateTime.UnixEpoch.AddSeconds(matchStatisticsData.MatchData.MatchInfo.StartTime),
                        ServerId = context.Servers.First().Id,
                        TeamHome = new Team()
                        {
                            ChannelId = 1,
                            TeamType = TeamType.Home
                        },
                        TeamAway = new Team()
                        {
                            ChannelId = 2,
                            TeamType = TeamType.Away
                            
                        }
                    };

                    context.Matches.Add(match);
                }
            }
            context.SaveChanges();
        }
    }
}
