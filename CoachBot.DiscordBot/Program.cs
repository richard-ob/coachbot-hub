using CoachBot.Model;
using CoachBot.Shared.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CoachBot
{
    internal class Program
    {
        private static void Main(string[] args) =>
            new Program().RunAsync().GetAwaiter().GetResult();

        private async Task RunAsync()
        {
            var config = ConfigHelper.GetConfig();
            var port = config.WebServerConfig.BotApiPort > 0 ? config.WebServerConfig.BotApiPort : 8080;
            var host = WebHost
              .CreateDefaultBuilder()
              .UseKestrel()
              .UseStartup<WebStartup>()
              .UseUrls($"http://*:{port}")
              .Build();
            host.Start();

            await Task.Delay(Timeout.Infinite);
        }
    }
}