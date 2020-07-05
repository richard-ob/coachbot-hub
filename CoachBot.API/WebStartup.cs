using AspNetCore.Proxy;
using CoachBot.Database;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Model;
using CoachBot.Services;
using CoachBot.Services.Logging;
using CoachBot.Services.Matchmaker;
using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Extensions.Logging;
using System;
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

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
            });

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                    //options.SerializerSettings.DateFormatString = "yyyy'-'dd'-'MM'T'HH':'mm':'ssZ";
                    options.SerializerSettings.Converters.Add(new UlongToStringConverter());
                    options.SerializerSettings.Converters.Add(new UlongNullableToStringConverter());
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddSingleton<ConfigService>()
                .AddSingleton(_client)
                .AddSingleton(logger)
                .AddSingleton(config)
                .AddSingleton<LogAdaptor>()
                .AddSingleton<ConfigService>()
                .AddTransient<BotService>()
                .AddTransient<RegionService>()
                .AddTransient<CountryService>()
                .AddTransient<ServerService>()
                .AddTransient<ChannelService>()
                .AddTransient<GuildService>()
                .AddTransient<MatchService>()
                .AddTransient<SearchService>()
                .AddTransient<PlayerService>()
                .AddTransient<PlayerTeamService>()
                .AddTransient<PositionService>()
                .AddTransient<TeamService>()
                .AddTransient<TournamentService>()
                .AddTransient<MatchStatisticsService>()
                .AddTransient<FantasyService>()
                .AddTransient<ScorePredictionService>()
                .AddTransient<PlayerProfileService>()
                .AddTransient<DiscordNotificationService>()
                .AddSingleton<DiscordService>()
                .AddSingleton<CacheService>()
                .AddTransient<AssetImageService>()
                .AddDbContext<CoachBotContext>(ServiceLifetime.Transient);

            services.AddProxies();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = new TimeSpan(7, 0, 0, 0);
                    options.LoginPath = "/unauthorized";
                })
                .AddSteam()
                .AddDiscord(x =>
                {
                    x.AppId = config.OAuth2Id;
                    x.AppSecret = config.OAuth2Secret;
                });

            var provider = services.BuildServiceProvider();

            provider.GetService<LogAdaptor>();
            provider.GetService<ConfigService>();

            var discordService = provider.GetService<DiscordService>();
            discordService.StartPersistentConnection();

            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}