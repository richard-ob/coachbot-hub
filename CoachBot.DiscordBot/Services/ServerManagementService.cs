using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Model;
using CoachBot.Shared.Helpers;
using CoachBot.Shared.Model;
using CoachBot.Tools;
using Discord;
using Discord.WebSocket;
using RconSharp;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoachBot.Services
{
    public class ServerManagementService
    {
        private readonly ServerService _serverService;
        private readonly ChannelService _channelService;
        private readonly MatchupService _matchupService;
        private readonly MatchService _matchService;
        private readonly DiscordSocketClient _discordClient;
        private readonly DiscordNotificationService _discordNotificationService;
        private readonly Config _config;

        public ServerManagementService(
            ServerService serverService,
            ChannelService channelService,
            MatchupService matchupService,
            MatchService matchService,
            DiscordSocketClient discordClient,
            DiscordNotificationService discordNotificationService,
            Config config)
        {
            _serverService = serverService;
            _channelService = channelService;
            _matchupService = matchupService;
            _matchService = matchService;
            _discordClient = discordClient;
            _discordNotificationService = discordNotificationService;
            _config = config;
        }

        public bool ValidateServer(ulong channelId, int serverListItemId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var servers = _serverService.GetServersByRegion((int)channel.Team.RegionId).Where(s => s.DeactivatedDate == null).ToList();

            return servers.Count >= serverListItemId && serverListItemId > 0;
        }

        public Embed GenerateServerListEmbed(ulong channelId)
        {
            var serverId = 1;
            var embedBuilder = new EmbedBuilder().WithTitle(":desktop: Servers");
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var servers = _serverService.GetServersByRegion((int)channel.Team.RegionId).Where(s => s.DeactivatedDate == null);

            foreach (var server in servers)
            {
                var gameServer = new GameServerQuery(server.Address);
                var sb = new StringBuilder();

                sb.Append($"```{serverId}``` ");

                if (gameServer.Name != null)
                {
                    sb.Append($"{gameServer.Name}  `[{gameServer.Players}/{gameServer.MaxPlayers}]`  {server.Country.DiscordFlagEmote}");
                }
                else
                {
                    sb.Append($"~~{server.Name}~~ [OFFLINE]");
                }

                if (!string.IsNullOrEmpty(server.RconPassword))
                {
                    sb.Append(" **[Auto Setup]**");
                }

                embedBuilder.AddField(sb.ToString(), $"steam://connect/{server.Address}");
                serverId++;
            }

            return embedBuilder.WithRequestedBy().WithDefaultColour().Build();
        }

        public async Task ToggleSingleKeeper(int serverId, bool enable)
        {
            var server = _serverService.GetServer(serverId);
            if (server == null || string.IsNullOrEmpty(server.RconPassword) || !ServerAddressHelper.IsValidAddress(server.Address)) return;

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

                return embedBuilder.WithRequestedBy().WithDefaultColour().Build();
            }
            else
            {
                return DiscordEmbedHelper.GenerateEmbed("Server information could not be retrieved. This server may be offline.", ServiceResponseStatus.Failure);
            }
        }

        public async Task<Embed> GenerateMapListAsync(int serverId)
        {
            var server = _serverService.GetServer(serverId);
            string maps = await GetMapListAsync(server);

            if (!string.IsNullOrWhiteSpace(maps))
            {
                var embedBuilder = new EmbedBuilder();
                embedBuilder.AddField(":map: Maps", maps);

                return embedBuilder.WithRequestedBy().WithDefaultColour().Build();
            }
            else
            {
                return DiscordEmbedHelper.GenerateEmbed($"An error occurred whilst attempting to retrieve the map list from **{server.Name}**", ServiceResponseStatus.Failure);
            }
        }

        public async Task<Embed> ChangeMapAsync(int serverListItemId, ulong channelId, string mapName)
        {
            var server = GetServerFromServerListItemId(serverListItemId, channelId);
            string maps = await GetMapListAsync(server);

            if (!maps.Contains(mapName) || mapName.Length < 6)
            {
                return DiscordEmbedHelper.GenerateEmbed($"**{server.Name}** does not have the map **{mapName}**. Please choose another.", ServiceResponseStatus.Failure);
            }

            if (IsServerInUse(server.Id))
            {
                 return DiscordEmbedHelper.GenerateEmbed($"Cannot execute command as the selected server seems to be in use.", ServiceResponseStatus.Failure);
            }

            if (!string.IsNullOrEmpty(server.RconPassword) && ServerAddressHelper.IsValidAddress(server.Address))
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

                    return DiscordEmbedHelper.GenerateEmbed($"Map successfully changed to **{mapName}** on **{server.Name}**", ServiceResponseStatus.Success);
                }
                catch { }
            }

            return DiscordEmbedHelper.GenerateEmbed($"An error occurred attempting to change the map on **{server.Name}**", ServiceResponseStatus.Failure);
        }

        private async Task<string> GetMapListAsync(Server server)
        {
            if (!string.IsNullOrEmpty(server.RconPassword) && ServerAddressHelper.IsValidAddress(server.Address))
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

            if (!string.IsNullOrEmpty(server.RconPassword) && ServerAddressHelper.IsValidAddress(server.Address))
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
                        kits = await messenger.ExecuteCommandAsync("mp_teamkits_csv");
                    }
                    messenger.CloseConnection();

                    StringBuilder sb = new StringBuilder();
                    var kitCount = 1;
                    const int KIT_LIMIT = 40;
                    foreach (var kit in kits.Split(',').Where(k => k.Contains(':') && k.Contains('_')))
                    {
                        if (kitCount > KIT_LIMIT) break;
                        var kitId = kit.Split(':')[0];
                        var kitName = kit.Split(':')[1];
                        sb.AppendLine($"`{kitId}` {kitName}");
                        kitCount++;
                    }

                    return DiscordEmbedHelper.GenerateSimpleEmbed(sb.ToString(), ":shirt: Kits (" + server.Name + ")");
                }
                catch
                {
                    return DiscordEmbedHelper.GenerateEmbed($"An error occurred trying to retrieve the kit list from **{server.Name}**", ServiceResponseStatus.Failure);
                }
            }
            else
            {
                return DiscordEmbedHelper.GenerateEmbed($"**{server.Name}** is not configured for server management", ServiceResponseStatus.Failure);
            }
        }

        public async void ChangeKitsAsync(int serverListItemId, ulong channelId, IUser user, int homeKitId, int awayKitId)
        {
            var server = GetServerFromServerListItemId(serverListItemId, channelId);
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var discordChannel = _discordClient.GetChannel(channelId) as SocketTextChannel;

            if (_serverService.CheckServerHasNoTournamentGamesScheduled(server.Id))
            {
                await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed("The selected server has a scheduled match within the next 90 minutes", ServiceResponseStatus.Failure));
                return;
            }

            if (!string.IsNullOrEmpty(server.RconPassword) && ServerAddressHelper.IsValidAddress(server.Address))
            {
                try
                {
                    INetworkSocket socket = new Extensions.RconSocket();
                    RconMessenger messenger = new RconMessenger(socket);
                    bool isConnected = await messenger.ConnectAsync(server.Address.Split(':')[0], int.Parse(server.Address.Split(':')[1]));
                    bool authenticated = await messenger.AuthenticateAsync(server.RconPassword);
                    if (authenticated)
                    {
                        await messenger.ExecuteCommandAsync($"mp_teamkits {homeKitId} {awayKitId}");
                        await messenger.ExecuteCommandAsync($"say [Coach] Kits set from Discord by {user.Username}");
                        await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed($"The kits have been changed successfully on **{server.Name}**", ServiceResponseStatus.Success));
                    }
                    messenger.CloseConnection();
                }
                catch
                {
                    await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed($"The kits could not be set on {server.Name}", ServiceResponseStatus.Failure));
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
                await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed($"The server seems to be offline. Please choose another server and use the !ready command again.", ServiceResponseStatus.Failure));

                return false;
            }

            if (!gameServerQuery.Map.StartsWith(matchFormat) && !string.IsNullOrEmpty(server.RconPassword))
            {
                await discordChannel.SendMessageAsync("", embed:
                    DiscordEmbedHelper.GenerateEmbed(
                        $"The server appears to be on a map (`{gameServerQuery.Map}`) that does not match the current format ({matchFormat}). Please use the `!map` command to change the map, and then use `!ready` again.",
                        ServiceResponseStatus.Failure
                    )
                );

                return false;
            }

            if (int.TryParse(gameServerQuery.Map.Substring(0, 1), out int mapFormat) && gameServerQuery.Players > mapFormat)
            {
                await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed($"The selected server seems to be in use, as there are more than {mapFormat} players on the server.", ServiceResponseStatus.Failure));

                return false;
            }

            if (gameServerQuery.Players > channel.ChannelPositions.Count)
            {
                await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed($"The selected server seems to be in use, as there are more than {channel.ChannelPositions.Count} players on the server.", ServiceResponseStatus.Failure));

                return false;
            }

            return true;
        }

        public bool IsServerInUse(int serverId)
        {
            var server = _serverService.GetServer(serverId);

            var gameServerQuery = new GameServerQuery(server.Address);

            if (int.TryParse(gameServerQuery.Map.Substring(0), out int mapFormat) && gameServerQuery.Players > mapFormat)
            {
                return true;
            }

            return false;
        }

        public async void PrepareServer(int serverListItemId, ulong channelId, int matchupId, bool execConfig = true)
        {
            var server = GetServerFromServerListItemId(serverListItemId, channelId);
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var matchup = _matchupService.GetMatchup(matchupId);
            var discordChannel = _discordClient.GetChannel(channelId) as SocketTextChannel;
            var matchFormat = channel.ChannelPositions.Count + "v" + channel.ChannelPositions.Count;
            var teamHomeCode = matchup.LineupHome.Channel.TeamId == matchup.LineupAway.Channel.TeamId ? matchup.LineupHome.Channel.Team.TeamCode + 'A' : matchup.LineupHome.Channel.Team.TeamCode;
            var awayTeamCode = matchup.LineupHome.Channel.TeamId == matchup.LineupAway.Channel.TeamId ? matchup.LineupAway.Channel.Team.TeamCode + 'B' : matchup.LineupAway.Channel.Team.TeamCode;

            if (DateTime.UtcNow.AddHours(-1) > matchup.ReadiedDate.Value)
            {
                await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed("This match has already taken place, and so the server cannot be changed.", ServiceResponseStatus.Failure));
                return;
            }

            if (_serverService.CheckServerHasNoTournamentGamesScheduled(server.Id))
            {
                await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed("The selected server has a scheduled match within the next 90 minutes", ServiceResponseStatus.Failure));
                return;
            }

            if (matchup.Match.ServerId != server.Id)
            {
                _matchService.UpdateServerForMatch((int)matchup.MatchId, server.Id);
            }

            if (!string.IsNullOrEmpty(server.RconPassword) && ServerAddressHelper.IsValidAddress(server.Address))
            {
                try
                {
                    INetworkSocket socket = new RconSocket();
                    RconMessenger messenger = new RconMessenger(socket);
                    bool isConnected = await messenger.ConnectAsync(server.Address.Split(':')[0], int.Parse(server.Address.Split(':')[1]));
                    bool authenticated = await messenger.AuthenticateAsync(server.RconPassword);
                    if (authenticated)
                    {
                        if (execConfig)
                        {
                            await messenger.ExecuteCommandAsync($"exec {channel.ChannelPositions.Count}v{channel.ChannelPositions.Count}.cfg");
                        }
                        if (matchup.LineupHome.HasGk && matchup.LineupAway.HasGk)
                        {
                            await messenger.ExecuteCommandAsync("sv_singlekeeper 0");
                        }
                        else
                        {
                            await messenger.ExecuteCommandAsync("sv_singlekeeper 1");
                        }
                        await messenger.ExecuteCommandAsync("mp_matchinfo \"Ranked Friendly Match\"");
                        await messenger.ExecuteCommandAsync("sv_webserver_matchdata_url \"" + _config.WebServerConfig.HubApiUrl + "/api/match-statistics" + "\"");
                        await messenger.ExecuteCommandAsync("sv_webserver_matchdata_enabled 1");
                        await messenger.ExecuteCommandAsync($"mp_teamnames \"{teamHomeCode}: {matchup.LineupHome.Channel.Team.Name}, {awayTeamCode}: {matchup.LineupAway.Channel.Team.Name}\"");
                        await messenger.ExecuteCommandAsync($"sv_webserver_matchdata_accesstoken " + GenerateMatchDataAuthToken(server, matchup.MatchId.Value, matchup.LineupHome.Channel.Team.TeamCode, matchup.LineupAway.Channel.Team.TeamCode));
                        await messenger.ExecuteCommandAsync("say Have a great game, and remember what I taught you in training - Coach");
                        var homeKitId = await GetKitForTeam(messenger, matchup.LineupHome.Channel.Team.Name);
                        var awayKitId = await GetKitForTeam(messenger, matchup.LineupAway.Channel.Team.Name);
                        if (homeKitId > 0 || awayKitId > 0)
                        {
                            await messenger.ExecuteCommandAsync($"mp_teamkits {homeKitId ?? 1} {awayKitId ?? 1}");
                        }
                        if (execConfig)
                        {
                            await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateSimpleEmbed(":stadium: The stadium has successfully been automatically set up"));
                        }
                        else
                        {
                            await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateSimpleEmbed(":stadium: The stadium has successfully been automatically set up for statistics, however the match config has not been executed, as the server may be in use"));
                        }
                    }
                    else
                    {
                        await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed("The server could not be auto-configured. You may need to set the server up manually.", ServiceResponseStatus.Failure));
                    }
                    messenger.CloseConnection();
                }
                catch
                {
                    await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed("The server could not be auto-configured. You may need to set the server up manually.", ServiceResponseStatus.Failure));
                }
            }
        }

        public async Task PrepareServerTournament(int matchId)
        {
            var match = _matchService.GetMatch(matchId);
            var server = match.Server;

            if (!string.IsNullOrEmpty(server.RconPassword) && ServerAddressHelper.IsValidAddress(server.Address))
            {
                if (match.Map != null)
                {
                    try
                    {
                        INetworkSocket socket = new RconSocket();
                        RconMessenger messenger = new RconMessenger(socket);
                        bool isConnected = await messenger.ConnectAsync(server.Address.Split(':')[0], int.Parse(server.Address.Split(':')[1]));
                        bool authenticated = await messenger.AuthenticateAsync(server.RconPassword);
                        if (authenticated)
                        {
                            await messenger.ExecuteCommandAsync($"changelevel {match.Map.Name}");
                        }
                        messenger.CloseConnection();
                    }
                    catch
                    {
                        await _discordNotificationService.SendAuditChannelMessage($"Couldn't set up map for tournament match: {match.Id} ({match.TeamHome.Name} vs {match.TeamAway.Name}) [KO: {((DateTime)match.KickOff).ToString("g")}]");
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(10));

                try
                {
                    INetworkSocket socket = new RconSocket();
                    RconMessenger messenger = new RconMessenger(socket);
                    bool isConnected = await messenger.ConnectAsync(server.Address.Split(':')[0], int.Parse(server.Address.Split(':')[1]));
                    bool authenticated = await messenger.AuthenticateAsync(server.RconPassword);
                    if (authenticated)
                    {
                        await messenger.ExecuteCommandAsync($"exec {server.Region.MatchFormat}v{server.Region.MatchFormat}.cfg");
                        await messenger.ExecuteCommandAsync("exec league.cfg");
                        await messenger.ExecuteCommandAsync("sv_singlekeeper 0");
                        await messenger.ExecuteCommandAsync("mp_matchinfo \"Tournament Match\"");
                        await messenger.ExecuteCommandAsync("sv_webserver_matchdata_url \"" + _config.WebServerConfig.HubApiUrl + "/api/match-statistics" + "\"");
                        await messenger.ExecuteCommandAsync("sv_webserver_matchdata_enabled 1");
                        await messenger.ExecuteCommandAsync($"mp_teamnames \"{match.TeamHome.TeamCode}: {match.TeamHome.Name}, {match.TeamAway.TeamCode}: {match.TeamAway.Name}\"");
                        await messenger.ExecuteCommandAsync($"sv_webserver_matchdata_accesstoken " + GenerateMatchDataAuthToken(server, match.Id, match.TeamHome.TeamCode, match.TeamAway.TeamCode));
                        await messenger.ExecuteCommandAsync("say Have a great game, and remember what I taught you in training - Coach");
                        var homeKitId = await GetKitForTeam(messenger, match.TeamHome.Name);
                        var awayKitId = await GetKitForTeam(messenger, match.TeamAway.Name);
                        if (homeKitId > 0 || awayKitId > 0)
                        {
                            await messenger.ExecuteCommandAsync($"mp_teamkits {homeKitId ?? 1} {awayKitId ?? 1}");
                        }
                    }
                    messenger.CloseConnection();
                }
                catch
                {
                    await _discordNotificationService.SendAuditChannelMessage($"Couldn't set up tournament match: {match.Id} ({match.TeamHome.Name} vs {match.TeamAway.Name}) [KO: {((DateTime)match.KickOff).ToString("g")}]");
                }
            }

            var homeChannel = _channelService.GetChannelBySearchTeamCode(match.TeamHome.TeamCode, MatchFormat.EightVsEight) ?? _channelService.GetChannelByTeamCode(match.TeamHome.TeamCode);
            var awayChannel = _channelService.GetChannelBySearchTeamCode(match.TeamAway.TeamCode, MatchFormat.EightVsEight) ?? _channelService.GetChannelByTeamCode(match.TeamAway.TeamCode);
            await SendTournamentReadyMessage(homeChannel);
            await SendTournamentReadyMessage(awayChannel);

            if (_discordClient.GetChannel(_config.DiscordConfig.ResultStreamChannelId) is SocketTextChannel resultsStreamChannel)
            {
                await resultsStreamChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateSimpleEmbed($"**{match.TeamHome.Name}** {match.TeamHome.BadgeEmote} vs {match.TeamAway.BadgeEmote} **{match.TeamAway.Name}** - 15 minutes until kick off!", $"**{match.Tournament.Name}:**"));
            }

            async Task SendTournamentReadyMessage(Channel channel)
            {
                if (channel != null)
                {
                    var discordChannel = _discordClient.GetChannel(channel.DiscordChannelId) as SocketTextChannel;
                    await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateSimpleEmbed($"15 minutes to go! Join the server now for your tournament match - **{match.Server.Name}** steam://connect/{match.Server.Address}", $"**{match.TeamHome.Name}** {match.TeamHome.BadgeEmote} vs {match.TeamAway.BadgeEmote} **{match.TeamAway.Name}**"));
                }
            }
        }

        public async Task RestartServer(int serverId)
        {
            var server = _serverService.GetServer(serverId);

            if (!string.IsNullOrEmpty(server.RconPassword) && ServerAddressHelper.IsValidAddress(server.Address))
            {
                try
                {
                    INetworkSocket socket = new RconSocket();
                    RconMessenger messenger = new RconMessenger(socket);
                    bool isConnected = await messenger.ConnectAsync(server.Address.Split(':')[0], int.Parse(server.Address.Split(':')[1]));
                    bool authenticated = await messenger.AuthenticateAsync(server.RconPassword);
                    if (authenticated)
                    {
                        await messenger.ExecuteCommandAsync("say [Coach] Server is automatically restarting to prepare for tournament match..");
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        await messenger.ExecuteCommandAsync("quit");
                    }
                    messenger.CloseConnection();
                }
                catch
                {

                }
            }
        }

        public async void ExecConfigAsync(int serverListItemId, ulong channelId, IUser user)
        {
            var server = GetServerFromServerListItemId(serverListItemId, channelId);
            var channel = _channelService.GetChannelByDiscordId(channelId);
            var discordChannel = _discordClient.GetChannel(channelId) as SocketTextChannel;
            var matchFormat = channel.ChannelPositions.Count + "v" + channel.ChannelPositions.Count;

            if (IsServerInUse(server.Id))
            {
                await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed($"Cannot execute command as the selected server seems to be in use.", ServiceResponseStatus.Failure));
                return;
            }

            if (!string.IsNullOrEmpty(server.RconPassword) && ServerAddressHelper.IsValidAddress(server.Address))
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
                        await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateSimpleEmbed($":stadium: The match has successfully been automatically set up on **{server.Name}**"));
                    }
                    messenger.CloseConnection();
                }
                catch
                {
                    await discordChannel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed($"The server config could not be executed.", ServiceResponseStatus.Failure));
                }
            }
        }

        public async void SendServerMessage(int serverId, string message)
        {
            var server = _serverService.GetServer(serverId);

            if (!string.IsNullOrEmpty(server.RconPassword) && ServerAddressHelper.IsValidAddress(server.Address))
            {
                try
                {
                    INetworkSocket socket = new RconSocket();
                    RconMessenger messenger = new RconMessenger(socket);
                    bool isConnected = await messenger.ConnectAsync(server.Address.Split(':')[0], int.Parse(server.Address.Split(':')[1]));
                    bool authenticated = await messenger.AuthenticateAsync(server.RconPassword);
                    if (authenticated)
                    {
                        await messenger.ExecuteCommandAsync($"say Coach says: " + message);
                    }
                    messenger.CloseConnection();
                }
                catch
                {

                }
            }
        }

        public Server GetServerFromServerListItemId(int serverListItemId, ulong channelId)
        {
            var channel = _channelService.GetChannelByDiscordId(channelId, false);
            var servers = _serverService.GetServersByRegion((int)channel.Team.RegionId).Where(s => s.DeactivatedDate == null).ToList();

            return servers[serverListItemId - 1];
        }

        private string GenerateMatchDataAuthToken(Server server, int matchId, string homeTeamCode, string awayTeamCode)
        {
            var token = $"{server.Address}_{matchId}_{homeTeamCode}_{awayTeamCode}";
            var encodedToken = Encoding.UTF8.GetBytes(token);

            return Convert.ToBase64String(encodedToken);
        }

        private async Task<int?> GetKitForTeam(RconMessenger rconMessenger, string teamName)
        {
            var kitsCsv = await rconMessenger.ExecuteCommandAsync($"mp_teamkits_csv {teamName.ToLower().Replace(' ', '_')}");
            var kits = kitsCsv.Split(',').Where(k => k.Contains(':') && k.Contains('_'));

            if (!kits.Any())
            {
                return null;
            }

            return kits.Select(k => Int32.Parse(k.Split(':').FirstOrDefault())).FirstOrDefault();
        }
    }
}