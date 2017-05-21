using System.Collections.Generic;
using CoachBot.Model;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System;

namespace CoachBot.Services.Matchmaker
{
    public class ConfigService
    {
        public Config config;

        public ConfigService()
        {
            config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));
            if (config.Servers == null) config.Servers = new List<Server>();
        }

        internal void Save()
        {
            File.WriteAllText(@"config.json", JsonConvert.SerializeObject(config));
        }

        public string AddServer(Server server)
        {
            config.Servers.Add(server);
            Save();
            return $"{server.Name} added to the server list";
        }

        public string RemoveServer(Server server)
        {
            config.Servers.Remove(server);
            Save();
            return $"{server.Name} removed from the server list";
        }

        public string ReadServerList()
        {
            var sb = new StringBuilder();
            var serverId = 1;

            foreach(var server in config.Servers)
            {
                sb.Append($"```#{serverId} {server.Name} {server.Address}``` steam://connect/{server.Address}");
                sb.Append(Environment.NewLine);
                serverId++;
            }
            return sb.ToString();
        }

    }
}
