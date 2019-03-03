using CoachBot.Domain.Repositories;
using CoachBot.Model;
using Discord;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class ServerService
    {
        private readonly ServerRepository _serverRepository;

        public ServerService(ServerRepository serverRepository)
        {
            _serverRepository = serverRepository;
        }

        public List<Server> GetServers()
        {
            return _serverRepository.GetAll();
        }

        public List<Server> GetServersByRegion(int regionId)
        {
            return _serverRepository.GetAll().Where(s => s.RegionId == regionId).ToList();
        }

        public Server GetServer(int id)
        {
            return _serverRepository.Get(id);
        }

        public void AddServer(Server server)
        {
            _serverRepository.Add(server);
        }

        public void UpdateServer(Server server)
        {
            _serverRepository.Update(server);
        }

        public void RemoveServer(int id)
        {
            _serverRepository.Delete(id);
        }

        public Embed GenerateServerListEmbed(int regionId)
        {
            var serverId = 1;
            var embedBuilder = new EmbedBuilder().WithTitle(":desktop: Servers");
            var servers = GetServersByRegion(regionId);

            foreach (var server in servers)
            {
                var autoSetup = !string.IsNullOrEmpty(server.RconPassword) ? "**[Auto Setup]**" : "";
                embedBuilder.AddField($"#{serverId} {server.Name} {autoSetup}", $"steam://connect/{server.Address}");
                serverId++;
            }

            return embedBuilder.Build();
        }
    }
}
