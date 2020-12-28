using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Services;
using CoachBot.Shared.Extensions;
using CoachBot.Shared.Model;
using CoachBot.Shared.Services.Logging;
using CoachBot.Tools;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace CoachBot
{
    public class CommandHandler
    {
        private readonly IServiceProvider _provider;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly ILogger _logger;
        private readonly Config _config;

        public CommandHandler(IServiceProvider provider)
        {
            _provider = provider;
            _client = _provider.GetService<DiscordSocketClient>();
            _client.MessageReceived += ProcessCommandAsync;
            _commands = _provider.GetService<CommandService>();
            _commands.Log += _provider.GetService<LogAdaptor>().LogCommand;
            _logger = _provider.GetService<Logger>().ForContext<CommandService>();
            _config = _provider.GetService<Config>();
        }

        public async Task ConfigureAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        private async Task ProcessCommandAsync(SocketMessage pMsg)
        {
            if (_config.BotConfig.BotStealthMode) return;
            if (!(pMsg is SocketUserMessage message)) return;
            if (!message.Content.StartsWith("!")) return;

            int argPos = 0;
            if (!ParseTriggers(message)) return;

            using (var scope = _provider.CreateScope())
            {
                var channelService = scope.ServiceProvider.GetService<ChannelService>();
                var matchmakingService = scope.ServiceProvider.GetService<MatchmakingService>();
                var context = new SocketCommandContext(_client, message);
                CallContext.SetData(CallContextDataType.DiscordUser, message.Author.Username);

                if (!channelService.ChannelExists(context.Channel.Id))
                {
                    return; // INFO: This can be removed when precondition is re-enabled
                }

                _logger.Debug("Invoked {Command} in {Context}. Result pending.", message, context.Channel);

                var result = await _commands.ExecuteAsync(context, argPos, _provider);

                var logMsg = $"[{message.Channel.Name} ({context.Guild.Name})] {message.Timestamp.ToString()}: @{message.Author.Username} {message.Content}";
                Console.WriteLine(logMsg);
                _logger.Information(logMsg);
                
                if (result is PreconditionResult precondition && !precondition.IsSuccess)
                {
                    await message.Channel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed(precondition.ErrorReason, ServiceResponseStatus.Failure));
                }
                else if (result is ParseResult parse && !parse.IsSuccess)
                {
                    await message.Channel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed(parse.ErrorReason, ServiceResponseStatus.Failure));
                }
                else if (result is TypeReaderResult reader && !reader.IsSuccess)
                {
                    await message.Channel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed($"Read Error: {reader.ErrorReason}", ServiceResponseStatus.Failure));
                }
                else if (!result.IsSuccess && result.Error == CommandError.UnknownCommand && channelService.ChannelHasPosition(context.Channel.Id, context.Message.Content.Substring(1)))
                {
                    await context.Channel.SendMessageAsync("", embed: matchmakingService.AddPlayer(context.Message.Channel.Id, context.Message.Author, context.Message.Content.Substring(1).ToUpper()));
                    foreach (var teamEmbed in matchmakingService.GenerateTeamList(context.Channel.Id))
                    {
                        await context.Channel.SendMessageAsync("", embed: teamEmbed);
                    }
                }
                else if (!result.IsSuccess && result.Error == CommandError.UnknownCommand && channelService.ChannelHasPosition(context.Channel.Id, context.Message.Content.Substring(1), ChannelTeamType.TeamTwo))
                {
                    var positionName = context.Message.Content.Substring(1, context.Message.Content.Length - 2).ToUpper();
                    await context.Channel.SendMessageAsync("", embed: matchmakingService.AddPlayer(context.Message.Channel.Id, context.Message.Author, positionName, ChannelTeamType.TeamTwo));
                    foreach (var teamEmbed in matchmakingService.GenerateTeamList(context.Channel.Id))
                    {
                        await context.Channel.SendMessageAsync("", embed: teamEmbed);
                    }
                }
                else if (!result.IsSuccess && result.Error == CommandError.UnknownCommand)
                {
                    await message.Channel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed($"Unknown command, {context.Message.Author.Mention}", ServiceResponseStatus.Failure));
                }
                else if (!result.IsSuccess)
                {
                    var errorId = DateTime.UtcNow.Ticks.ToString();
                    if (_client.GetChannel(_config.DiscordConfig.AuditChannelId) is ITextChannel auditChannel)
                    {
                        await auditChannel.SendMessageAsync("", embed: new EmbedBuilder().WithTitle($"Error - {message.Channel.Name} [REF:{errorId}]").WithDescription($":exclamation: {result.ErrorReason} ``{context.Message.Author.Username}: {context.Message.Content}``").WithCurrentTimestamp().Build());
                    }

                    await message.Channel.SendMessageAsync("", embed: DiscordEmbedHelper.GenerateEmbed($"An error occurred. Please contact an admin, {context.Message.Author.Mention}. [REF:{errorId}]", ServiceResponseStatus.Failure));
                }

            try
            {
                await message.DeleteAsync();
            }
            catch
            {
                Console.WriteLine($"Bot doesn't have have manage messages privileges in {message.Channel.Name} ({context.Guild.Name})");
            }

            _logger.Debug("Invoked {Command} in {Context} with {Result}", message, context.Channel, result);

            }
        }

        private bool ParseTriggers(SocketUserMessage message)
        {
            bool flag = false;
            if (!message.Author.IsBot) flag = true;
            return flag;
        }
    }
}