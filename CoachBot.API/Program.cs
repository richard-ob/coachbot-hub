﻿using CoachBot.Shared.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace CoachBot
{
    public class Program
    {
        private static void Main(string[] args) =>
            new Program().RunAsync().GetAwaiter().GetResult();

        private async Task RunAsync()
        {
            var config = ConfigHelper.GetConfig();
            var port = config.BotApiPort > 0 ? config.ApiPort : 8080;
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