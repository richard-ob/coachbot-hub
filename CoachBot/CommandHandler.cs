using Discord.Addons.EmojiTools;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using System;
using System.Reflection;
using System.Threading.Tasks;
using CoachBot.Services.Logging;
using CoachBot.Services.Matchmaker;
using System.Linq;

namespace CoachBot
{
    public class CommandHandler
    {
        private readonly IServiceProvider _provider;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly ILogger _logger;
        private readonly ConfigService _configService;
        private readonly MatchmakerService _matchmakerService;

        public CommandHandler(IServiceProvider provider)
        {
            _provider = provider;
            _client = _provider.GetService<DiscordSocketClient>();
            _client.MessageReceived += ProcessCommandAsync;
            _commands = _provider.GetService<CommandService>();
            var log = _provider.GetService<LogAdaptor>();
            _commands.Log += log.LogCommand;
            _logger = _provider.GetService<Logger>().ForContext<CommandService>();
            _configService = _provider.GetService<ConfigService>();
            _matchmakerService = _provider.GetService<MatchmakerService>();            
        }

        public async Task ConfigureAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task ProcessCommandAsync(SocketMessage pMsg)
        {
            var message = pMsg as SocketUserMessage;
            if (message == null) return;
            if (message.Content.StartsWith("##")) return;

            int argPos = 0;
            if (!ParseTriggers(message)) return;

            var context = new SocketCommandContext(_client, message);
            var result = await _commands.ExecuteAsync(context, argPos, _provider);
            var matchmakerChannel = _matchmakerService.Channels.FirstOrDefault(c => c.Id == context.Channel.Id);
            try
            {
                await message.DeleteAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Bot doesn't have have manage channel privileges in {message.Channel.Name} ({context.Guild.Name})");
            }
            if (result is PreconditionResult precondition && !precondition.IsSuccess)
            {
                await message.Channel.SendMessageAsync(precondition.ErrorReason);
                await message.AddReactionAsync(EmojiExtensions.FromText(":no_entry:"));
            }
            else if (result is ParseResult parse && !parse.IsSuccess)
            {
                await message.Channel.SendMessageAsync(parse.ErrorReason);
                await message.AddReactionAsync(EmojiExtensions.FromText(":x:"));                
            }
            else if (result is TypeReaderResult reader && !reader.IsSuccess)
            {
                await message.Channel.SendMessageAsync($"Read Error: {reader.ErrorReason}");
            }
            else if (!result.IsSuccess && result.Error == CommandError.UnknownCommand && matchmakerChannel.Positions.Any(p => context.Message.Content.Substring(1).ToUpper().Contains(p)))
            {
                await message.Channel.SendMessageAsync(_matchmakerService.AddPlayer(message.Channel.Id, message.Author, context.Message.Content.Substring(1))); // Allows wildcard commands for positions
                await message.Channel.SendMessageAsync(_matchmakerService.GenerateTeamList(matchmakerChannel.Id));
            }
            else if (!result.IsSuccess && result.Error == CommandError.UnknownCommand)
            {
                await message.Channel.SendMessageAsync($"Unknown command, {context.Message.Author.Mention}");
            }
            else if (!result.IsSuccess)
            {
                await message.Channel.SendMessageAsync(result.ErrorReason);
                await message.AddReactionAsync(EmojiExtensions.FromText(":rage:"));
            }

            _logger.Debug("Invoked {Command} in {Context} with {Result}", message, context.Channel, result);
        }

        private bool ParseTriggers(SocketUserMessage message)
        {
            bool flag = false;
            if (!message.Author.IsBot) flag = true;
            return flag;
        }
    }
}
