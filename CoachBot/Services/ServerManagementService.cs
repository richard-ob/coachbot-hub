using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Tools;
using Discord;
using Discord.WebSocket;
using RconSharp;
using System.Text;
using System.Threading.Tasks;

namespace CoachBot.Services
{
    public class ServerManagementService
    {
        private readonly ServerService _serverService;
        private readonly ChannelService _channelService;
        private readonly DiscordSocketClient _discordClient;

        public ServerManagementService(ServerService serverService, ChannelService channelService, DiscordSocketClient discordClient)
        {
            _serverService = serverService;
            _channelService = channelService;
            _discordClient = discordClient;
        }

        public bool ValidateServer(ulong channelId, int serverListItemId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId, false);
            var servers = _serverService.GetServersByRegion((int)channel.RegionId);

            return servers.Count >= serverListItemId && serverListItemId > 0;
        }

        public Embed GenerateServerListEmbed(ulong channelId)
        {
            var serverId = 1;
            var embedBuilder = new EmbedBuilder().WithTitle(":desktop: Servers");
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var servers = _serverService.GetServersByRegion((int)channel.RegionId);

            foreach (var server in servers)
            {
                var gameServer = new GameServerQuery(server.Address);
                var autoSetup = !string.IsNullOrEmpty(server.RconPassword) ? "**[Auto Setup]**" : "";
                var sb = new StringBuilder();

                sb.Append($"```{serverId}``` ");

                if (gameServer.Name != null)
                {
                    sb.Append($"{gameServer.Name} ");
                    if (gameServer.CountryCode != null)
                    {
                        sb.Append($":flag_{gameServer.CountryCode}: ".ToLower());
                    }
                    sb.Append($"`[{gameServer.Players}/{gameServer.MaxPlayers}]`");
                }
                else
                {
                    sb.Append($"~~{server.Name}~~ [OFFLINE]");
                }

                if(!string.IsNullOrEmpty(server.RconPassword))
                {
                    sb.Append("**[Auto Setup]**");
                }

                embedBuilder.AddField(sb.ToString(), $"steam://connect/{server.Address}");
                serverId++;
            }

            return embedBuilder.Build();
        }

        public async Task ToggleSingleKeeper(int serverId, bool enable)
        {
            var server = _serverService.GetServer(serverId);
            if (string.IsNullOrEmpty(server.RconPassword) || !server.Address.Contains(":"))
            {
                return;
            }
            INetworkSocket socket = new RconSocket();
            RconMessenger messenger = new RconMessenger(socket);
            bool isConnected = await messenger.ConnectAsync(server.Address.Split(':')[0], int.Parse(server.Address.Split(':')[1]));
            bool authenticated = await messenger.AuthenticateAsync(server.RconPassword);
            if (authenticated)
            {
                await messenger.ExecuteCommandAsync($"sv_singlekeeper {(enable ? 1 : 0)}");
                await messenger.ExecuteCommandAsync($"say \"Single keeper {(enable ? "enabled" : "disabled")} by Coach\"");
            }

            return;
        }
    }
}
