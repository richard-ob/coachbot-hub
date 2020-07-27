using System;
using System.Linq;
using CoachBot.Domain.Model;
using CoachBot.Model;

namespace CoachBot.Database
{
    public static class CoachBotContextExtensions
    {
        public static void Initialize(this CoachBotContext context)
        {
            try
            {
                context.Searches.RemoveRange(context.Searches.Where(s => s.CreatedDate < DateTime.UtcNow.AddHours(-2))); // Clear all searches when restarted as timers will have stopped
                // context.Lineups.RemoveRange(context.Lineups.Where(t => !context.Matchups.Any(m => m.LineupHomeId == t.Id || m.LineupAwayId == t.Id))); // Remove orphaned teams
                //context.PlayerTeamPositions.RemoveRange(context.PlayerTeamPositions.Where(ptp => ptp.CreatedDate < DateTime.Now.AddDays(-1)).Where(ptp => ptp.Team == null || ptp.Team.Match == null || ptp.Team.Match.ReadiedDate == null)); // Clear any signings older than a day

                var player = new Player()
                {
                    Name = "Thing'e'",
                    SteamID = 76561197960374238,
                    DiscordUserId = 166153339610857472,
                    HubRole = PlayerHubRole.Owner,
                    Rating = 7.2
                };
                context.Players.Add(player);
                context.SaveChanges();
            }
            catch
            {
            }
            context.Database.EnsureCreated();
        }
    }
}