using Discord;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoachBot.Model;
using System;
using CoachBot.Extensions;
using Discord.WebSocket;
using System.Timers;
using System.Threading.Tasks;

namespace CoachBot.Services.Matchmaker
{
    public class MatchmakerService
    {
        private readonly ConfigService _configService;
        private readonly StatisticsService _statisticsService;
        private DiscordSocketClient _client;

        public MatchmakerService(ConfigService configService, StatisticsService statisticsService, DiscordSocketClient client)
        {
            _configService = configService;
            _statisticsService = statisticsService;
            _client = client;
        }

        public string ConfigureChannel(ulong channelId, string teamName, List<Position> positions, string kitEmote = null, string badgeEmote = null, string color = null, bool isMixChannel = false, Formation formation = 0, bool classicLineup = false)
        {
            if (positions.Count() <= 1) return ":no_entry: You must add at least two positions";
            if (positions.GroupBy(p => p).Where(g => g.Count() > 1).Any()) return ":no_entry: All positions must be unique";

            var existingChannelConfig = _configService.Config.Channels.FirstOrDefault(c => c.Id.Equals(channelId));
            if (existingChannelConfig != null) _configService.Config.Channels.Remove(existingChannelConfig);

            var channel = new Channel()
            {
                Id = channelId,
                Positions = positions.Select(p => new Position() { PositionName = p.PositionName.ToUpper() }).ToList(),
                Team1 = new Team()
                {
                    IsMix = true,
                    Name = teamName,
                    KitEmote = kitEmote,
                    BadgeEmote = badgeEmote,
                    Color = color,
                    Players = new List<Player>(),
                    Substitutes = new List<Player>()
                },
                Team2 = new Team()
                {
                    IsMix = isMixChannel,
                    Name = isMixChannel && teamName.ToLower() == "mix" ? "Mix #2" : isMixChannel ? "Mix" : null,
                    Players = new List<Player>(),
                },
                Formation = formation,
                ClassicLineup = classicLineup
            };
            _configService.UpdateChannelConfiguration(channel);
            return ":white_check_mark: Channel successfully configured";
        }

        public string AddPlayer(ulong channelId, IUser user, string position = null, Teams team = Teams.Team1)
        {
            if(position != null) position = position.Replace("#", string.Empty);
            var channel = _configService.Config.Channels.First(c => c.Id == channelId);
            var player = new Player()
            {
                DiscordUserId = user.Id,
                Name = user.Username,
                DiscordUserMention = user.Mention
            };
            if (channel.SignedPlayers.Any(p => p.DiscordUserId == user.Id)) return $":no_entry: You are already signed, {user.Mention}";
            if (channel.Team1.Substitutes.Any(s => s.DiscordUserId == user.Id))
            {
                var sub = channel.Team1.Substitutes.First(s => s.DiscordUserId == user.Id);
                channel.Team1.Substitutes.Remove(sub);
            }
            if (position == null && team == Teams.Team1)
            {
                position = channel.Positions.FirstOrDefault(p => !channel.Team1.Players.Any(pl => pl.Position.PositionName == p.PositionName)).PositionName;
            }
            else if (position == null && team == Teams.Team2)
            {
                position = channel.Positions.FirstOrDefault(p => !channel.Team2.Players.Any(pl => pl.Position.PositionName == p.PositionName)).PositionName;
            }

            position = position.ToUpper();
            var positionAvailableTeam1 = !channel.Team1.Players.Any(p => p.Position.PositionName == position) && channel.Positions.Any(p => p.PositionName == position);
            var positionAvailableTeam2 = !channel.Team2.Players.Any(p => p.Position.PositionName == position) && channel.Positions.Any(p => p.PositionName == position) && channel.Team2.IsMix;

            if (positionAvailableTeam1 && team == Teams.Team1)
            {
                player.Position = new Position(position);
                channel.Team1.Players.Add(player);
                return $":white_check_mark:  Signed **{player.Name}** to **{position}** for **{channel.Team1.Name}**";
            }
            else if (positionAvailableTeam2)
            {
                player.Position = new Position(position);
                channel.Team2.Players.Add(player);
                return $":white_check_mark:  Signed **{player.Name}** to **{position}** for **{channel.Team2.Name ?? "Mix"}**";
            }
            else
            {
                return $":no_entry: Position unavailable. Please try again, {user.Mention}.";
            }
        }

