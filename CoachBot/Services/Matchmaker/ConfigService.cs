using System.Collections.Generic;
using CoachBot.Model;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;
using Discord;

namespace CoachBot.Services.Matchmaker
{
    public class ConfigService
    {
        public Config Config;

        public ConfigService()
        {
            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));
            if (string.IsNullOrEmpty(Config.BotToken)) throw new Exception("No valid bot token provided");
            if (Config.Servers == null) Config.Servers = new List<Server>();
            if (Config.Channels == null) Config.Channels = new List<Channel>();
            if (Config.Regions == null) Config.Regions = new List<Region>();
        }

        internal void Save()
        {
            File.WriteAllText(@"config.json", JsonConvert.SerializeObject(Config));
        }

        public void UpdateBotToken(string botToken)
        {
            Config.BotToken = botToken;
            Save();
        }

        public void UpdateOAuthTokens(string appId, string appSecret)
        {
            Config.OAuth2Id = appId;
            Config.OAuth2Secret = appSecret;
            Save();
        }

        public List<Server> GetServers()
        {
            var servers = Config.Servers;
            servers.ForEach(s => s.Region = Config.Regions.FirstOrDefault(r => r.RegionId == s.RegionId));
            return servers;
        }

        public string AddServer(Server server)
        {
            Config.Servers.Add(server);
            Save();
            return $"{server.Name} added to the server list";
        }

        public string RemoveServer(int id)
        {
            if (id > Config.Servers.Count)
            {
                return $"Server #{id} is not a valid server id";
            }
            var server = Config.Servers[id - 1];
            Config.Servers.Remove(server);
            Save();
            return $"{server.Name} removed from the server list";
        }

        public Embed ReadServerList(ulong channelId)
        {
            var serverId = 1;
            var channelRegionId = Config.Channels.First(c => c.Id == channelId).RegionId;
            var embedBuilder = new EmbedBuilder().WithTitle(":desktop: Servers");
            foreach (var server in Config.Servers.Where(s => s.RegionId == channelRegionId))
            {
                embedBuilder.AddField($"#{serverId} {server.Name}", $"steam://connect/{server.Address}");
                serverId++;
            }
            if (!Config.Servers.Any(s => s.RegionId == channelRegionId))
            {
                embedBuilder.AddField("Oh dear..", "No servers available for this region. Please message an IOSoccer administrator.");
            }
            return embedBuilder.Build();
        }

        public List<Region> GetRegions()
        {
            var regions = Config.Regions;
            regions.ForEach(r => r.ServerCount = Config.Servers.Count(s => s.RegionId == r.RegionId));
            return regions;
        }

        public void AddRegion(Region region)
        {
            region.RegionId = 1;
            if (Config.Regions.Count() > 0) region.RegionId = Config.Regions.Max(r => r.RegionId) + 1;
            Config.Regions.Add(region);
            Save();
        }

        public void RemoveRegion(int id)
        {
            if (!Config.Servers.Any(s => s.RegionId == id))
            {
                var region = Config.Regions.FirstOrDefault(r => r.RegionId == id);
                Config.Regions.Remove(region);
                Save();
            }
        }

        public Embed ListCommands()
        {
            var embedBuilder = new EmbedBuilder().WithTitle(":keyboard: Commands");
            embedBuilder.AddField("!sign", "Sign yourself in the first available position");
            embedBuilder.AddField("!sign <position>", "Sign yourself in the specified position");
            embedBuilder.AddField("!sign <position> <name>", "Sign on behalf of someone not in Discord");
            embedBuilder.AddField("!sign2", "Sign yourself in the first available position to Team 2");
            embedBuilder.AddField("!sign2 <position>", "Sign in specified position to Team 2");
            embedBuilder.AddField("!sign2 <position> <name>", "Sign on behalf of someone not in Discord to Team 2");
            embedBuilder.AddField("!unsign", "Unsign from the match");
            embedBuilder.AddField("!unsign <name>", "Unsign the person specified from the match");
            embedBuilder.AddField("!sub", "Sign yourself as a sub. You will be allocated to next first available outfield position");
            embedBuilder.AddField("!unsub", "Unsign yourself from the subs bench");
            embedBuilder.AddField("!unsub <name>", "Unsign the person specified from the subs bench");
            embedBuilder.AddField("!requestsub <server id> <position>", "Highlights everyone in the channel and requests a sub to join the server");
            embedBuilder.AddField("!vs <team>", "Set the opposition team for the current match");
            embedBuilder.AddField("!vsmix", "Set the opposition team to a managed mix for the current match");
            embedBuilder.AddField("!ready", "Send all players to server");
            embedBuilder.AddField("!ready <server id>", "Send all players to the server provided");
            embedBuilder.AddField("!reset", "Manually reset the match");
            embedBuilder.AddField("!search", "Search for an opponent to face");
            embedBuilder.AddField("!stopsearch", "Cancel an opponent search");
            embedBuilder.AddField("!challenge <team id>", "Challenge a specific team who are currently searching for opposition");
            embedBuilder.AddField("!unchallenge", "Cancel a challenge before !ready has been called");
            embedBuilder.AddField("!servers", "See the full available server list");
            embedBuilder.AddField("!recentmatches", "See a list of recent matches played");
            embedBuilder.AddField("!leaderboard", "See the appearance rankings for this channel");
            embedBuilder.WithFooter("If you wish to configure CoachBot for your channel, please visit http://coachbot.iosoccer.com");
            return embedBuilder.Build();
        }

        public Channel ReadChannelConfiguration(ulong channelId)
        {
            return Config.Channels.FirstOrDefault(c => c.Id.Equals(channelId));
        }

        public void UpdateChannelConfiguration(Channel channel)
        {
            var existingChannelConfig = Config.Channels.FirstOrDefault(c => c.Id.Equals(channel.Id));
            if (existingChannelConfig != null) Config.Channels.Remove(existingChannelConfig);
            Config.Channels.Add(channel);
            Save();
        }

    }
}
