using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Model;
using CoachBot.Tools;
using Discord;
using Discord.WebSocket;
using RconSharp;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            if (gameServer != null && gameServer.Game != null)
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

        public async Task<Embed> ChangeMapAsync(int serverListItemId, ulong channelId, string mapName)
        {
            var server = GetServerFromServerListItemId(serverListItemId, channelId);
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

        public async Task<Embed> GenerateKitListAsync(int serverListItemId, ulong channelId)
        {
            var server = GetServerFromServerListItemId(serverListItemId, channelId);

            if (!string.IsNullOrEmpty(server.RconPassword) && server.Address.Contains(":"))
            {
                try
                {
                    string kits = "";
                    INetworkSocket socket = new Extensions.RconSocket();
                    RconMessenger messenger = new RconMessenger(socket);
                    bool isConnected = await messenger.ConnectAsync(server.Address.Split(':')[0], int.Parse(server.Address.Split(':')[1]));
                    bool authenticated = await messenger.AuthenticateAsync(server.RconPassword);
                    if (authenticated)
                    {
                        kits = await messenger.ExecuteCommandAsync("mp_teamkits");
                    }
                    messenger.CloseConnection();

                    Regex regex = new Regex(@"[\d]+   [A-Za-z]+");
                    StringBuilder sb = new StringBuilder();

                    foreach (var match in regex.Matches(kits))
                    {
                        sb.AppendLine("`" + match.ToString()).Replace("   ", "` ");
                    }

                    return EmbedTools.GenerateSimpleEmbed(sb.ToString(), ":shirt: Kits (" + server.Name + ")");
                }
                catch
                {
                    return EmbedTools.GenerateSimpleEmbed("An error occurred");
                }
            }
            else
            {
                return EmbedTools.GenerateSimpleEmbed("This server is not configured for server management");
            }
        }

        public async void ChangeKitsAsync(int serverListItemId, ulong channelId, IUser user, int homeKitId, int awayKitId)
        {
            var server = GetServerFromServerListItemId(serverListItemId, channelId);
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var match = _matchService.GetCurrentMatchForChannel(channelId);
            var discordChannel = _discordClient.GetChannel(channelId) as SocketTextChannel;

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
                        await messenger.ExecuteCommandAsync($"exec mp_teamkits {homeKitId} {awayKitId}");
                        await messenger.ExecuteCommandAsync($"say [Coach] Kits set from Discord by {user.Username}");
                        await discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(":stadium: The kits have been changed successfully").Build());
                    }
                    messenger.CloseConnection();
                }
                catch
                {
                    await discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":no_entry: The kits could not be set, {user.Mention}").Build());
                }
            }
        }

        public async Task<bool> ValidateServerAvailability(int serverListItemId, ulong channelId)
        {
            var server = GetServerFromServerListItemId(serverListItemId, channelId);
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var discordChannel = _discordClient.GetChannel(channelId) as SocketTextChannel;
            var matchFormat = channel.ChannelPositions.Count + "v" + channel.ChannelPositions.Count;
            var gameServerQuery = new GameServerQuery(server.Address);

            if (gameServerQuery == null || gameServerQuery.Game == null)
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

            return true;
        }

        public async void PrepareServer(int serverListItemId, ulong channelId, int matchId)
        {
            var server = GetServerFromServerListItemId(serverListItemId, channelId);
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var match = _matchService.GetCurrentMatchForChannel(channelId);
            var discordChannel = _discordClient.GetChannel(channelId) as SocketTextChannel;
            var matchFormat = channel.ChannelPositions.Count + "v" + channel.ChannelPositions.Count;

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
                        await messenger.ExecuteCommandAsync("sv_webserver_matchdata_url " + "http://localhost/api/matchstatistic");
                        await messenger.ExecuteCommandAsync("sv_webserver_matchdata_enabled 1");
                        await messenger.ExecuteCommandAsync($"sv_webserver_matchdata_accesstoken " + GenerateMatchDataAuthToken(server, matchId));
                        await messenger.ExecuteCommandAsync("say Have a great game, and remember what I taught you in training - Coach");
                        await discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(":stadium: The stadium has successfully been automatically set up").Build());
                    }
                    messenger.CloseConnection();
                }
                catch
                {
                    await discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":no_entry: The server could not be auto-configured. You may need to set the server up manually.").Build());
                }
            }
        }

        public async void ExecConfigAsync(int serverListItemId, ulong channelId, IUser user)
        {
            var server = GetServerFromServerListItemId(serverListItemId, channelId);
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var match = _matchService.GetCurrentMatchForChannel(channelId);
            var discordChannel = _discordClient.GetChannel(channelId) as SocketTextChannel;
            var matchFormat = channel.ChannelPositions.Count + "v" + channel.ChannelPositions.Count;

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
                        await messenger.ExecuteCommandAsync($"say [Coach] match config execution triggered from Discord by {user.Username}");
                        await discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(":stadium: The stadium has successfully been automatically set up").Build());
                    }
                    messenger.CloseConnection();
                }
                catch
                {
                    await discordChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":no_entry: The server config could not be executed.").Build());
                }
            }
        }

        public Server GetServerFromServerListItemId(int serverListItemId, ulong channelId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId, false);
            var servers = _serverService.GetServersByRegion((int)channel.RegionId);

            return servers[serverListItemId - 1];
        }

        private string GenerateMatchDataAuthToken(Server server, int matchId)
        {
            var token = $"{server.Address}_{matchId}";
            var encodedToken = System.Text.Encoding.UTF8.GetBytes(token);
            return System.Convert.ToBase64String(encodedToken);
        }
    }
}