        public string AddPlayer(ulong channelId, string playerName, string position, Teams team = Teams.Team1)
        {
            position = position.Replace("#", string.Empty);
            var channel = _configService.Config.Channels.First(c => c.Id == channelId);
            var player = new Player()
            {
                Name = playerName,
                Position = new Position()
            };
            position = position.ToUpper();
            if (channel.SignedPlayers.Any(p => p.Name == playerName)) return $":no_entry: **{playerName}** is already signed.";
            var positionAvailableTeam1 = !channel.Team1.Players.Any(p => p.Position.PositionName == position) && channel.Positions.Any(p => p.PositionName == position);
            var positionAvailableTeam2 = !channel.Team2.Players.Any(p => p.Position.PositionName == position) && channel.Positions.Any(p => p.PositionName == position);
            if (positionAvailableTeam1 && team == Teams.Team1)
            {
                player.Position.PositionName = position;
                channel.Team1.Players.Add(player);
                return $":white_check_mark:  Signed **{player.Name}** to **{position}** for **{channel.Team1.Name}**";
            }
            else if (positionAvailableTeam2 && channel.Team2.IsMix)
            {
                player.Position.PositionName = position;
                channel.Team2.Players.Add(player);
                return $":white_check_mark:  Signed **{player.Name}** to **{position}** for **{channel.Team2.Name ?? "Mix"}**";
            }
            else
            {
                return ":no_entry: Position unavailable. Please try again.";
            }
        }

        public string RemovePlayer(ulong channelId, IUser user)
        {
            var channel = _configService.Config.Channels.FirstOrDefault(c => c.Id == channelId);
            if (channel.Team1.Players.Any(p => p.DiscordUserId == user.Id))
            {
                var player = channel.Team1.Players.First(p => p.DiscordUserId == user.Id);
                channel.Team1.Players.Remove(player);
                if (channel.Team1.Substitutes.Any() && player.Position.PositionName.ToLower() != "gk")
                {
                    var sub = channel.Team1.Substitutes.FirstOrDefault();
                    channel.Team1.Substitutes.Remove(sub);
                    sub.Position.PositionName = player.Position.PositionName;
                    channel.Team1.Players.Add(sub);
                    return $":arrows_counterclockwise:  **Substitution** {Environment.NewLine} {sub.DiscordUserMention} comes off the bench to replace **{user.Username}**";
                }
                return $":negative_squared_cross_mark: Unsigned **{user.Username}**";
            }
            if (channel.Team2.Players.Any(p => p.DiscordUserId == user.Id))
            {
                channel.Team2.Players.Remove(channel.Team2.Players.First(p => p.DiscordUserId == user.Id));
                return $":negative_squared_cross_mark: Unsigned **{user.Username}**";
            }
            return $":no_entry: You are not signed {user.Mention}";
        }

        public string RemovePlayer(ulong channelId, string playerName)
        {
            var channel = _configService.Config.Channels.FirstOrDefault(c => c.Id == channelId);
            if (channel.Team1.Players.Any(p => p.Name == playerName))
            {
                var player = channel.Team1.Players.First(p => p.Name == playerName);
                channel.Team1.Players.Remove(player);
                if (channel.Team1.Substitutes.Any() && player.Position.PositionName.ToLower() != "gk")
                {
                    var sub = channel.Team1.Substitutes.FirstOrDefault();
                    channel.Team1.Substitutes.Remove(sub);
                    sub.Position.PositionName = player.Position.PositionName;
                    channel.Team1.Players.Add(sub);
                    return $":arrows_counterclockwise:  **Substitution** {Environment.NewLine} {sub.DiscordUserMention} comes off the bench to replace **{playerName}**";
                }
                return $":negative_squared_cross_mark: Unsigned **{playerName}**";
            }
            if (channel.Team2.Players.Any(p => p.Name == playerName))
            {
                channel.Team2.Players.Remove(channel.Team2.Players.First(p => p.Name == playerName));
                return $":negative_squared_cross_mark: Unsigned **{playerName}**";
            }
            return $":no_entry: **{playerName}** is not signed";
        }

