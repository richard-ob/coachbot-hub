using CoachBot.Bot.Preconditions;
using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Preconditions;
using CoachBot.Services;
using CoachBot.Shared.Extensions;
using CoachBot.Tools;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace CoachBot.Modules
{
    [RequireChannelConfigured]
    [RequireChannelAndTeamActive]
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

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);
            CallContext.SetData(CallContextDataType.DiscordUser, Context.Message.Author.Username);
            Context.Message.AddReactionAsync(new Emoji("⚙️"));
        }

        protected override void AfterExecute(CommandInfo command)
        {
            base.AfterExecute(command);

            Context.Message.AddReactionAsync(new Emoji("✅"));
        }

        [Command("!enablesinglekeeper")]
        public async Task EnableSingleKeeperAsync(int serverListItemId)
        {
            if (_discordServerService.ValidateServer(Context.Channel.Id, serverListItemId))
            {
                var channel = _channelService.GetChannelByDiscordId(Context.Message.Channel.Id);
                var server = _serverService.GetServersByRegion((int)channel.Team.RegionId)[serverListItemId - 1];
                await _discordServerService.ToggleSingleKeeper(server.Id, true);
            }
            else
            {
                await ReplyAsync("", embed: DiscordEmbedHelper.GenerateEmbed("Invalid server ID provided. Use `!servers` to see the full server list.", ServiceResponseStatus.Failure));
            }
        }

        [Command("!disablesinglekeeper")]
        public async Task DisableSingleKeeperAsync(int serverListItemId)
        {
            if (_discordServerService.ValidateServer(Context.Channel.Id, serverListItemId))
            {
                var channel = _channelService.GetChannelByDiscordId(Context.Message.Channel.Id);
                var server = _serverService.GetServersByRegion((int)channel.Team.RegionId)[serverListItemId - 1];
                await _discordServerService.ToggleSingleKeeper(server.Id, false);
            }
            else
            {
                await ReplyAsync("", embed: DiscordEmbedHelper.GenerateEmbed("Invalid server ID provided. Use `!servers` to see the full server list.", ServiceResponseStatus.Failure));
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
                var server = _serverService.GetServersByRegion((int)channel.Team.RegionId)[serverListItemId - 1];
                await ReplyAsync("", embed: _discordServerService.GetServerInfo(server.Id));
            }
            else
            {
                await ReplyAsync("", embed: DiscordEmbedHelper.GenerateEmbed("Invalid server ID provided. Use `!servers` to see the full server list.", ServiceResponseStatus.Failure));
            }
        }

        [Command("!maps")]
        public async Task ListMapsAsync(int serverListItemId = 0)
        {
            if (_discordServerService.ValidateServer(Context.Channel.Id, serverListItemId))
            {
                var server = _discordServerService.GetServerFromServerListItemId(serverListItemId, Context.Message.Channel.Id);
                var response = await _discordServerService.GenerateMapListAsync(server.Id);
                await ReplyAsync("", embed: response);
            }
            else
            {
                await ReplyAsync("", embed: DiscordEmbedHelper.GenerateEmbed("Invalid server ID provided. Use `!servers` to see the full server list.", ServiceResponseStatus.Failure));
            }
        }

        [Command("!kits")]
        public async Task ListKitsAsync(int serverListItemId = 0)
        {
            if (_discordServerService.ValidateServer(Context.Channel.Id, serverListItemId))
            {
                var response = await _discordServerService.GenerateKitListAsync(serverListItemId, Context.Channel.Id);
                await ReplyAsync("", embed: response);
            }
            else
            {
                await ReplyAsync("", embed: DiscordEmbedHelper.GenerateEmbed("Invalid server ID provided. Use `!servers` to see the full server list.", ServiceResponseStatus.Failure));
            }
        }

        [Command("!changemap")]
        public async Task ChangeMapsAsync(int serverListItemId = 0, string mapName = "")
        {
            if (_discordServerService.ValidateServer(Context.Channel.Id, serverListItemId) && !string.IsNullOrWhiteSpace(mapName))
            {
                var response = await _discordServerService.ChangeMapAsync(serverListItemId, Context.Channel.Id, mapName);
                await ReplyAsync("", embed: response);
            }
            else
            {
                await ReplyAsync("", embed: DiscordEmbedHelper.GenerateEmbed("Invalid server ID or map name provided. Use `!servers` to see the full server list or `!maps <server id>` for the full map list.", ServiceResponseStatus.Failure));
            }
        }

        [Command("!execconfig")]
        public async Task ExecConfigAsync(int serverListItemId = 0)
        {
            if (_discordServerService.ValidateServer(Context.Channel.Id, serverListItemId))
            {
                _discordServerService.ExecConfigAsync(serverListItemId, Context.Channel.Id, Context.Message.Author);
            }
            else
            {
                await ReplyAsync("", embed: DiscordEmbedHelper.GenerateEmbed("Invalid server ID or map name provided. Use `!servers` to see the full server list or `!maps <server id>` for the full map list.", ServiceResponseStatus.Failure));
            }
        }

        [Command("!changekits")]
        public async Task ChangeKitsAsync(int serverListItemId = 0, int homeKitId = 0, int awayKitId = 0)
        {
            if (_discordServerService.ValidateServer(Context.Channel.Id, serverListItemId) && homeKitId > 0 && awayKitId > 0)
            {
                _discordServerService.ChangeKitsAsync(serverListItemId, Context.Channel.Id, Context.Message.Author, homeKitId, awayKitId);
            }
            else
            {
                await ReplyAsync("", embed: DiscordEmbedHelper.GenerateEmbed("Invalid server ID or kit ID provided. Use `!servers` to see the full server list or `!kits <server id>` for the full kit list.", ServiceResponseStatus.Failure));
            }
        }
    }
}