using CoachBot.Bot;
using CoachBot.Database;
using CoachBot.Domain.Services;
using CoachBot.LegacyImporter;
using CoachBot.Model;
using CoachBot.Services;
using CoachBot.Services.Logging;
using CoachBot.Services.Matchmaker;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Extensions.Logging;
using System.IO;

namespace CoachBot
{
    public class WebStartup
    {
        public IConfiguration Configuration { get; }
        private DiscordSocketClient _client;

        public WebStartup(IConfiguration configuration)
        {
            Configuration = configuration;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));
            var logger = LogAdaptor.CreateLogger();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new SerilogLoggerProvider(logger));

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            });

            services.AddSingleton<ConfigService>()
                .AddSingleton(_client)
                .AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false, ThrowOnError = false }))
                .AddSingleton(logger)
                .AddSingleton(config)
                .AddSingleton<LogAdaptor>()
                .AddSingleton<ConfigService>()
                .AddTransient<RegionService>()
                .AddTransient<ServerService>()
                .AddTransient<ChannelService>()
                .AddTransient<SubstitutionService>()
                .AddTransient<MatchmakingService>()
                .AddTransient<ServerManagementService>()
                .AddTransient<MatchService>()
                .AddTransient<SearchService>()
                .AddTransient<PlayerService>()
                .AddTransient<DiscordNotificationService>()
                .AddTransient<BotService>()
                .AddSingleton<CacheService>()
                .AddSingleton<BotInstance>()
                .AddSingleton<Importer>()
                .AddDbContext<CoachBotContext>(ServiceLifetime.Transient);

            var provider = services.BuildServiceProvider();

            provider.GetService<LogAdaptor>();
            provider.GetService<ConfigService>();
            provider.GetService<BotInstance>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}