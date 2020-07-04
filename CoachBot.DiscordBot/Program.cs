using System.Threading.Tasks;
using CoachBot.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace CoachBot
{
    internal class Program
    {
        private static void Main(string[] args) =>
            new Program().RunAsync().GetAwaiter().GetResult();

        private async Task RunAsync()
        {
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));
            var port = config.ApiPort > 0 ? config.ApiPort : 8080;
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