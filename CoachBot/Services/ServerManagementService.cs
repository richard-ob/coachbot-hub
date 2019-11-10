using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Model;
using CoachBot.Tools;
using Discord;
using Discord.WebSocket;
using RconSharp;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoachBot.Services
{
    public class ServerManagementService
    {
        private readonly ServerService _serverService;
        private readonly ChannelService _channelService;
        private readonly MatchService _matchService;
        private readonly DiscordSocketClient _discordClient;

        public ServerManagementService(ServerService serverService, ChannelService channelService, MatchService matchService, DiscordSocketClient discordClient)
        {
            _serverService = serverService;
            _channelService = channelService;
            _matchService = matchService;
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
            if (server == null || string.IsNullOrEmpty(server.RconPassword) || !server.Address.Contains(":")) return;

            INetworkSocket socket = new RconSocket();
            RconMessenger messenger = new RconMessenger(socket);
            bool isConnected = await messenger.ConnectAsync(server.Address.Split(':')[0], int.Parse(server.Address.Split(':')[1]));
            bool authenticated = await messenger.AuthenticateAsync(server.RconPassword);
            if (authenticated)
            {
                await messenger.ExecuteCommandAsync($"sv_singlekeeper {(enable ? 1 : 0)}");
                await messenger.ExecuteCommandAsync($"say \"Single keeper {(enable ? "enabled" : "disabled")} by Coach\"");
            }
            messenger.CloseConnection();

            return;
        }

        public Embed GetServerInfo(int serverId)
        {
            var server = _serverService.GetServer(serverId);
            var gameServer = new GameServerQuery(server.Address);

            if (gameServer != null)
            {
                var embedBuilder = new EmbedBuilder();
                embedBuilder.AddField("Name", gameServer.Name);
                embedBuilder.AddField("Address", $"steam://{server.Address}");
                embedBuilder.AddField("Type", gameServer.ServerType);
                embedBuilder.AddField("Players", $"{gameServer.Players}/{gameServer.MaxPlayers}");
                embedBuilder.AddField("Map", gameServer.Map);
                embedBuilder.AddField("OS", gameServer.Environment.ToString() == "Windows" ? "<:windows:642830761732341771>" : "<:linux:642830761891987497>");
                embedBuilder.AddField("VAC", gameServer.VAC);
                embedBuilder.AddField("Game", gameServer.Game);

                return embedBuilder.Build();
            }
            else
            {
                return EmbedTools.GenerateSimpleEmbed("");
            }
        }

        public async Task<Embed> GenerateMapListAsync(int serverId)
        {
            var server = _serverService.GetServer(serverId);
            string maps = await GetMapListAsync(server);            

            if (!string.IsNullOrWhiteSpace(maps))
            {
                var embedBuilder = new EmbedBuilder();
                embedBuilder.AddField("Maps", maps);

                return embedBuilder.Build();
            }
            else
            {
                return EmbedTools.GenerateSimpleEmbed("An error occurred.");
            }
        }

        public async Task<Embed> ChangeMapAsync(int serverId, string mapName)
        {
            var server = _serverService.GetServer(serverId);
            string maps = await GetMapListAsync(server);

            if (!maps.Contains(mapName) || mapName.Length < 6)
            {
                return EmbedTools.GenerateSimpleEmbed("This server does not have this map available.");
            }

            if (!string.IsNullOrEmpty(server.RconPassword) && server.Address.Contains(":"))
            {
                try
                {
                    INetworkSocket socket = new Extensions.RconSocket();
                    RconMessenger messenger = new RconMessenger(socket);
                    bool isConnected = await messenger.ConnectAsync(server.Address.Split(':')[0], int.Parse(server.Address.Split(':')[1]));
                    bool authenticated = await messenger.AuthenticateAsync(server.RconPassword);
                    if (authenticated)
                    {
                        await messenger.ExecuteCommandAsync($"changelevel {mapName}");
                    }
                    messenger.CloseConnection();

                    return EmbedTools.GenerateSimpleEmbed("Map succesfully changed to " + mapName);
                }
                catch { }
            }

            return EmbedTools.GenerateSimpleEmbed("An error occurred.");
        }

        private async Task<string> GetMapListAsync(Server server)
        {
            if (!string.IsNullOrEmpty(server.RconPassword) && server.Address.Contains(":"))
            {
                try
                {
                    string maps = "";
                    INetworkSocket socket = new Extensions.RconSocket();
                    RconMessenger messenger = new RconMessenger(socket);
                    bool isConnected = await messenger.ConnectAsync(server.Address.Split(':')[0], int.Parse(server.Address.Split(':')[1]));
                    bool authenticated = await messenger.AuthenticateAsync(server.RconPassword);
                    if (authenticated)
                    {
                        maps = await messenger.ExecuteCommandAsync("maps *");
                        maps = maps.Replace("-------------\n", "");
                        maps = maps.Replace("PENDING:", "");
                        maps = maps.Replace("(fs) ", "");
                        maps = maps.Replace(".bsp", "");
                    }
                    messenger.CloseConnection();

                    return maps;
                }
                catch { }
            }

            return "";
        }

        public async Task<bool> SetupServer(int serverId, ulong channelId)
        {
            var server = _serverService.GetServer(serverId);
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var match = _matchService.GetCurrentMatchForChannel(channelId);
            var discordChannel = _discordClient.GetChannel(channelId) as SocketTextChannel;
            var matchFormat = channel.ChannelPositions.Count + "v" + channel.ChannelPositions.Count;
            
            var gameServerQuery = new GameServerQuery(server.Address);
            if (gameServerQuery == null)
            {
                await discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":no_entry: The server seems to be offline. Please choose another server and use the !ready command again.").Build());

                return false;
            }

            if (!gameServerQuery.Map.StartsWith(matchFormat) && !string.IsNullOrEmpty(server.RconPassword))
            {
                await discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(
                    $":no_entry: The server appears to be on a map (`{gameServerQuery.Map}`) that does not match the current format ({matchFormat}). Please use the `!map` command to change the map, and then use `!ready` again."
                ).Build());

                return false;
            }

            if (int.TryParse(gameServerQuery.Map.Substring(0), out int mapFormat) && gameServerQuery.Players > mapFormat)
            {
                await discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":no_entry: The selected server seems to be in use, as there are more than {channel.ChannelPositions.Count} on the server.").Build());

                return false;
            }

            if (!string.IsNullOrEmpty(server.RconPassword) && server.Address.Contains(":"))
            {
                try
                {
                    INetworkSocket socket = new Extensions.RconSocket();
                    RconMessenger messenger = new RconMessenger(socket);
                    bool isConnected = await messenger.ConnectAsync(server.Address.Split(':')[0], int.Parse(server.Address.Split(':')[1]));
                    bool authenticated = await messenger.AuthenticateAsync(server.RconPassword);
                    if (authenticated)
                    {
                        await messenger.ExecuteCommandAsync($"exec {channel.ChannelPositions.Count}v{channel.ChannelPositions.Count}.cfg");
                        if (match.TeamHome.PlayerTeamPositions.Any(p => p.Position.Name.ToUpper() == "GK") || match.TeamAway.PlayerTeamPositions.Any(p => p.Position.Name.ToUpper() == "GK"))
                        {
                            await messenger.ExecuteCommandAsync("sv_singlekeeper 0");
                        }
                        else
                        {
                            await messenger.ExecuteCommandAsync("sv_singlekeeper 1");
                        }
                        await messenger.ExecuteCommandAsync("say Have a great game, and remember what I taught you in training - Coach");
                        await discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(":stadium: The stadium has successfully been automatically set up").Build());
                    }
                    messenger.CloseConnection();
                }
                catch
                {
                    await discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":no_entry: The server seems to be offline. Please choose another server and use the !ready command again.").Build());

                    return false;
                }
            }

            return true;
        }
    }
}
