using CoachBot.Domain.Services;
using CoachBot.Preconditions;
using CoachBot.Services;
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
            var channel = _channelService.GetChannelByDiscordId(Context.Message.Channel.Id);
            var server = _serverService.GetServersByRegion((int)channel.RegionId)[serverListItemId - 1];
            await _discordServerService.ToggleSingleKeeper(server.Id, true);
        }

        [Command("!disablesinglekeeper")]
        [RequireChannelConfigured]
        public async Task DisableSingleKeeperAsync(int serverListItemId)
        {
            var channel = _channelService.GetChannelByDiscordId(Context.Message.Channel.Id);
            var server = _serverService.GetServersByRegion((int)channel.RegionId)[serverListItemId - 1];
            await _discordServerService.ToggleSingleKeeper(server.Id, false);
        }

        [Command("!servers")]
        public async Task ServersAsync()
        {
            var response = _discordServerService.GenerateServerListEmbed(Context.Message.Channel.Id);
            await ReplyAsync("", embed: response);
        }
    }
}