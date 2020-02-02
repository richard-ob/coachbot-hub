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
            var searches = _coachBotContext.Searches.ToList();

            return searches;
        }

        public ServiceResponse Search(int channelId)
        {
            var challenger = _coachBotContext.Channels.Include(c => c.ChannelPositions).FirstOrDefault(c => c.Id == channelId);
            if (challenger.IsMixChannel) return new ServiceResponse(ServiceResponseStatus.Failure, $"Mix channels cannot search for opposition");
            if (challenger.ChannelPositions.Count() - 1 > GetCurrentMatchForChannel(challenger.DiscordChannelId).SignedPlayersAndSubs.Count()) return new ServiceResponse(ServiceResponseStatus.Failure, $"All outfield positions must be filled");
            if (GetSearches().Any(c => c.ChannelId == challenger.Id)) return new ServiceResponse(ServiceResponseStatus.Failure, $"You're already searching for a match. Type **!stopsearch** to cancel the previous search.");

            var search = new Search()
            {
                CreatedDate = DateTime.UtcNow,
                ChannelId = channelId,
                DiscordMessageIds = new List<ulong>()
            };

            _coachBotContext.Searches.Add(search);
            _coachBotContext.SaveChanges();
            TimeoutSearch(channelId).ConfigureAwait(false);

            return new ServiceResponse(ServiceResponseStatus.Success, $"Search successfully started");
        }

        public async Task TimeoutSearch(int channelId)
        {
            await Task.Delay(15 * 60 * 1000);
            if (_coachBotContext.Searches.Any(s => s.ChannelId == channelId))
            {
                var channel =_coachBotContext.Channels.Find(channelId);
                _discordNotificationService.SendChannelMessage(channel.DiscordChannelId, ":timer: Your search for an opponent has timed out after 15 minutes.Please try again if you are still searching");
                StopSearch(channelId);
            }
        }

        public ServiceResponse StopSearch(int channelId)
        {
            var search =_coachBotContext.Searches.FirstOrDefault(s => s.ChannelId == channelId);
            if (search == null) return new ServiceResponse(ServiceResponseStatus.Failure, $"There is no search in progress");

            _coachBotContext.Remove(search);
            _coachBotContext.SaveChanges();

            var discordChannelId = _coachBotContext.Channels.Find(channelId).DiscordChannelId;
            var discordChannel = _discordSocketClient.GetChannel(discordChannelId) as ITextChannel;
            discordChannel.DeleteMessagesAsync(search.DiscordMessageIds);

            return new ServiceResponse(ServiceResponseStatus.Success, $"Search cancelled");
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
