using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Model;
using CoachBot.Services.Matchmaker;
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
    public class GuildController : Controller
    {
        private readonly GuildService _guildService;
        private readonly ChannelService _channelService;

        public GuildController(GuildService guildService, ChannelService channelService)
        {
            _guildService = guildService;
            _channelService = channelService;
        }

        [HttpGet("{guildId}")]
        public Guild Get(ulong guildId)
        {
            return _guildService.GetGuildByDiscordId(guildId, true);
        }

        [HttpGet("{guildId}/channels")]
        public List<Channel> GetChannelsForGuild(ulong guildId)
        {
            return _channelService.GetChannelsForGuild(guildId);
        }
    }
}
