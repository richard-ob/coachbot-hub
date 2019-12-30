using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using CoachBot.Services.Matchmaker;
using CoachBot.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Newtonsoft.Json;
using System.IO;
using CoachBot.Bot;

namespace CoachBot
{
    internal class Program
    {
        private static void Main(string[] args) =>
            new Program().RunAsync().GetAwaiter().GetResult();

        private async Task RunAsync()
        {
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));
            var host = WebHost
              .CreateDefaultBuilder()
              .UseKestrel()
              .UseStartup<WebStartup>()
              .UseUrls($"http://*:{(config.ApiPort > 0 ? config.ApiPort.ToString() : "80")}")
              .Build();
            host.Start();
            
            while (true) { }
        }
    }        
}