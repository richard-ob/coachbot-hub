using System;
using System.Linq;

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
                //context.PlayerTeamPositions.RemoveRange(context.PlayerTeamPositions.Where(ptp => ptp.CreatedDate < DateTime.Now.AddDays(-1)).Where(ptp => ptp.Team == null || ptp.Team.Match == null || ptp.Team.Match.ReadiedDate == null)); // Clear any signings older than a day
                context.SaveChanges();
            }
            catch
            {

            }
            context.Database.EnsureCreated();
        }
    }
}
