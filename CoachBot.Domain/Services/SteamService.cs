using CoachBot.Shared.Model;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoachBot.Domain.Services
{
    public class SteamService
    {
        private readonly Config _config;

        public SteamService(Config config) {
            _config = config;
        }

        public async Task<string> GetSteamName(ulong steamId)
        {
            string steamName = null;
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync($"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={_config.SteamApiToken}&steamids={steamId}");
                var responseParts = response.Split("\"personaname\":\"");
                steamName = responseParts[1].Substring(0, responseParts[1].IndexOf("\","));
            }

            return steamName;
        }
    }
}