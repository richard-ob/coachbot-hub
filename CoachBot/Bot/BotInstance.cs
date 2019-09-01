using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoachBot.Bot
{
    public class BotInstance
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _client;
        private readonly ConfigService _configService;
        private readonly MatchmakerService _matchmakerService;
        private CommandHandler _handler;
        private ulong _lastAfkCheckUser = 0;

        public BotInstance(IServiceProvider serviceProvider, DiscordSocketClient client, ConfigService configService, MatchmakerService matchmakerService)
        {
            _serviceProvider = serviceProvider;
            _client = client;
            _configService = configService;
            _matchmakerService = matchmakerService;
            Startup();
        }

        public async void Startup()
        {
            Console.WriteLine("Connecting..");
            await _client.LoginAsync(TokenType.Bot, _configService.Config.BotToken);
            await _client.StartAsync();

            _client.Connected += Connected;
            _client.Ready += BotReady;
            _client.GuildMemberUpdated += UserStatusUpdated;
            _client.ChannelDestroyed += ChannelDestroyed;

            _handler = new CommandHandler(_serviceProvider);
            await _handler.ConfigureAsync();
        }

        private Task Connected()
        {
            Console.WriteLine("Connected!");
            return Task.CompletedTask;
        }

        private Task BotReady()
        {
            Console.WriteLine("Ready!");
            Console.WriteLine("Matchmaking in:");
            foreach (var server in _client.Guilds)
            {
                foreach (var channel in server.Channels)
                {
                    if (_configService.Config.Channels.Any(c => c.Id == channel.Id))
                    {
                        Console.WriteLine($"{channel.Name} on {server.Name}");
                    }
                }
            }
            return Task.CompletedTask;
        }

        private Task ChannelDestroyed(SocketChannel channel)
        {
            var textChannel = channel as SocketTextChannel;
            if (textChannel != null)
            {
                Console.WriteLine($"Channel has been destroyed: {textChannel.Name} on {textChannel.Guild.Name}");
            }
            return Task.CompletedTask;
        }

        private Task UserStatusUpdated(SocketGuildUser userPre, SocketGuildUser userPost)
        {
            if (_lastAfkCheckUser == userPost.Id) return Task.CompletedTask;
            _lastAfkCheckUser = userPost.Id;
            if (userPost.Status.Equals(UserStatus.Online)) return Task.CompletedTask;
            var playerSigned = false;
            foreach (var channel in _configService.Config.Channels)
            {
                var player = channel.SignedPlayers.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                if (player == null) player = channel.Team1.Substitutes.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                if (player != null)
                {
                    playerSigned = true;
                }
            }
            if (!playerSigned) return Task.CompletedTask;
            if (userPre.Status != UserStatus.Offline && userPost.Status == UserStatus.Offline)
            {
                Task.Factory.StartNew(() => UserOffline(userPre, userPost));
            }

            if ((userPre.Status != UserStatus.AFK && userPre.Status != UserStatus.Idle) && (userPost.Status == UserStatus.AFK || userPost.Status == UserStatus.Idle))
            {
                Task.Factory.StartNew(() => UserAway(userPre, userPost));
            }

            return Task.CompletedTask;
        }

        private Task UserOffline(SocketGuildUser userPre, SocketGuildUser userPost)
        {
            Task.Delay(TimeSpan.FromMinutes(10)).Wait();
            var currentState = _client.GetUser(userPre.Id);
            if (!currentState.Status.Equals(UserStatus.Offline)) return Task.CompletedTask; // User is no longer offline
            foreach (var channel in _configService.Config.Channels)
            {
                var player = channel.SignedPlayers.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                var textChannel = _client.GetChannel(channel.Id) as ITextChannel;
                if (player != null)
                {
                    textChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":warning: Removed {player.DiscordUserMention ?? player.Name} from the line-up as they have gone offline").WithCurrentTimestamp().Build());
                    textChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(_matchmakerService.RemovePlayer(channel.Id, userPre)).WithCurrentTimestamp().Build());
                    textChannel.SendMessageAsync("", embed: _matchmakerService.GenerateTeamList(channel.Id));
                    if (_configService.Config.Channels.First(c => c.Id == channel.Id).Team2.IsMix)
                    {
                        textChannel.SendMessageAsync("", embed: _matchmakerService.GenerateTeamList(channel.Id, Teams.Team2));
                    }
                    if (player.DiscordUserId != null)
                    {
                        var dmChannel = _client.GetUser((ulong)player.DiscordUserId).GetOrCreateDMChannelAsync() as IDMChannel;
                        dmChannel.SendMessageAsync($"You've been unsigned from the line-up in {textChannel.Name} as you have gone offline. Sorry champ.");
                    }
                }
                var sub = channel.Team1.Substitutes.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                if (sub != null)
                {
                    textChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(_matchmakerService.RemoveSub(channel.Id, userPre)).WithCurrentTimestamp().Build());
                    textChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":warning: Removed {player.DiscordUserMention ?? player.Name} from the subs bench as they have gone offline").WithCurrentTimestamp().Build());
                }
            }
            return Task.CompletedTask;
        }

        private Task UserAway(SocketGuildUser userPre, SocketGuildUser userPost)
        {
            Task.Delay(TimeSpan.FromMinutes(15)).Wait();
            var currentState = _client.GetUser(userPre.Id);
            if (currentState.Status.Equals(UserStatus.Online)) return Task.CompletedTask; // User is no longer AFK/Idle
            foreach (var channel in _configService.Config.Channels)
            {
                var player = channel.SignedPlayers.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                var sub = channel.Team1.Substitutes.FirstOrDefault(p => p.DiscordUserId == userPost.Id);
                if (player != null)
                {
                    var textChannel = _client.GetChannel(channel.Id) as ITextChannel;
                    textChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":clock1: {player.DiscordUserMention ?? player.Name} might be AFK. Keep your eyes peeled.").WithCurrentTimestamp().Build());
                }
                if (sub != null)
                {
                    var textChannel = _client.GetChannel(channel.Id) as ITextChannel;
                    textChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($":clock1: {sub.DiscordUserMention ?? sub.Name} might be AFK. Keep your eyes peeled.").WithCurrentTimestamp().Build());
                }
            }
            return Task.CompletedTask;
        }
    }
}
