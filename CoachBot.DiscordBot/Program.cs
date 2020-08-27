using CoachBot.Model;
using CoachBot.Shared.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.IO;
using System.Net;
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
            var httpPort = config.WebServerConfig.BotApiPort > 0 ? config.WebServerConfig.BotApiPort : 8080;
            var httpsPort = config.WebServerConfig.SecureBotApiPort > 0 ? config.WebServerConfig.SecureBotApiPort : 44381;
            var host = WebHost
              .CreateDefaultBuilder()
              .UseKestrel(options =>
              {
                  options.Listen(IPAddress.Any, httpPort);
                  options.Listen(IPAddress.Any, httpsPort, listenOptions =>
                  {
                      listenOptions.UseHttps(config.WebServerConfig.SecurityCertFile, config.WebServerConfig.SecurityCertPassword);
                  });
              })
              .UseStartup<WebStartup>()
              .Build();
            host.Start();

            await Task.Delay(Timeout.Infinite);
        }
    }
}