using Discord;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoachBot.Model;
using System;

namespace CoachBot.Services.Matchmaker
{
    public class MatchmakerService
    {
        private readonly ConfigService _configService;
        private readonly StatisticsService _statisticsService;

        private Channel _lastMatch;

        public List<Channel> Channels = new List<Channel>();

        public MatchmakerService(ConfigService configService, StatisticsService statisticsService)
        {
            _configService = configService;
            _statisticsService = statisticsService;
            foreach(var channel in _configService.Config.Channels)
            {
                Channels.Add(new Channel()
                {
                    Id = channel.Id,
                    Team1 = new Team()
                    {
                        IsMix = channel.Team1.IsMix,
                        Name = channel.Team1.Name,
                        Players = new Dictionary<Player, string>()
                    },
                    Positions = channel.Positions.Select(p => new string(p.ToCharArray())).ToList(),
                    Team2 = new Team()
                    {
                        IsMix = channel.Team2.IsMix,
                        Players = new Dictionary<Player, string>()
                    }
                });
            } 
        }

        public string ConfigureChannel(ulong channelId, string teamName, List<string> positions, bool isMixChannel = false)
        {
            if (positions.Count() <= 1) return ":exclamation: You must add at least two positions";
            if (positions.GroupBy(p => p).Where(g => g.Count() > 1).Any()) return ":exclamation: All positions must be unique";

            var existingChannelConfig = Channels.FirstOrDefault(c => c.Id.Equals(channelId));
            if (existingChannelConfig != null) Channels.Remove(existingChannelConfig);

            var channel = new Channel()
            {
                Id = channelId,
                Positions = positions.Select(p => p.ToUpper()).ToList(),
                Team1 = new Team()
                {
                    IsMix = true,
                    Name = teamName,
                    Players = new Dictionary<Player, string>()
                },
                Team2 = new Team()
                {
                    IsMix = isMixChannel,
                    Name = isMixChannel ? "Mix" : null,
                    Players = new Dictionary<Player, string>()
                }
            };
            Channels.Add(channel);
            _configService.UpdateChannelConfiguration(channel);
            return ":white_check_mark: Channel successfully configured";
        }

        public string AddPlayer(ulong channelId, IUser user, string position = null, Teams team = Teams.Team1)
        {
            if(position != null) position = position.Replace("#", string.Empty);
            var channel = Channels.First(c => c.Id == channelId);
            var player = new Player()
            {
                DiscordUserId = user.Id,
                Name = user.Username,
                DiscordUserMention = user.Mention
            };
            if (channel.SignedPlayers.Any(p => p.DiscordUserId == user.Id)) return $":exclamation: You are already signed, {user.Mention}";
            if (position == null)
            {
                position = channel.Positions.FirstOrDefault(p => !channel.Team1.Players.Any(pl => pl.Value == p) || !channel.Team2.Players.Any(pl => pl.Value == p));
            }
            else
            {
                position = position.ToUpper();
            }
            var positionAvailableTeam1 = !channel.Team1.Players.Any(p => p.Value == position) && channel.Positions.Any(p => p == position);
            var positionAvailableTeam2 = !channel.Team2.Players.Any(p => p.Value == position) && channel.Positions.Any(p => p == position) && channel.Team2.IsMix;
            if (positionAvailableTeam1 && team == Teams.Team1)
            {
                channel.Team1.Players.Add(player, position);
                return $":heavy_plus_sign: Signed **{player.Name}** to **{position}** for **{channel.Team1.Name}**";
            }
            else if (positionAvailableTeam2)
            {
                channel.Team2.Players.Add(player, position);
                return $":heavy_plus_sign: Signed **{player.Name}** to **{position}** for **{channel.Team2.Name ?? "Mix"}**";
            }
            else
            {
                return $":exclamation: Position unavailable. Please try again, {user.Mention}.";
            }
        }

        public string AddPlayer(ulong channelId, string playerName, string position, Teams team = Teams.Team1)
        {
            position = position.Replace("#", string.Empty);
            var channel = Channels.First(c => c.Id == channelId);
            var player = new Player()
            {
                Name = playerName
            };
            position = position.ToUpper();
            if (channel.SignedPlayers.Any(p => p.Name == playerName)) return $":exclamation: **{playerName}** is already signed.";
            var positionAvailableTeam1 = !channel.Team1.Players.Any(p => p.Value == position) && channel.Positions.Any(p => p == position);
            var positionAvailableTeam2 = !channel.Team2.Players.Any(p => p.Value == position) && channel.Positions.Any(p => p == position);
            if (positionAvailableTeam1 && team == Teams.Team1)
            {
                channel.Team1.Players.Add(player, position);
                return $":heavy_plus_sign: Signed **{player.Name}** to **{position}** for **{channel.Team1.Name}**";
            }
            else if (positionAvailableTeam2 && channel.Team2.IsMix)
            {
                channel.Team2.Players.Add(player, position);
                return $":heavy_plus_sign: Signed **{player.Name}** to **{position}** for **{channel.Team2.Name ?? "Mix"}**";
            }
            else
            {
                return ":exclamation: Position unavailable. Please try again.";
            }
        }

