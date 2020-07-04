using CoachBot.Model;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.IO;

namespace CoachBot
{
    public class Program
    {
        public static void Main(string[] args) =>
          new WebHostBuilder()
              .UseKestrel()
              .UseIISIntegration()
              .UseStartup<WebStartup>()
              .UseUrls($"http://*:{Config.ApiPort}")
              .Build()
              .Run();

        public static Config Config => JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));
    }
}