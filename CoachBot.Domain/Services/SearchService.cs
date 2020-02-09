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
            var searches = _coachBotContext.Searches.Include(s => s.Channel).ToList();

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
                .WithTitle($":mag: {challenger.BadgeEmote ?? challenger.Name} are searching for a team to face")
                .WithDescription($"To challenge **{challenger.Name}** type **!challenge {challenger.TeamCode}** and contact {startedBy} for more information")
                .WithCurrentTimestamp()
                .WithColor(challenger.SystemColor);

            var regionChannels = _coachBotContext.Channels
                .Where(c => c.RegionId == challenger.RegionId)
                .Where(c => !c.DisableSearchNotifications && c.Id != challenger.Id)
                .Where(c => c.SearchIgnoreList == null || !c.SearchIgnoreList.Any(i => i == challenger.DiscordChannelId))
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
            await Task.Delay(15 * 60 * 1000);
            if (_coachBotContext.Searches.Any(s => s.ChannelId == channelId))
            {
                var channel =_coachBotContext.Channels.Find(channelId);
                await _discordNotificationService.SendChannelMessage(channel.DiscordChannelId, ":timer: Your search for an opponent has timed out after 15 minutes.Please try again if you are still searching");
                await StopSearch(channelId);
            }
        }

        public async Task<ServiceResponse> StopSearch(int channelId)
        {
            var search =_coachBotContext.Searches.FirstOrDefault(s => s.ChannelId == channelId);
            if (search == null) return new ServiceResponse(ServiceResponseStatus.Failure, $"There is no search in progress to stop");

            _coachBotContext.Remove(search);
            _coachBotContext.SaveChanges();

            foreach(var messageChannel in search.DiscordSearchMessages.Keys)
            {
                var discordChannel = _discordSocketClient.GetChannel(messageChannel) as IMessageChannel;
                var messageId = search.DiscordSearchMessages.GetValueOrDefault(messageChannel);
                await discordChannel.DeleteMessagesAsync(new[] { messageId });
            }

            return new ServiceResponse(ServiceResponseStatus.Success, $"Search successfully stopped");
        }

        public Match GetCurrentMatchForChannel(ulong channelId)
        {
            return _coachBotContext.Matches
                .Include(m => m.TeamHome)
                    .ThenInclude(th => th.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.TeamHome)
                    .ThenInclude(th => th.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Position)
                .Include(m => m.TeamAway)
                    .ThenInclude(ta => ta.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Player)
                .Include(m => m.TeamAway)
                    .ThenInclude(ta => ta.PlayerTeamPositions)
                    .ThenInclude(ptp => ptp.Position)
                .OrderByDescending(m => m.CreatedDate)
                .First(m => m.TeamHome.Channel.DiscordChannelId == channelId);
        }
    }
}
