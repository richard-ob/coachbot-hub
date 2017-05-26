using System.Collections.Generic;
using CoachBot.Model;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System;
using System.Linq;

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

        public string ReadServerList()
        {
            var sb = new StringBuilder();
            var serverId = 1;
            sb.AppendLine(":desktop: Servers:");
            foreach(var server in Config.Servers)
            {
                sb.AppendLine($"**#{serverId}** {server.Name} - {server.Address} - steam://connect/{server.Address}");
                serverId++;
            }
            return sb.ToString();
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
