using CoachBot.Extensions;
using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class ChannelController : Controller
    {
        private readonly MatchmakerService _matchmakerService;
        private readonly BotService _botService;
        private DiscordSocketClient _client;

        public ChannelController(MatchmakerService matchmakerService, BotService botService, DiscordSocketClient client)
        {
            _matchmakerService = matchmakerService;
            _botService = botService;
            _client = client;
        }

        [HttpGet]
        public IList<Channel> Get()
        {
            var userId = ulong.Parse(User.Claims.ToList().First().Value);
            var channels = _botService.GetChannelsForUser(userId, false);
            foreach (var channel in channels)
            {
                var guildChannel = (SocketGuildChannel)_client.GetChannel(channel.Id);
                if (guildChannel != null)
                {
                    channel.Name = guildChannel.Name;
                    channel.GuildName = guildChannel.Guild.Name;
                }
            }
            return channels;
        }

        [HttpGet("{id}")]
        public Channel Get(ulong id)
        {
            var userId = ulong.Parse(User.Claims.ToList().First().Value);
            var channel = _botService.GetChannelsForUser(userId, false).First(c => c.Id == id);
            var guildChannel = (SocketGuildChannel)_client.GetChannel(id);
            channel.Name = guildChannel.Name;
            channel.GuildName = guildChannel.Guild.Name;
            channel.Emotes = guildChannel.Guild.Emotes.Select(e => new KeyValuePair<string, string>($"<:{e.Name}:{e.Id}>", e.Url));

            return channel;
        }


        [HttpGet("unconfigured")]
        public IList<Channel> GetUnconfiguredChannels()
        {
            var userId = ulong.Parse(User.Claims.ToList().First().Value);
            var channels = _botService.GetChannelsForUser(userId, true);
            foreach (var channel in channels)
            {
                var guildChannel = (SocketGuildChannel)_client.GetChannel(channel.Id);
                if (guildChannel != null)
                {
                    channel.Name = guildChannel.Name;
                    channel.GuildName = guildChannel.Guild.Name;
                }
            }
            return channels;
        }

        [HttpPost]
        public void Update([FromBody]Channel channel)
        {
            _matchmakerService.ConfigureChannel(channel.Id, channel.Team1.Name, channel.Positions, channel.RegionId, channel.Team1.KitEmote, channel.Team1.BadgeEmote, channel.Team1.Color, channel.IsMixChannel, channel.Formation, channel.ClassicLineup, channel.DisableSearchNotifications);
        }
    }
}
