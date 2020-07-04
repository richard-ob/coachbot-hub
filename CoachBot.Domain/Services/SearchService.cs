using CoachBot.Database;
using CoachBot.Domain.Model;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoachBot.Domain.Services
{
    public class SearchService
    {
        private readonly CoachBotContext _coachBotContext;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly DiscordNotificationService _discordNotificationService;

        public SearchService(CoachBotContext coachBotContext, DiscordSocketClient discordSocketClient, DiscordNotificationService discordNotificationService)
        {
            _coachBotContext = coachBotContext;
            _discordSocketClient = discordSocketClient;
            _discordNotificationService = discordNotificationService;
        }

        public List<Search> GetSearches()
        {
            var searches = _coachBotContext.Searches
                .Include(s => s.Channel)
                    .ThenInclude(c => c.Team)
                    .ThenInclude(t => t.Guild)
                .ToList();

            return searches;
        }

        public async Task<ServiceResponse> Search(int channelId, string startedBy)
        {
            var challenger = _coachBotContext.Channels.Include(c => c.ChannelPositions).FirstOrDefault(c => c.Id == channelId);
            if (challenger.IsMixChannel) return new ServiceResponse(ServiceResponseStatus.Failure, $"Mix channels cannot search for opposition");
            if (challenger.ChannelPositions.Count() - 1 > GetCurrentMatchForChannel(challenger.DiscordChannelId).SignedPlayersAndSubs.Count()) return new ServiceResponse(ServiceResponseStatus.Failure, $"All outfield positions must be filled");
            if (GetSearches().Any(c => c.ChannelId == challenger.Id)) return new ServiceResponse(ServiceResponseStatus.Failure, $"You're already searching for a match. Type **!stopsearch** to cancel the previous search.");

            var search = new Search()
            {
                CreatedDate = DateTime.UtcNow,
                ChannelId = channelId,
                DiscordSearchMessages = new Dictionary<ulong, ulong>()
            };

            _coachBotContext.Searches.Add(search);
            _coachBotContext.SaveChanges();
            TimeoutSearch(channelId);

            var embed = new EmbedBuilder()
                .WithTitle($":mag: {challenger.Team.BadgeEmote ?? challenger.Team.Name} are searching for a team to face")
                .WithDescription($"To challenge **{challenger.Team.Name}** type **!challenge {challenger.Team.TeamCode}** and contact {startedBy} for more information")
                .WithCurrentTimestamp()
                .WithColor(challenger.Team.SystemColor)
                .Build();

            var regionChannels = _coachBotContext.Channels
                .Where(c => c.Team.RegionId == challenger.Team.RegionId)
                .Where(c => !c.DisableSearchNotifications && c.Id != challenger.Id)
                .Where(c => c.SearchIgnoreList == null || !c.SearchIgnoreList.Any(i => i == challenger.TeamId))
                .Select(c => c.DiscordChannelId)
                .ToList();

            var discordSearchMessages = await _discordNotificationService.SendChannelMessage(regionChannels, embed);
            search.DiscordSearchMessages = discordSearchMessages;
            _coachBotContext.Searches.Update(search);
            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.Success, $"Search successfully started");
        }

        public async void TimeoutSearch(int channelId)
        {
            await Task.Delay(30 * 60 * 1000);
            if (_coachBotContext.Searches.Any(s => s.ChannelId == channelId))
            {
                var channel = _coachBotContext.Channels.Find(channelId);
                await _discordNotificationService.SendChannelMessage(channel.DiscordChannelId, ":timer: Your search for an opponent has timed out after 15 minutes. Please try again if you are still searching");
                await StopSearch(channelId);
            }
        }

        public async Task<ServiceResponse> StopSearch(int channelId)
        {
            var search = _coachBotContext.Searches.FirstOrDefault(s => s.ChannelId == channelId);
            if (search == null) return new ServiceResponse(ServiceResponseStatus.Failure, $"There is no search in progress to stop");

            _coachBotContext.Remove(search);
            _coachBotContext.SaveChanges();

            foreach (var messageChannel in search.DiscordSearchMessages.Keys)
            {
                var discordChannel = _discordSocketClient.GetChannel(messageChannel) as ITextChannel;
                var messageId = search.DiscordSearchMessages.GetValueOrDefault(messageChannel);
                await discordChannel.DeleteMessagesAsync(new[] { messageId });
            }

            return new ServiceResponse(ServiceResponseStatus.Success, $"Search successfully stopped");
        }

        public Match GetCurrentMatchForChannel(ulong channelId)
        {
            return _coachBotContext.Matches
                .Include(m => m.LineupHome)
                    .ThenInclude(th => th.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.LineupHome)
                    .ThenInclude(th => th.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Position)
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.LineupAway)
                    .ThenInclude(ta => ta.PlayerLineupPositions)
                    .ThenInclude(ptp => ptp.Position)
                .OrderByDescending(m => m.CreatedDate)
                .First(m => m.LineupHome.Channel.DiscordChannelId == channelId);
        }
    }
}