        public void RemovePlayer(ulong channelId, IUser user)
        {
            var player = Channels.First(c => c.Id == channelId).Team1.Players.FirstOrDefault(p => p.Key.DiscordUserId == user.Id);
            if (player.Key == null) player = Channels.FirstOrDefault(c => c.Id == channelId).Team2.Players.First(p => p.Key.DiscordUserId == user.Id);
            if (player.Key != null)
            {
                Channels.First(c => c.Id == channelId).Team1.Players.Remove(player.Key);
                Channels.First(c => c.Id == channelId).Team2.Players.Remove(player.Key);
            }
        }

        public void RemovePlayer(ulong channelId, string playerName)
        {
            var player = Channels.FirstOrDefault(c => c.Id == channelId).Team1.Players.First(p => p.Key.Name == playerName);
            if (player.Key == null) Channels.FirstOrDefault(c => c.Id == channelId).Team2.Players.First(p => p.Key.Name == playerName);
            if (player.Key != null)
            {
                Channels.First(c => c.Id == channelId).Team1.Players.Remove(player.Key);
                Channels.First(c => c.Id == channelId).Team2.Players.Remove(player.Key);
            }
        }

        public string ChangeOpposition(ulong channelId, Team team)
        {
            var previousOpposition = Channels.First(c => c.Id == channelId).Team2;
            Channels.First(c => c.Id == channelId).Team2 = team;
            if (team.Name == null && previousOpposition.Name != null) return $":heavy_minus_sign: Opposition removed";
            if (team.Name == null && previousOpposition.Name == null) return $":exclamation: You must provide a team name to face";
            return $":busts_in_silhouette: **{team.Name}** are challenging";
        }

        public void ResetMatch(ulong channelId)
        {
            var channelConfig = _configService.ReadChannelConfiguration(channelId);
            Channels.FirstOrDefault(c => c.Id == channelId).Team1 = new Team()
            {
                IsMix = channelConfig.Team1.IsMix,
                Name = channelConfig.Team1.Name,
                Players = new Dictionary<Player, string>()
            };
            Channels.FirstOrDefault(c => c.Id == channelId).Team2 = new Team()
            {
                IsMix = channelConfig.Team2.IsMix,
                Name = null,
                Players = new Dictionary<Player, string>()
            };
        }

        public string ReadyMatch(ulong channelId, int? serverId = null)
        {
            var channel = Channels.First(c => c.Id == channelId);
            // Need to work out logic for Single GK to make this work properly
            if (channel.Team2.IsMix == true && (channel.Positions.Count() * 2) - 1 > (channel.SignedPlayers.Count())) return ":exclamation: All positions must be filled";
            if (channel.Team2.IsMix == false && (channel.Positions.Count()) - 1 > (channel.SignedPlayers.Count())) return ":exclamation: All positions must be filled";
            if (channel.Team2.Name == null) return ":exclamation: You must set a team to face";

            var sb = new StringBuilder();
            var servers = _configService.Config.Servers;
            if (serverId != null && servers.Any() && serverId > 0 && serverId <= servers.Count())
            {
                var server = _configService.Config.Servers[(int)serverId - 1];
                sb.Append($":checkered_flag: Match Ready! Join {server.Name} steam://connect/{server.Address} ");
            }
            else
            {
                sb.Append($":checkered_flag: Match Ready! Join the server ASAP! ");
            }
            foreach (var player in channel.SignedPlayers)
            {
                sb.Append($"{player.DiscordUserMention ?? player.Name} ");
            }
            sb.AppendLine();
            _statisticsService.AddMatch(channel);
            //ResetMatch(channelId);
            return sb.ToString();
        }

        public string UnreadyMatch(ulong channelId)
        {
            if (_lastMatch == null) return ":exclamation: No previous line-up available to revert to";
            if (_lastMatch.Id != channelId) return ":exclamation: No previous line-up available to revert to";

            Channels.First(c => c.Id == channelId).Positions = _lastMatch.Positions;
            Channels.First(c => c.Id == channelId).Team1 = _lastMatch.Team1;
            Channels.First(c => c.Id == channelId).Team2 = _lastMatch.Team2;
            return ":white_check_mark: Previous line-up restored";
        }

        public Embed GenerateTeamListClassic(ulong channelId)
        {
            var teamList = new StringBuilder();
            var embedFooterBuilder = new EmbedFooterBuilder();
            var channel = Channels.First(c => c.Id == channelId);
            foreach (var position in channel.Positions)
            {
                var player = channel.Team1.Players.FirstOrDefault(p => p.Value == position).Key;
                var playerName = player != null ? player.Name : "";
                teamList.AppendFormat("**{0}**:{1} ", position, playerName);
            }

            if (channel.Team2.IsMix)
            {
                teamList.AppendLine("");
                foreach (var position in channel.Positions)
                {
                    var player = channel.Team2.Players.FirstOrDefault(p => p.Value == position).Key;
                    var playerName = player != null ? player.Name : "";
                    teamList.AppendFormat("**{0}**:{1} ", position, playerName);
                }
            }
            else if (channel.Team2.Name != null && !string.IsNullOrEmpty(channel.Team2.Name))
            {
                embedFooterBuilder.WithText($"vs {channel.Team2.Name}");
            }
            teamList.AppendLine();
            var builder = new EmbedBuilder().WithTitle($"{channel.Team1.Name} Team List").WithDescription(teamList.ToString()).WithFooter(embedFooterBuilder);
            return builder.Build();
        }

