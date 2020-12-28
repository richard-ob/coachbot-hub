using CoachBot.Bot;
using CoachBot.Database;
using CoachBot.Domain.Services;
using CoachBot.Model;
using CoachBot.Services;
using CoachBot.Shared.Services.Logging;
using CoachBot.Services.Matchmaker;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog.Extensions.Logging;
using CoachBot.Shared.Helpers;
using CoachBot.Shared.Services;
using Discord.Rest;
using Microsoft.AspNetCore.Hosting;
using System;

namespace CoachBot
{
    public class WebStartup
    {
        public IConfiguration Configuration { get; }
        private DiscordSocketClient _client;
        private DiscordRestClient _restClient;

        public WebStartup(IConfiguration configuration)
        {
            Configuration = configuration;                
            _client = new DiscordSocketClient(GetDiscordSocketConfig());
            _restClient = new DiscordRestClient(new DiscordRestConfig() {
                LogLevel = LogSeverity.Debug
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var config = ConfigHelper.GetConfig();
            var logger = LogAdaptor.CreateLogger();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new SerilogLoggerProvider(logger));

            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            });

            services
                .AddSingleton(_client)
                .AddSingleton(_restClient)
                .AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false, ThrowOnError = false }))
                .AddSingleton(logger)
                .AddSingleton(config)
                .AddSingleton<LogAdaptor>()
                .AddTransient<RegionService>()
                .AddTransient<ServerService>()
                .AddTransient<ChannelService>()
                .AddTransient<SubstitutionService>()
                .AddTransient<MatchmakingService>()
                .AddTransient<ServerManagementService>()
                .AddTransient<SearchService>()
                .AddTransient<PlayerService>()
                .AddTransient<MatchService>()
                .AddTransient<MatchupService>()
                .AddTransient<DiscordNotificationService>()
                .AddTransient<SteamService>()
                .AddTransient<AssetImageService>()
                .AddSingleton<BotService>()
                .AddSingleton<CacheService>()
                .AddSingleton<BotInstance>()
                .AddDbContext<CoachBotContext>(ServiceLifetime.Transient);

            services.AddHostedService<TimedHostedService>();

            var provider = services.BuildServiceProvider();
            provider.GetService<CoachBotContext>().Initialize();
            provider.GetService<LogAdaptor>();

            var bot = provider.GetService<BotInstance>();
            bot.Startup();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
        }

        private DiscordSocketConfig GetDiscordSocketConfig()
        {
            var discordSocketConfig = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            };

            discordSocketConfig.GatewayIntents = GatewayIntents.DirectMessageReactions |
                GatewayIntents.DirectMessages |
                GatewayIntents.DirectMessageTyping |
                GatewayIntents.GuildEmojis |
                GatewayIntents.GuildIntegrations |
                GatewayIntents.GuildMembers |
                GatewayIntents.GuildMessageReactions |
                GatewayIntents.GuildMessages |
                GatewayIntents.GuildMessageTyping |
                GatewayIntents.GuildPresences |
                GatewayIntents.Guilds;

            return discordSocketConfig;
        }
    }
}