using CoachBot.Model;
using CoachBot.Shared.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
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
            var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development;
            var host = Host.CreateDefaultBuilder()
              .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.UseStartup<WebStartup>();
                  webBuilder.ConfigureKestrel(options =>
                  {
                      if (!isDevelopment)
                      {
                          options.ConfigureHttpsDefaults(listenOptions =>
                          {
                              listenOptions.ServerCertificate = new X509Certificate2(Path.Combine(config.WebServerConfig.SecurityCertFile, config.WebServerConfig.SecurityCertPassword));
                          });
                      }

                      options.Listen(IPAddress.Any, httpPort);
                      options.Listen(IPAddress.Any, httpsPort, listenOptions => listenOptions.UseHttps());
                  });
              })
            .Build();

            host.Start();

            await Task.Delay(Timeout.Infinite);
        }

        /*private static void Main(string[] args) =>
            new Program().RunAsync().GetAwaiter().GetResult();

        private async Task RunAsync()
        {
            var config = ConfigHelper.GetConfig();
            var httpPort = config.WebServerConfig.BotApiPort > 0 ? config.WebServerConfig.BotApiPort : 8080;
            var httpsPort = config.WebServerConfig.SecureBotApiPort > 0 ? config.WebServerConfig.SecureBotApiPort : 44381;
            var host = Host.CreateDefaultBuilder()
              .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.UseStartup<WebStartup>();
                  webBuilder.ConfigureKestrel(options =>
                  {
                      options.Listen(IPAddress.Any, httpPort);
                      options.Listen(IPAddress.Any, httpsPort, listenOptions =>
                      {
                          listenOptions.UseHttps(config.WebServerConfig.SecurityCertFile, config.WebServerConfig.SecurityCertPassword);
                      });
                  });
              })
            .Build();

            host.Start();

            await Task.Delay(Timeout.Infinite);
        }*/
    }
}