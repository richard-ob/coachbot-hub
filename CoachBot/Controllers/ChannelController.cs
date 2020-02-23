using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class ChannelController : Controller
    {
        private readonly BotService _botService;
        private readonly ChannelService _channelService;
        private readonly DiscordSocketClient _client;

        public ChannelController(BotService botService, ChannelService channelService, DiscordSocketClient client)
        {
            _botService = botService;
            _channelService = channelService;
            _client = client;
        }

        [HttpGet]
        public IList<Channel> Get()
        {
            var userId = ulong.Parse(User.Claims.ToList().First().Value);
            var channels = _channelService.GetChannelsForUser(userId, false);
            foreach (var channel in channels)
            {
                var guildChannel = (SocketGuildChannel)_client.GetChannel(channel.DiscordChannelId);
                if (guildChannel != null)
                {
                    channel.DiscordChannelName = guildChannel.Name;
                    channel.Team = new Team()
                    {
                        Guild = new Guild() { Name = guildChannel.Name, DiscordGuildId = guildChannel.Guild.Id }
                    };
                }
            }
            return channels;
        }

        [HttpGet("{id}")]
        public Channel Get(ulong id)
        {
            var userId = ulong.Parse(User.Claims.ToList().First().Value);
            var channel = _channelService.GetChannelsForUser(userId, false).First(c => c.DiscordChannelId == id);
            var guildChannel = (SocketGuildChannel)_client.GetChannel(id);
            channel.DiscordChannelName = guildChannel.Name;
            channel.Team = new Team()
            {
                Guild = new Guild() { Name = guildChannel.Guild.Name, DiscordGuildId = guildChannel.Guild.Id }
            };

            return channel;
        }


        [HttpGet("unconfigured")]
        public IList<Channel> GetUnconfiguredChannels()
        {
            var userId = ulong.Parse(User.Claims.ToList().First().Value);
            var channels = _channelService.GetChannelsForUser(userId, true);
            foreach (var channel in channels)
            {
                var guildChannel = (SocketGuildChannel)_client.GetChannel(channel.DiscordChannelId);
                if (guildChannel != null)
                {
                    channel.DiscordChannelName = guildChannel.Name;
                    channel.Team = new Team()
                    {
                        Guild = new Guild() { Name = guildChannel.Guild.Name, DiscordGuildId = guildChannel.Guild.Id }
                    };
                }
            }

            return channels;
        }

        [HttpPut]
        public void Update([FromBody]Channel channel)
        {
            _channelService.Update(channel);
        }

        [HttpPost]
        public void Create([FromBody]Channel channel)
        {
            _channelService.Create(channel);
        }
    }
}
