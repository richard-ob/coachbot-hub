using CoachBot.Model;
using Newtonsoft.Json;
using System.IO;
using System;

namespace CoachBot.Services
{
    public class ConfigService
    {
        public Config Config;

        public ConfigService()
        {
            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));
            if (string.IsNullOrEmpty(Config.BotToken)) throw new Exception("No valid bot token provided");
        }
    }
}
