﻿using CoachBot.Model;
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
using Serilog.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using Newtonsoft.Json;
using System.IO;
using CoachBot.Database;
using CoachBot.Domain.Services;
using CoachBot.Domain.Repositories;
using CoachBot.Bot;
using CoachBot.Extensions;
using CoachBot.Services;
using AspNet.Security.OpenId.Steam;
using Microsoft.AspNetCore.Authorization;

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
                .AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false, ThrowOnError = false }))
                .AddSingleton(logger)
                .AddSingleton(config)
                .AddSingleton<LogAdaptor>()
                .AddSingleton<ConfigService>()
                .AddTransient<BotService>()
                .AddTransient<RegionService>()
                .AddTransient<CountryService>()
                .AddTransient<ServerRepository>()
                .AddTransient<ServerService>()
                .AddTransient<ChannelService>()
                .AddTransient<GuildService>()
                .AddTransient<MatchmakingService>()
                .AddTransient<ServerManagementService>()
                .AddTransient<MatchService>()
                .AddTransient<SearchService>()
                .AddTransient<PlayerService>()
                .AddTransient<PlayerTeamService>()
                .AddTransient<PositionService>()
                .AddTransient<TeamService>()
                .AddTransient<SubstitutionService>()
                .AddTransient<TournamentService>()
                .AddTransient<MatchStatisticsService>()
                .AddTransient<FantasyService>()
                .AddTransient<ScorePredictionService>()
                .AddTransient<PlayerProfileService>()
                .AddTransient<DiscordNotificationService>()
                .AddTransient<DiscordService>()
                .AddSingleton<CacheService>()
                .AddTransient<AssetImageService>()
                .AddSingleton<BotInstance>()
                .AddDbContext<CoachBotContext>(ServiceLifetime.Transient);

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

            /*services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(SteamAuthenticationDefaults.AuthenticationScheme)
                    .AddAuthenticationSchemes(SteamAuthenticationDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });*/
            var provider = services.BuildServiceProvider();

            provider.GetService<LogAdaptor>();
            provider.GetService<ConfigService>();
            provider.GetService<BotInstance>();

            //provider.GetService<CoachBotContext>().Initialize();
            //provider.GetService<CoachBotContext>().SeedPreReleaseData();
            //provider.GetService<CoachBotContext>().SeedMatchData();
            //provider.GetService<CoachBotContext>().SeedTeams();
            //provider.GetService<MatchStatisticsService>().GenerateStatistics();

            provider.GetService<MatchStatisticsService>().GenerateTeamForm();
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