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
using CoachBot.Model;
using Discord;

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
            if (!(pMsg is SocketUserMessage message)) return;
            if (!message.Content.StartsWith("!")) return;

            int argPos = 0;
            if (!ParseTriggers(message)) return;

            var context = new SocketCommandContext(_client, message);
            var result = await _commands.ExecuteAsync(context, argPos, _provider);
            var matchmakerChannel = _configService.Config.Channels.FirstOrDefault(c => c.Id == context.Channel.Id);
            var logMsg = $"[{message.Channel.Name} ({context.Guild.Name})] {message.Timestamp.ToString()}: @{message.Author.Username} {message.Content}";
            Console.WriteLine(logMsg);
            _logger.Information(logMsg);
            try
            {
                await message.DeleteAsync();
            }
            catch (Exception)
            {
                Console.WriteLine($"Bot doesn't have have manage messages privileges in {message.Channel.Name} ({context.Guild.Name})");
            }
            if (result is PreconditionResult precondition && !precondition.IsSuccess)
            {
                await message.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(precondition.ErrorReason).WithCurrentTimestamp());
            }
            else if (result is ParseResult parse && !parse.IsSuccess)
            {
                await message.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":no_entry: {parse.ErrorReason}").WithCurrentTimestamp().Build());             
            }
            else if (result is TypeReaderResult reader && !reader.IsSuccess)
            {
                await message.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":no_entry: Read Error: {reader.ErrorReason}").WithCurrentTimestamp().Build());
            }
            else if (!result.IsSuccess && result.Error == CommandError.UnknownCommand 
                     && matchmakerChannel.Positions.Any(p => context.Message.Content.Substring(1).ToUpper().Contains(p.PositionName)))
            {
                await message.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(_matchmakerService.AddPlayer(message.Channel.Id, message.Author, context.Message.Content.Substring(1))).WithCurrentTimestamp().Build()); // Allows wildcard commands for positions
                await message.Channel.SendMessageAsync("", embed: _matchmakerService.GenerateTeamList(matchmakerChannel.Id));
                if (_configService.Config.Channels.First(c => c.Id == message.Channel.Id).Team2.IsMix)
                {
                    await message.Channel.SendMessageAsync("", embed: _matchmakerService.GenerateTeamList(message.Channel.Id, Teams.Team2));
                }
            }
            else if (!result.IsSuccess && result.Error == CommandError.UnknownCommand)
            {
                await message.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":no_entry: Unknown command, {context.Message.Author.Mention}").WithCurrentTimestamp());
            }
            else if (!result.IsSuccess)
            {
                await message.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":no_entry: {result.ErrorReason}").WithCurrentTimestamp().Build());
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
