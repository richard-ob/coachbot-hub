using CoachBot.Database;
using CoachBot.Domain.Model;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class SearchService
    {
        private readonly CoachBotContext _coachBotContext;
        private readonly DiscordSocketClient _discordSocketClient;

        public SearchService(CoachBotContext coachBotContext, DiscordSocketClient discordSocketClient)
        {
            _coachBotContext = coachBotContext;
            _discordSocketClient = discordSocketClient;
        }

        public List<Search> GetSearches()
        {
            return _coachBotContext.Searches.ToList();
        }

        public ServiceResponse Search(int channelId)
        {
            var challenger = _coachBotContext.Channels.FirstOrDefault(c => c.Id == channelId);
            if (challenger.IsMixChannel) return new ServiceResponse(ServiceResponseStatus.Failure, $":no_entry: Mix channels cannot search for opposition");
            if (challenger.ChannelPositions.Count() - 1 > GetCurrentMatchForChannel(challenger.DiscordChannelId).SignedPlayers.Count()) return new ServiceResponse(ServiceResponseStatus.Failure, $":no_entry: All outfield positions must be filled");
            if (GetSearches().Any(c => c.ChannelId == challenger.Id)) return new ServiceResponse(ServiceResponseStatus.Failure, $":no_entry: You're already searching for a match. Type **!stopsearch** to cancel the previous search.");

            var search = new Search()
            {
                CreatedDate = DateTime.UtcNow,
                ChannelId = channelId,
                DiscordMessageIds = new List<ulong>()
            };

            _coachBotContext.Searches.Add(search);
            _coachBotContext.SaveChanges();

            return new ServiceResponse(ServiceResponseStatus.Success, $"Search successfully started");
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
