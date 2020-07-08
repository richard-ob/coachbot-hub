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
              .UseStartup<WebStartup>()
              .UseUrls($"http://*:{ApiPort}")
              .Build()
              .Run();

        public static int ApiPort {
            get {
                var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config-dev.json"));
                return config.ApiPort > 0 ? config.ApiPort : 80;
            }
        }
    }
}