        public Embed GenerateTeamList(ulong channelId, Teams teamType = Teams.Team1)
        {
            var teamList = new StringBuilder();
            var embedFooterBuilder = new EmbedFooterBuilder();
            var channel = Channels.First(c => c.Id == channelId);
            var availablePlaceholderText = ":grey_question:";
            var team = teamType == Teams.Team1 ? channel.Team1 : channel.Team2;
            var oppositionTeam = teamType == Teams.Team1 ? channel.Team2 : channel.Team1;
            var builder = new EmbedBuilder().WithTitle($"{team.Name} Team List")
                                            .WithDescription(oppositionTeam.Name != null ? $"vs {oppositionTeam.Name}" : "");
            if (teamType == Teams.Team1)
            {
                builder.WithColor(new Color(0x31b610));
            }
            else
            {
                builder.WithColor(new Color(0xff0606));
            }

            if (channel.Positions.Count() == 8)
            {
                builder.AddInlineField("\u200B", "\u200B");
                var player8 = team.Players.FirstOrDefault(p => p.Value == channel.Positions[7]).Key;
                builder.AddInlineField(player8 != null ? player8.Name : availablePlaceholderText, AddPrefix(channel.Positions[7]));
                builder.AddInlineField("\u200B", "\u200B");
                var player7 = team.Players.FirstOrDefault(p => p.Value == channel.Positions[6]).Key;
                builder.AddInlineField(player7 != null ? player7.Name : availablePlaceholderText, AddPrefix(channel.Positions[6]));
                var player6 = team.Players.FirstOrDefault(p => p.Value == channel.Positions[5]).Key;
                builder.AddInlineField(player6 != null ? player6.Name : availablePlaceholderText, AddPrefix(channel.Positions[5]));
                var player5 = team.Players.FirstOrDefault(p => p.Value == channel.Positions[4]).Key;
                builder.AddInlineField(player5 != null ? player5.Name : availablePlaceholderText, AddPrefix(channel.Positions[4]));
                var player4 = team.Players.FirstOrDefault(p => p.Value == channel.Positions[3]).Key;
                builder.AddInlineField(player4 != null ? player4.Name : availablePlaceholderText, AddPrefix(channel.Positions[3]));
                var player3 = team.Players.FirstOrDefault(p => p.Value == channel.Positions[2]).Key;
                builder.AddInlineField(player3 != null ? player3.Name : availablePlaceholderText, AddPrefix(channel.Positions[2]));
                var player2 = team.Players.FirstOrDefault(p => p.Value == channel.Positions[1]).Key;
                builder.AddInlineField(player2 != null ? player2.Name : availablePlaceholderText, AddPrefix(channel.Positions[1]));
                builder.AddInlineField("\u200B", "\u200B");
                var player1 = team.Players.FirstOrDefault(p => p.Value == channel.Positions[0]).Key;
                builder.AddInlineField(player1 != null ? player1.Name : availablePlaceholderText, AddPrefix(channel.Positions[0]));
                builder.AddInlineField("\u200B", "\u200B");
            }
            else if (channel.Positions.Count() == 4)
            {
                builder.AddInlineField("\u200B", "\u200B");
                var player4 = team.Players.FirstOrDefault(p => p.Value == channel.Positions[3]).Key;
                builder.AddInlineField(player4 != null ? player4.Name : availablePlaceholderText, AddPrefix(channel.Positions[3]));
                builder.AddInlineField("\u200B", "\u200B");
                var player3 = team.Players.FirstOrDefault(p => p.Value == channel.Positions[2]).Key;
                builder.AddInlineField(player3 != null ? player3.Name : availablePlaceholderText, AddPrefix(channel.Positions[2]));
                builder.AddInlineField("\u200B", "\u200B");
                var player2 = team.Players.FirstOrDefault(p => p.Value == channel.Positions[1]).Key;
                builder.AddInlineField(player2 != null ? player2.Name : availablePlaceholderText, AddPrefix(channel.Positions[1]));
                builder.AddInlineField("\u200B", "\u200B");
                var player1 = team.Players.FirstOrDefault(p => p.Value == channel.Positions[0]).Key;
                builder.AddInlineField(player1 != null ? player1.Name : availablePlaceholderText, AddPrefix(channel.Positions[0]));
                builder.AddInlineField("\u200B", "\u200B");
            }
            else
            {
                return GenerateTeamListClassic(channelId);
            }
            return builder.Build();
        }

        public static string AddPrefix(string position)
        {
            int parsedInt = 0;
            if (int.TryParse(position, out parsedInt) == true)
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