        public string AddSub(ulong channelId, IUser user)
        {
            var channel = _configService.Config.Channels.First(c => c.Id == channelId);
            var player = new Player()
            {
                DiscordUserId = user.Id,
                Name = user.Username,
                DiscordUserMention = user.Mention
            };
            if (channel.Team1.Substitutes.Any(p => p.DiscordUserId == user.Id)) return $":no_entry: You are already signed as a sub, {user.Mention}";
            if (channel.Team1.Players.Any(p => p.DiscordUserId == user.Id)) return $":no_entry: You are already signed, {user.Mention}";

            channel.Team1.Substitutes.Add(player);
            return $":white_check_mark:  Added **{player.Name}** to subs bench for **{channel.Team1.Name ?? "Mix"}**";
        }

        public string RemoveSub(ulong channelId, IUser user)
        {
            var channel = _configService.Config.Channels.First(c => c.Id == channelId);
            var player = channel.Team1.Substitutes.FirstOrDefault(s => s.DiscordUserId == user.Id);
            if (player != null)
            {
                channel.Team1.Substitutes.Remove(player);
                return $":negative_squared_cross_mark: Removed **{player.Name}** from the subs bench";
            }
            return $":no_entry: You are not on the subs bench, {user.Mention}";
        }

        public string RemoveSub(ulong channelId, string playerName)
        {
            var channel = _configService.Config.Channels.First(c => c.Id == channelId);
            var player = channel.Team1.Substitutes.FirstOrDefault(s => s.Name == playerName);
            if (player != null)
            {
                channel.Team1.Substitutes.Remove(player);
                return $":negative_squared_cross_mark: Removed **{player.Name}** from the subs bench";
            }
            return $":no_entry: {playerName} is not on the subs bench";
        }

        public string ChangeOpposition(ulong channelId, Team team)
        {
            var channel = _configService.Config.Channels.First(c => c.Id == channelId);
            var previousOpposition = _configService.Config.Channels.First(c => c.Id == channelId).Team2;
            channel.Team2 = team;
            if (channel.IsSearching) return $":no_entry: You are currently searching for opposition. Please type **!stopsearch** if you have found an opponent.";
            if (team.Name == null && previousOpposition.Name != null) return $":negative_squared_cross_mark: Opposition removed";
            if (team.Name == null && previousOpposition.Name == null) return $":no_entry: You must provide a team name to face";
            return $":busts_in_silhouette: **{team.Name}** are challenging";
        }

        public void ResetMatch(ulong channelId)
        {
            var channelConfig = _configService.ReadChannelConfiguration(channelId);
            _configService.Config.Channels.FirstOrDefault(c => c.Id == channelId).Team1 = new Team()
            {
                IsMix = channelConfig.Team1.IsMix,
                Name = channelConfig.Team1.Name,
                KitEmote = channelConfig.Team1.KitEmote,
                Color = channelConfig.Team1.Color,
                Players = new List<Player>(),
                Substitutes = new List<Player>()
            };
            _configService.Config.Channels.FirstOrDefault(c => c.Id == channelId).Team2 = new Team()
            {
                IsMix = channelConfig.Team2.IsMix,
                Name = channelConfig.Team2.IsMix && channelConfig.Team1.Name.ToLower() == "mix" ? "Mix #2" : channelConfig.Team2.IsMix ? "Mix" : null,
                Players = new List<Player>()
            };
            _configService.Config.Channels.FirstOrDefault(c => c.Id == channelId).LastHereMention = null;
        }

