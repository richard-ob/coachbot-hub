using CoachBot.Database;
using CoachBot.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CoachBot.Domain.Extensions
{
    public static class MatchQueryExtensions
    {
        public static Match GetCurrentMatchForChannel(this CoachBotContext coachBotContext, ulong channelId)
        {
            return coachBotContext.Matches
                .Include(m => m.LineupHome)
                    .ThenInclude(th => th.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.LineupHome)
                    .ThenInclude(th => th.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Position)
                .Include(m => m.LineupHome)
                   .ThenInclude(th => th.PlayerLineupPositions)
                   .ThenInclude(ptp => ptp.Lineup)
                .Include(m => m.LineupHome)
                    .ThenInclude(th => th.PlayerSubstitutes)
                    .ThenInclude(ps => ps.Player)
                .Include(m => m.LineupHome)
                    .ThenInclude(th => th.Channel)
                    .ThenInclude(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
                .Include(m => m.LineupHome)
                    .ThenInclude(th => th.Channel)
                    .ThenInclude(c => c.Team)
                    .ThenInclude(t => t.Guild)
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Position)
                .Include(m => m.LineupAway)
                   .ThenInclude(th => th.PlayerLineupPositions)
                   .ThenInclude(ptp => ptp.Lineup)
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.PlayerSubstitutes)
                    .ThenInclude(ps => ps.Player)
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.Channel)
                    .ThenInclude(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.Channel)
                    .ThenInclude(c => c.Team)
                    .ThenInclude(t => t.Guild)
                .Where(m => m.ReadiedDate == null)
                .OrderByDescending(m => m.CreatedDate)
                .First(m => m.LineupHome.Channel.DiscordChannelId == channelId || (m.LineupAway != null && m.LineupAway.Channel.DiscordChannelId == channelId));
        }

        public static Match GetMatchById(this CoachBotContext coachBotContext, int matchId)
        {
            return coachBotContext.Matches
                .Include(m => m.LineupHome)
                    .ThenInclude(th => th.Channel)
                    .ThenInclude(c => c.Team)
                    .ThenInclude(t => t.BadgeImage)
                .Include(m => m.LineupHome)
                    .ThenInclude(th => th.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.LineupHome)
                    .ThenInclude(th => th.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Position)
                .Include(m => m.LineupHome)
                   .ThenInclude(th => th.PlayerLineupPositions)
                   .ThenInclude(ptp => ptp.Lineup)
                .Include(m => m.LineupHome)
                    .ThenInclude(ta => ta.PlayerSubstitutes)
                    .ThenInclude(ps => ps.Player)
                .Include(m => m.LineupAway)
                    .ThenInclude(th => th.Channel)
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Position)
                .Include(m => m.LineupAway)
                   .ThenInclude(th => th.PlayerLineupPositions)
                   .ThenInclude(ptp => ptp.Lineup)
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.PlayerSubstitutes)
                    .ThenInclude(ps => ps.Player)
                .Include(m => m.MatchStatistics)
                .Include(m => m.Server)
                    .ThenInclude(s => s.Region)
                .Single(m => m.Id == matchId);
        }
    }
}