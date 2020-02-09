using CoachBot.Database;
using CoachBot.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoachBot.Domain.Extensions
{
    public static class MatchQueryExtensions
    {
        public static Match GetCurrentMatchForChannel(this CoachBotContext coachBotContext, ulong channelId)
        {
            return coachBotContext.Matches
                .Include(m => m.TeamHome)
                    .ThenInclude(th => th.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.TeamHome)
                    .ThenInclude(th => th.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Position)
                .Include(m => m.TeamHome)
                   .ThenInclude(th => th.PlayerTeamPositions)
                   .ThenInclude(ptp => ptp.Team)
                .Include(m => m.TeamHome)
                    .ThenInclude(th => th.PlayerSubstitutes)
                    .ThenInclude(ps => ps.Player)
                .Include(m => m.TeamHome)
                    .ThenInclude(th => th.Channel)
                    .ThenInclude(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
                .Include(m => m.TeamAway)
                    .ThenInclude(ta => ta.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.TeamAway)
                    .ThenInclude(ta => ta.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Position)
                .Include(m => m.TeamAway)
                   .ThenInclude(th => th.PlayerTeamPositions)
                   .ThenInclude(ptp => ptp.Team)
                .Include(m => m.TeamAway)
                    .ThenInclude(ta => ta.PlayerSubstitutes)
                    .ThenInclude(ps => ps.Player)
                .Include(m => m.TeamAway)
                    .ThenInclude(ta => ta.Channel)
                    .ThenInclude(c => c.ChannelPositions)
                    .ThenInclude(cp => cp.Position)
                .Where(m => m.ReadiedDate == null)
                .OrderByDescending(m => m.CreatedDate)
                .First(m => m.TeamHome.Channel.DiscordChannelId == channelId || (m.TeamAway != null && m.TeamAway.Channel.DiscordChannelId == channelId));
        }

        public static Match GetMatchById(this CoachBotContext coachBotContext, int matchId)
        {
            return coachBotContext.Matches
                .Include(m => m.TeamHome)
                    .ThenInclude(th => th.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.TeamHome)
                    .ThenInclude(th => th.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Position)
                .Include(m => m.TeamHome)
                   .ThenInclude(th => th.PlayerTeamPositions)
                   .ThenInclude(ptp => ptp.Team)
                .Include(m => m.TeamHome)
                    .ThenInclude(ta => ta.PlayerSubstitutes)
                    .ThenInclude(ps => ps.Player)
                .Include(m => m.TeamAway)
                    .ThenInclude(ta => ta.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.TeamAway)
                    .ThenInclude(ta => ta.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Position)
                .Include(m => m.TeamAway)
                   .ThenInclude(th => th.PlayerTeamPositions)
                   .ThenInclude(ptp => ptp.Team)
                .Include(m => m.TeamAway)
                    .ThenInclude(ta => ta.PlayerSubstitutes)
                    .ThenInclude(ps => ps.Player)
                .Include(m => m.Server)
                .Single(m => m.Id == matchId);
        }
    }
}