        public void ReadyMatch(ulong channelId, int? serverId = null, bool ignorePlayerCounts = false)
        {
            var channel = _configService.Config.Channels.First(c => c.Id == channelId);
            var socketChannel = (SocketTextChannel)_client.GetChannel(channel.Id);

            if (!ignorePlayerCounts && channel.Team2.IsMix == true && (channel.Positions.Count() * 2) - 1 > (channel.SignedPlayers.Count()))
            {
                socketChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(":no_entry: All positions must be filled").Build());
                return;
            }
            if (!ignorePlayerCounts && channel.Team2.IsMix == false && (channel.Positions.Count()) - 1 > (channel.SignedPlayers.Count()))
            {
                socketChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(":no_entry: All positions must be filled").Build());
                return;
            }
            if (channel.Team2.Name == null)
            {
                socketChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(":no_entry: You must set a team to face").Build());
                return;
            }
            if (serverId == null || serverId == 0 || serverId > _configService.Config.Servers.Count())
            {
                socketChannel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(":no_entry: Please supply a server number (e.g. !ready 3). Type !servers for the server list.").Build());
                return;
            }

            var sb = new StringBuilder();
            var server = _configService.Config.Servers[(int)serverId - 1];
            sb.Append($":checkered_flag: Match Ready! {Environment.NewLine} Join {server.Name} steam://connect/{server.Address} ");
            foreach (var player in channel.SignedPlayers)
            {
                sb.Append($"{player.DiscordUserMention ?? player.Name} ");
            }
            _statisticsService.AddMatch(channel);
            sb.AppendLine();
            ResetMatch(channelId);
            socketChannel.SendMessageAsync(sb.ToString());
        }

        public string UnreadyMatch(ulong channelId)
        {
            /*if (_lastMatch == null) return ":no_entry: No previous line-up available to revert to";
            if (_lastMatch.Id != channelId) return ":no_entry: No previous line-up available to revert to";

            _config.Channels.First(c => c.Id == channelId).Positions = _lastMatch.Positions;
            _config.Channels.First(c => c.Id == channelId).Team1 = _lastMatch.Team1;
            _config.Channels.First(c => c.Id == channelId).Team2 = _lastMatch.Team2;*/
            return ":white_check_mark: Previous line-up restored";
        }

        public string Search(ulong channelId, string challengerMention)
        {
            var challenger = _configService.Config.Channels.First(c => c.Id == channelId);
            if (challenger.Positions.Count() -1 > challenger.SignedPlayers.Count()) return ":no_entry: All outfield positions must be filled";
            if (challenger.IsSearching) return ":no_entry: You're already searching for a match. Type **!stopsearch** to cancel the previous search.";

            var embed = new EmbedBuilder()
                .WithTitle($":mag: {challenger.Team1.BadgeEmote ?? challenger.Team1.Name} are searching for a team to face")
                .WithDescription($"To challenge {challenger.Team1.Name} type **!challenge {challenger.Id}** and contact {challengerMention} for more information")
                .WithCurrentTimestamp();
            if (challenger.Team1.Color != null && challenger.Team1.Color[0] == '#')
            {
                embed.WithColor(new Color(ColorExtensions.FromHex(challenger.Team1.Color).R, ColorExtensions.FromHex(challenger.Team1.Color).G, ColorExtensions.FromHex(challenger.Team1.Color).B));
            }
            else
            {
                embed.WithColor(new Color(0xFFFFFF));
            }
            challenger.IsSearching = true;
            foreach (var channel in _configService.Config.Channels.Where(c => c.Positions.Count == challenger.Positions.Count && c.Id != channelId))
            {
                (_client.GetChannel(channel.Id) as SocketTextChannel)?.SendMessageAsync("", embed: embed.Build());
            }
            var timeout = TimeoutSearch(channelId);
            timeout.ConfigureAwait(false);

            return ":white_check_mark: Searching for opposition.. To cancel, type **!stopsearch**";
        }

        public async Task TimeoutSearch(ulong channelId)
        {
            await Task.Delay(900000);
            var channel = _configService.Config.Channels.First(c => c.Id == channelId);
            if (channel.IsSearching)
            {
                (_client.GetChannel(channelId) as SocketTextChannel)?.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(":timer: Your search for an opponent has timed out after 15 minutes. Please try again if you are still searching").WithCurrentTimestamp().Build());
                channel.IsSearching = false;
            }
        }

        public string StopSearch(ulong channelId)
        {
            var challenger = _configService.Config.Channels.First(c => c.Id == channelId);
            if (!challenger.IsSearching)
            {
                return ":no_entry: Your team is not currently searching for a match.";
            }
            challenger.IsSearching = false;
            return ":negative_squared_cross_mark: Cancelled search for opposition";
        }

        public string Challenge(ulong challengerChannelId, ulong oppositionId, string challengerMention)
        {
            var challenger = _configService.Config.Channels.First(c => c.Id == challengerChannelId);
            var opposition = _configService.Config.Channels.First(c => c.Id == oppositionId);
            if (!opposition.IsSearching) return $":no_entry: {opposition.Team1.Name} are no longer search for a team to face";
            if (challengerChannelId == oppositionId) return $":no_entry: You can't face yourself. Don't waste my time.";
            if (challenger.Positions.Count() != opposition.Positions.Count()) return $":no_entry: Sorry, {opposition.Team1.Name} are looking for an {opposition.Positions.Count()}v{opposition.Positions.Count()}";
            if (Math.Round(challenger.Positions.Count() * 0.7) > challenger.SignedPlayers.Count()) return $":no_entry: At least {Math.Round(challenger.Positions.Count() * 0.7)} positions must be filled";
            challenger.IsSearching = false;
            var acceptMsg = $":handshake: {challenger.Team1.Name} have accepted the challenge! Contact {challengerMention} to arrange further.";
            (_client.GetChannel(opposition.Id) as SocketTextChannel).SendMessageAsync("", embed: new EmbedBuilder().WithDescription(acceptMsg).WithCurrentTimestamp().Build());
            opposition.Team2 = new Team()
            {
                Name = challenger.Team1.Name,
                IsMix = false,
                ChannelId = challenger.Id,
                Players = new List<Player>()
            };
            challenger.Team2 = new Team()
            {
                Name = opposition.Team1.Name,
                IsMix = false,
                ChannelId = opposition.Id,
                Players = new List<Player>()
            };
            (_client.GetChannel(challengerChannelId) as SocketTextChannel).SendMessageAsync("", embed: GenerateTeamList(challengerChannelId, Teams.Team1));
            (_client.GetChannel(oppositionId) as SocketTextChannel).SendMessageAsync("", embed: GenerateTeamList(oppositionId, Teams.Team1));
            return $":handshake: You have successfully challenged {opposition.Team1.Name}. !ready will send both teams to the server";
        }

        public string Unchallenge(ulong challengerChannelId, string unchallenger)
        {
            var challenger = _configService.Config.Channels.First(c => c.Id == challengerChannelId);
            var opposition = _configService.Config.Channels.First(c => c.Id == challenger.Team2.ChannelId);
            if (opposition == null) return $":no_entry: You don't have any active accepted challenges to cancel. Maybe !ready has already been called?";
            var unchallengeMsg = $"The game between {challenger.Team1.Name} & {opposition.Team1.Name} has been called off by {unchallenger}";
            (_client.GetChannel(opposition.Id) as SocketTextChannel).SendMessageAsync("", embed: new EmbedBuilder().WithTitle(":thunder_cloud_rain: Match Abandoned!").WithDescription(unchallengeMsg).WithCurrentTimestamp().Build());
            (_client.GetChannel(challenger.Id) as SocketTextChannel).SendMessageAsync("", embed: new EmbedBuilder().WithTitle(":thunder_cloud_rain: Match Abandoned!").WithDescription(unchallengeMsg).WithCurrentTimestamp().Build());
            opposition.Team2.Name = null;
            opposition.Team2.ChannelId = null;
            challenger.Team2.Name = null;
            challenger.Team2.ChannelId = null;
            (_client.GetChannel(challengerChannelId) as SocketTextChannel).SendMessageAsync("", embed: GenerateTeamList(challengerChannelId, Teams.Team1));
            (_client.GetChannel(opposition.Id) as SocketTextChannel).SendMessageAsync("", embed: GenerateTeamList(opposition.Id, Teams.Team1));
            return $":negative_squared_cross_mark: You have successfully unchallenged {opposition.Team1.Name}";
        }

        public string MentionHere(ulong channelId)
        {
            var channel = _configService.Config.Channels.First(c => c.Id == channelId);
            if (channel.LastHereMention == null || channel.LastHereMention < DateTime.Now.AddMinutes(-5))
            {
                channel.LastHereMention = DateTime.Now;
                return "@here";
            }
            else
            {
                return $"The last channel highlight was less than 5 minutes ago ({String.Format("{0:T}", channel.LastHereMention)})";
            }
        }

        public Embed GenerateTeamList(ulong channelId, Teams teamType = Teams.Team1)
        {
            var teamList = new StringBuilder();
            var embedFooterBuilder = new EmbedFooterBuilder();
            var channel = _configService.Config.Channels.First(c => c.Id == channelId);
            if (channel.ClassicLineup) return GenerateTeamListVintage(channelId, teamType);
            var availablePlaceholderText = !string.IsNullOrEmpty(channel.Team1.KitEmote) && teamType == Teams.Team1 ? channel.Team1.KitEmote : ":shirt:";
            if (teamType == Teams.Team2 && (channelId == 252113301004222465 || channelId == 295580567649648641)) availablePlaceholderText = "<:redshirt:318130493755228160>";
            if (teamType == Teams.Team2 && channelId == 310829524277395457) availablePlaceholderText = "<:redshirt:318114878063902720>";
            var team = teamType == Teams.Team1 ? channel.Team1 : channel.Team2;
            var oppositionTeam = teamType == Teams.Team1 ? channel.Team2 : channel.Team1;
            var builder = new EmbedBuilder().WithTitle($"{team.BadgeEmote ?? team.Name} Team Sheet")
                                            .WithDescription(oppositionTeam.Name != null ? $"vs {oppositionTeam.Name}" : "")
                                            .WithCurrentTimestamp();
            if (teamType == Teams.Team1)
            {
                if (team.Color != null && team.Color[0] == '#')
                {
                    builder.WithColor(new Color(ColorExtensions.FromHex(team.Color).R, ColorExtensions.FromHex(team.Color).G, ColorExtensions.FromHex(team.Color).B));
                }
                else
                {
                    builder.WithColor(new Color(0x2463b0));
                }
            }
            else
            {
                builder.WithColor(new Color(0xd60e0e));
            }

            if (channel.Positions.Count() == 8 && channel.Formation == Formation.ThreeThreeOne)
            {
                builder.AddInlineField("\u200B", "\u200B");
                var player8 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[7].PositionName);
                builder.AddInlineField(player8 != null ? player8.Name : availablePlaceholderText, AddPrefix(channel.Positions[7].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player7 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[6].PositionName);
                builder.AddInlineField(player7 != null ? player7.Name : availablePlaceholderText, AddPrefix(channel.Positions[6].PositionName));
                var player6 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[5].PositionName);
                builder.AddInlineField(player6 != null ? player6.Name : availablePlaceholderText, AddPrefix(channel.Positions[5].PositionName));
                var player5 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[4].PositionName);
                builder.AddInlineField(player5 != null ? player5.Name : availablePlaceholderText, AddPrefix(channel.Positions[4].PositionName));
                var player4 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[3].PositionName);
                builder.AddInlineField(player4 != null ? player4.Name : availablePlaceholderText, AddPrefix(channel.Positions[3].PositionName));
                var player3 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[2].PositionName);
                builder.AddInlineField(player3 != null ? player3.Name : availablePlaceholderText, AddPrefix(channel.Positions[2].PositionName));
                var player2 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[1].PositionName);
                builder.AddInlineField(player2 != null ? player2.Name : availablePlaceholderText, AddPrefix(channel.Positions[1].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player1 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[0].PositionName);
                builder.AddInlineField(player1 != null ? player1.Name : availablePlaceholderText, AddPrefix(channel.Positions[0].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
            }
            else if (channel.Positions.Count() == 8 && channel.Formation == Formation.ThreeTwoTwo)
            {
                var player8 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[7].PositionName);
                builder.AddInlineField(player8 != null ? player8.Name : availablePlaceholderText, AddPrefix(channel.Positions[7].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player7 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[6].PositionName);
                builder.AddInlineField(player7 != null ? player7.Name : availablePlaceholderText, AddPrefix(channel.Positions[6].PositionName));
                var player6 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[5].PositionName);
                builder.AddInlineField(player6 != null ? player6.Name : availablePlaceholderText, AddPrefix(channel.Positions[5].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player5 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[4].PositionName);
                builder.AddInlineField(player5 != null ? player5.Name : availablePlaceholderText, AddPrefix(channel.Positions[4].PositionName));
                var player4 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[3].PositionName);
                builder.AddInlineField(player4 != null ? player4.Name : availablePlaceholderText, AddPrefix(channel.Positions[3].PositionName));
                var player3 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[2].PositionName);
                builder.AddInlineField(player3 != null ? player3.Name : availablePlaceholderText, AddPrefix(channel.Positions[2].PositionName));
                var player2 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[1].PositionName);
                builder.AddInlineField(player2 != null ? player2.Name : availablePlaceholderText, AddPrefix(channel.Positions[1].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player1 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[0].PositionName);
                builder.AddInlineField(player1 != null ? player1.Name : availablePlaceholderText, AddPrefix(channel.Positions[0].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
            }
            else if (channel.Positions.Count() == 8 && channel.Formation == Formation.ThreeOneTwoOne)
            {
                builder.AddInlineField("\u200B", "\u200B");
                var player8 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[7].PositionName);
                builder.AddInlineField(player8 != null ? player8.Name : availablePlaceholderText, AddPrefix(channel.Positions[7].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player7 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[6].PositionName);
                builder.AddInlineField(player7 != null ? player7.Name : availablePlaceholderText, AddPrefix(channel.Positions[6].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player6 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[5].PositionName);
                builder.AddInlineField(player6 != null ? player6.Name : availablePlaceholderText, AddPrefix(channel.Positions[5].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player5 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[4].PositionName);
                builder.AddInlineField(player5 != null ? player5.Name : availablePlaceholderText, AddPrefix(channel.Positions[4].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player4 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[3].PositionName);
                builder.AddInlineField(player4 != null ? player4.Name : availablePlaceholderText, AddPrefix(channel.Positions[3].PositionName));
                var player3 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[2].PositionName);
                builder.AddInlineField(player3 != null ? player3.Name : availablePlaceholderText, AddPrefix(channel.Positions[2].PositionName));
                var player2 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[1].PositionName);
                builder.AddInlineField(player2 != null ? player2.Name : availablePlaceholderText, AddPrefix(channel.Positions[1].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player1 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[0].PositionName);
                builder.AddInlineField(player1 != null ? player1.Name : availablePlaceholderText, AddPrefix(channel.Positions[0].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
            }
            else if (channel.Positions.Count() == 8 && channel.Formation == Formation.ThreeOneThree)
            {
                var player8 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[7].PositionName);
                builder.AddInlineField(player8 != null ? player8.Name : availablePlaceholderText, AddPrefix(channel.Positions[7].PositionName));
                var player7 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[6].PositionName);
                builder.AddInlineField(player7 != null ? player7.Name : availablePlaceholderText, AddPrefix(channel.Positions[6].PositionName));
                var player6 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[5].PositionName);
                builder.AddInlineField(player6 != null ? player6.Name : availablePlaceholderText, AddPrefix(channel.Positions[5].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player5 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[4].PositionName);
                builder.AddInlineField(player5 != null ? player5.Name : availablePlaceholderText, AddPrefix(channel.Positions[4].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player4 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[3].PositionName);
                builder.AddInlineField(player4 != null ? player4.Name : availablePlaceholderText, AddPrefix(channel.Positions[3].PositionName));
                var player3 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[2].PositionName);
                builder.AddInlineField(player3 != null ? player3.Name : availablePlaceholderText, AddPrefix(channel.Positions[2].PositionName));
                var player2 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[1].PositionName);
                builder.AddInlineField(player2 != null ? player2.Name : availablePlaceholderText, AddPrefix(channel.Positions[1].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player1 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[0].PositionName);
                builder.AddInlineField(player1 != null ? player1.Name : availablePlaceholderText, AddPrefix(channel.Positions[0].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
            }
            else if (channel.Positions.Count() == 4 && channel.Formation == Formation.TwoOne)
            {
                builder.AddInlineField("\u200B", "\u200B");
                var player4 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[3].PositionName);
                builder.AddInlineField(player4 != null ? player4.Name : availablePlaceholderText, AddPrefix(channel.Positions[3].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player3 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[2].PositionName);
                builder.AddInlineField(player3 != null ? player3.Name : availablePlaceholderText, AddPrefix(channel.Positions[2].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player2 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[1].PositionName);
                builder.AddInlineField(player2 != null ? player2.Name : availablePlaceholderText, AddPrefix(channel.Positions[1].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
                var player1 = team.Players.FirstOrDefault(p => p.Position.PositionName == channel.Positions[0].PositionName);
                builder.AddInlineField(player1 != null ? player1.Name : availablePlaceholderText, AddPrefix(channel.Positions[0].PositionName));
                builder.AddInlineField("\u200B", "\u200B");
            }
            else
            {
                foreach (var position in channel.Positions)
                {
                    var player = team.Players.FirstOrDefault(p => p.Position.PositionName == position.PositionName);
                    builder.AddInlineField(player != null ? player.Name : availablePlaceholderText, AddPrefix(position.PositionName));
                }
                if (channel.Positions.Count() % 3 == 2) // Ensure that two-column fields are three-columns to ugly alignment
                {
                    builder.AddInlineField("\u200B", "\u200B");
                }
            }
            if (teamType == Teams.Team1 && team.Substitutes.Any())
            {
                var subs = new StringBuilder();
                foreach (var sub in team.Substitutes)
                {
                    if (team.Substitutes.Last() != sub)
                    {
                        subs.Append($"{sub.Name}, ");
                    }
                    else
                    {
                        subs.Append($"{sub.Name}");
                    }
                }
                builder.AddField("Subs", subs.ToString());
            }
            return builder.Build();
        }

        public Embed GenerateTeamListVintage(ulong channelId, Teams teamType = Teams.Team1)
        {
            var teamList = new StringBuilder();
            var channel = _configService.Config.Channels.First(c => c.Id == channelId);
            var team = teamType == Teams.Team1 ? channel.Team1 : channel.Team2; 
            var sb = new StringBuilder();
            var teamColor = new Color(teamType == Teams.Team1 ? (uint)0x2463b0 : (uint)0xd60e0e);
            var emptyPos = ":grey_question:";

            if (team.Color != null && teamType == Teams.Team1 && team.Color[0] == '#')
            {
                teamColor = new Color(ColorExtensions.FromHex(team.Color).R, ColorExtensions.FromHex(team.Color).G, ColorExtensions.FromHex(team.Color).B);
            }

            var embedBuilder = new EmbedBuilder().WithTitle($"{team.BadgeEmote ?? team.Name} Team List");
            foreach(var position in channel.Positions)
            {
                var player = team.Players.FirstOrDefault(p => p.Position.PositionName == position.PositionName);
                var playerName = player != null ? $"**{player.Name}**" : emptyPos;
                sb.Append($"{position.PositionName}:{playerName} ");
            }
            if (teamType == Teams.Team1 && team.Substitutes.Any())
            {
                var subs = new StringBuilder();
                foreach (var sub in team.Substitutes)
                {
                    if (team.Substitutes.Last() != sub)
                    {
                        subs.Append($"{sub.Name}, ");
                    }
                    else
                    {
                        subs.Append($"{sub.Name}");
                    }
                }
                sb.Append($"*Subs*: **{subs.ToString()}**");
            }
            if (!string.IsNullOrEmpty(channel.Team2.Name) && teamType == Teams.Team1 && !channel.Team2.IsMix)
            {
                sb.AppendLine("");
                sb.Append($"vs {channel.Team2.Name}");
            }

            return embedBuilder.WithColor(teamColor).WithDescription(sb.ToString()).WithCurrentTimestamp().Build();
        }

        public static string AddPrefix(string position)
        {
            if (int.TryParse(position, out int parsedInt) == true)
            {
                return $"#{position}";
            }
            else
            {
                return position;
            }
        }
    }
}
