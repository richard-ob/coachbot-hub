using CoachBot.Model;
using CoachBot.Shared.Helpers;
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
                var config = ConfigHelper.GetConfig();
                return config.ApiPort > 0 ? config.ApiPort : 80;
            }
        }
    }
}