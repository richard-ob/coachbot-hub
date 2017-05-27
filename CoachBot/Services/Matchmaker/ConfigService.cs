using System.Collections.Generic;
using CoachBot.Model;
using Newtonsoft.Json;
using System.IO;
using System.Text;
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
        }

        internal void Save()
        {
            File.WriteAllText(@"config.json", JsonConvert.SerializeObject(Config));
        }

        public string AddServer(Server server)
        {
            Config.Servers.Add(server);
            Save();
            return $"{server.Name} added to the server list";
        }

        public string RemoveServer(Server server)
        {
            Config.Servers.Remove(server);
            Save();
            return $"{server.Name} removed from the server list";
        }

        public Embed ReadServerList()
        {
            var serverId = 1;
            var embedBuilder = new EmbedBuilder().WithTitle(":desktop: Servers");
            foreach(var server in Config.Servers)
            {
                embedBuilder.AddField($"#{serverId} {server.Name}", $"steam://connect/{server.Address}");
                serverId++;
            }
            return embedBuilder.Build();
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
            embedBuilder.AddField("!vs <team>", "Set the opposition team for the current match");
            embedBuilder.AddField("!vsmix", "Set the opposition team to a managed mix for the current match");
            embedBuilder.AddField("!ready", "Send all players to server");
            embedBuilder.AddField("!ready <server id>", "Send all players to the server provided");
            embedBuilder.AddField("!reset", "Manually reset the match");
            embedBuilder.AddField("!servers", "See the full available server list");
            embedBuilder.AddField("!addserver <ip:port> <name>", "Add a server to the server list");
            embedBuilder.AddField("!removeserver <ip:port> <name>", "Remove specified server to the server list");
            embedBuilder.AddField("!recentmatches", "See a list of recent matches played");
            embedBuilder.AddField("!leaderboard", "See the appearance rankings for this channel");
            embedBuilder.AddField("!configure <team name> <positions> (e.g. !configure BB GK RB CB LB RW CM LW CF)", "Configure the current channel's matchmaking settings with default settings");
            embedBuilder.AddField("!configure <team name> <is a mix channel> <use formation on team sheet> <use classic team sheet view> <positions> (e.g. !configure BB false true false GK RB CB LB RW CM LW CF)", "Configure the current channel's matchmaking settings");
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
