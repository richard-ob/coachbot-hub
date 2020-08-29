﻿using AspNetCore.Proxy;
using CoachBot.Database;
using CoachBot.Domain.Services;
using CoachBot.Shared.Extensions;
using CoachBot.Model;
using CoachBot.Services;
using CoachBot.Shared.Services.Logging;
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
using CoachBot.Shared.Services;
using CoachBot.Shared.Helpers;
using CoachBot.Extensions;
using Serilog;

namespace CoachBot
{
    public class WebStartup
    {
        public IConfiguration Configuration { get; }
        private DiscordSocketClient _client;
        private ILoggerProvider _loggerProvider;

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
            var config = ConfigHelper.GetConfig();
            var logger = LogAdaptor.CreateLogger();
            var loggerFactory = new LoggerFactory();
            _loggerProvider = new SerilogLoggerProvider(logger);
            loggerFactory.AddProvider(_loggerProvider);
            
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

            services.AddSingleton(_client)
                .AddSingleton(logger)
                .AddSingleton(config)
                .AddSingleton<LogAdaptor>()
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
                .AddTransient<MapService>()
                .AddTransient<DiscordNotificationService>()
                .AddSingleton<DiscordService>()
                .AddSingleton<CacheService>()
                .AddTransient<AssetImageService>()
                .AddTransient<SteamService>()
                .AddTransient<AnnouncementService>()
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
                    x.AppId = config.DiscordConfig.OAuth2Id;
                    x.AppSecret = config.DiscordConfig.OAuth2Secret;
                });

            var provider = services.BuildServiceProvider();

            provider.GetService<LogAdaptor>();

            var discordService = provider.GetService<DiscordService>();
            discordService.StartPersistentConnection();

            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseMvc();

            loggerFactory.AddProvider(_loggerProvider);
        }
    }
}