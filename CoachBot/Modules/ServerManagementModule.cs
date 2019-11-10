﻿using CoachBot.Domain.Services;
using CoachBot.Preconditions;
using CoachBot.Services;
using CoachBot.Tools;
using Discord.Commands;
using System.Threading.Tasks;

namespace CoachBot.Modules
{
    public class ServerManagementModule : ModuleBase
    {
        private readonly ServerService _serverService;
        private readonly ChannelService _channelService;
        private readonly ServerManagementService _discordServerService;

        public ServerManagementModule(ServerService serverService, ChannelService channelService, ServerManagementService discordServerService)
        {
            _serverService = serverService;
            _channelService = channelService;
            _discordServerService = discordServerService;
        }

        [Command("!enablesinglekeeper")]
        [RequireChannelConfigured]
        public async Task EnableSingleKeeperAsync(int serverListItemId)
        {
            if (_discordServerService.ValidateServer(Context.Channel.Id, serverListItemId))
            {
                var channel = _channelService.GetChannelByDiscordId(Context.Message.Channel.Id);
                var server = _serverService.GetServersByRegion((int)channel.RegionId)[serverListItemId - 1];
                await _discordServerService.ToggleSingleKeeper(server.Id, true);
            }
            else
            {
                await ReplyAsync("", embed: EmbedTools.GenerateSimpleEmbed("Invalid server ID provided"));
            }
        }

        [Command("!disablesinglekeeper")]
        [RequireChannelConfigured]
        public async Task DisableSingleKeeperAsync(int serverListItemId)
        {
            if (_discordServerService.ValidateServer(Context.Channel.Id, serverListItemId))
            {
                var channel = _channelService.GetChannelByDiscordId(Context.Message.Channel.Id);
                var server = _serverService.GetServersByRegion((int)channel.RegionId)[serverListItemId - 1];
                await _discordServerService.ToggleSingleKeeper(server.Id, false);
            }
            else
            {
                await ReplyAsync("", embed: EmbedTools.GenerateSimpleEmbed("Invalid server ID provided"));
            }
        }

        [Command("!servers")]
        public async Task ServersAsync()
        {
            var response = _discordServerService.GenerateServerListEmbed(Context.Message.Channel.Id);
            await ReplyAsync("", embed: response);
        }

        [Command("!serverinfo")]
        public async Task ServerInfoAsync(int serverListItemId)
        {
            if (_discordServerService.ValidateServer(Context.Channel.Id, serverListItemId))
            {
                var channel = _channelService.GetChannelByDiscordId(Context.Message.Channel.Id);
                var server = _serverService.GetServersByRegion((int)channel.RegionId)[serverListItemId - 1];
                await ReplyAsync("", embed: _discordServerService.GetServerInfo(server.Id));
            }
            else
            {
                await ReplyAsync("", embed: EmbedTools.GenerateSimpleEmbed("Invalid server ID provided"));
            }
        }
    }
}