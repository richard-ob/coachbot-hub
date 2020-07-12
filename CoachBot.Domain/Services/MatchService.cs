using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class MatchService
    {
        private readonly CoachBotContext _coachBotContext;
        private readonly ChannelService _channelService;
        private readonly ServerService _serverService;
        private readonly SearchService _searchService;
        private readonly DiscordNotificationService _discordNotificationService;

        public MatchService(CoachBotContext coachBotContext, ChannelService channelService, ServerService serverService, SearchService searchService, DiscordNotificationService discordNotificationService)
        {
            _coachBotContext = coachBotContext;
            _channelService = channelService;
            _serverService = serverService;
            _searchService = searchService;
            _discordNotificationService = discordNotificationService;
        }

        public Match GetMatch(int matchId)
        {
            return _coachBotContext.Matches
                .Include(m => m.TeamHome)
                    .ThenInclude(t => t.BadgeImage)
                .Include(m => m.TeamAway)
                    .ThenInclude(t => t.BadgeImage)
                .Include(m => m.MatchStatistics)
                .Include(m => m.Tournament)
                .Include(s => s.Server)
                    .ThenInclude(s => s.Country)
                .Single(m => m.Id == matchId);
        }

        public PagedResult<Match> GetMatches(int page, int pageSize, string sortOrder, MatchFilters filters)
        {
            var queryable = _coachBotContext.Matches
                .Include(m => m.TeamHome)
                    .ThenInclude(t => t.BadgeImage)
                .Include(m => m.TeamAway)
                    .ThenInclude(t => t.BadgeImage)
                .Include(m => m.MatchStatistics)
                .Include(m => m.Tournament)
                .Include(s => s.Server)
                    .ThenInclude(s => s.Country)
                .Where(m => filters.RegionId == null || m.Server.RegionId == filters.RegionId || filters.TournamentId != null)
                .Where(m => filters.IncludeUpcoming || m.KickOff != null)
                .Where(m => filters.IncludePast || m.KickOff > DateTime.UtcNow)
                .Where(m => filters.PlayerId == null || m.PlayerMatchStatistics.Any(p => p.PlayerId == filters.PlayerId))
                .Where(m => filters.TeamId == null || m.TeamMatchStatistics.Any(t => t.TeamId == filters.TeamId))
                .Where(m => m.TeamHomeId != null && m.TeamAwayId != null)
                .Where(m => filters.TournamentId == null || m.TournamentId == filters.TournamentId)
                .Where(m => filters.IncludeUnpublished || m.TournamentId == null || m.Tournament.IsPublic)
                .Where(m => filters.DateFrom == null || m.KickOff > filters.DateFrom)
                .Where(m => filters.DateTo == null || m.KickOff < filters.DateTo)
                .Where(m => filters.TimePeriod != StatisticsTimePeriod.Week || m.KickOff > DateTime.UtcNow.AddDays(-7))
                .Where(m => filters.TimePeriod != StatisticsTimePeriod.Month || m.KickOff > DateTime.UtcNow.AddMonths(-1))
                .Where(m => filters.TimePeriod != StatisticsTimePeriod.Year || m.KickOff > DateTime.UtcNow.AddYears(-1));

            return queryable.GetPaged(page, pageSize, sortOrder);
        }

        public void UpdateMatch(Match match)
        {
            var existingMatch = _coachBotContext.Matches.Single(m => m.Id == match.Id);
            existingMatch.KickOff = match.KickOff;
            existingMatch.ServerId = match.ServerId;
            _coachBotContext.SaveChanges();
        }

        public void UpdateServerForMatch(int matchId, int serverId)
        {
            var existingMatch = _coachBotContext.Matches.Single(m => m.Id == matchId);
            existingMatch.ServerId = serverId;
            _coachBotContext.SaveChanges();
        }

    }
}