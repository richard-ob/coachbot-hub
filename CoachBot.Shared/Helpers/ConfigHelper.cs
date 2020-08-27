using CoachBot.Shared.Model;
using Newtonsoft.Json;
using System.IO;

namespace CoachBot.Shared.Helpers
{
    public static class ConfigHelper
    {
        public static Config GetConfig()
        {
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config-dev.json"));
        }
    }
}
