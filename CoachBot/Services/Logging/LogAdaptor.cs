using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace CoachBot.Services.Logging
{
    public class LogAdaptor
    {
        private readonly ILogger _logger;

        public static Logger CreateLogger()
        {
           return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.RollingFile("log-{Date}.txt")
                .CreateLogger();
        }

        public LogAdaptor(Logger logger, DiscordSocketClient client)
        {
            _logger = logger.ForContext<DiscordSocketClient>();
            client.Log += LogAsync;
        }

        private Task LogAsync(LogMessage message)
        {
            if (message.Exception == null)
                _logger.Write(
                    GetEventLevel(message.Severity),
                    $"{message.Message}");
            else
                _logger.Write(
                        GetEventLevel(message.Severity),
                        message.Exception,
                        $"{message.Message}"
                    );

            return Task.CompletedTask;
        }

        internal async Task LogCommand(LogMessage message)
        {
            if (message.Exception != null && message.Exception is CommandException cmd)
            {
                Console.WriteLine(cmd.ToString());
            }
            await LogAsync(message);
        }

        private static LogEventLevel GetEventLevel(LogSeverity severity)
        {
            return (LogEventLevel) Math.Abs((int) (severity - 5));
        }
    }
}
