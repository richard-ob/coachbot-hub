using CoachBot.Services.Logging;
using CoachBot.Services.Matchmaker;
using Discord;
using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;

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
            // Configure logging
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
            services.AddMvc();
            services.AddSingleton<ConfigService>();
            services.AddSingleton(_client)
                .AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false, ThrowOnError = false }))
                .AddSingleton(logger)
                .AddSingleton<LogAdaptor>()
                .AddSingleton<InteractiveService>()
                .AddSingleton<ConfigService>()
                .AddSingleton<MatchmakerService>()
                .AddSingleton<StatisticsService>()
                .AddSingleton<ChatService>();
                
            var provider = services.BuildServiceProvider();

            // Autowire and create these dependencies now
            provider.GetService<LogAdaptor>();
            provider.GetService<ConfigService>();

            //return provider;

        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAll");
            app.UseMvc();

        }
    }
}