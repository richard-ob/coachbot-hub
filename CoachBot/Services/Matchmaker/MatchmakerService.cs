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

        public List<Channel> Channels { get; set; }

        public MatchmakerService(ConfigService configService)
        {
            _configService = configService;
            Channels = configService.config.Channels;
        }

        public string ConfigureChannel(ulong channelId, string teamName, List<string> positions, bool isMixChannel = false)
        {
            if (positions.Count() <= 1) return "You must add at least two positions";
            if (positions.GroupBy(p => p).Where(g => g.Count() > 1).Any()) return "All positions must be unique";

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
            return "Channel successfully configured";
        }

        public string AddPlayer(ulong channelId, IUser user, string position = null, Teams team = Teams.Team1)
        {
            var channel = Channels.First(c => c.Id == channelId);
            var player = new Player()
            {
                DiscordUserId = user.Id,
                Name = user.Username,
                DiscordUserMention = user.Mention
            };
            if (channel.SignedPlayers.Any(p => p.DiscordUserId == user.Id)) return $"You are already signed, {user.Mention}";
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
                return $"Signed {player.Name} to {position} for {channel.Team1.Name}";
            }
            else if (positionAvailableTeam2)
            {
                channel.Team2.Players.Add(player, position);
                return $"Signed {player.Name} to {position} for {channel.Team2.Name}";
            }
            else
            {
                return $"Position unavailable. Please try again, {user.Mention}.";
            }
        }

        public string AddPlayer(ulong channelId, string playerName, string position, Teams team = Teams.Team1)
        {
            var channel = Channels.First(c => c.Id == channelId);
            var player = new Player()
            {
                Name = playerName
            };
            position = position.ToUpper();
            if (channel.SignedPlayers.Any(p => p.Name == playerName)) return $"{playerName} is already signed.";
            var positionAvailableTeam1 = !channel.Team1.Players.Any(p => p.Value == position) && channel.Positions.Any(p => p == position);
            var positionAvailableTeam2 = !channel.Team2.Players.Any(p => p.Value == position) && channel.Positions.Any(p => p == position);
            if (positionAvailableTeam1 && team == Teams.Team1)
            {
                channel.Team1.Players.Add(player, position);
                return $"Signed {player.Name} to {position} for {channel.Team1.Name}";
            }
            else if (positionAvailableTeam2 && channel.Team2.IsMix)
            {
                channel.Team2.Players.Add(player, position);
                return $"Signed {player.Name} to {position} for {channel.Team2.Name}";
            }
            else
            {
                return "Position unavailable. Please try again.";
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
            Channels.First(c => c.Id == channelId).Team2 = team;
            return $"{team.Name} is challenging";
        }

        public void ResetMatch(ulong channelId)
        {
            var channelConfig = _configService.ReadChannelConfiguration(channelId);
            Channels.FirstOrDefault(c => c.Id == channelId).Team1 = channelConfig.Team1;
            Channels.FirstOrDefault(c => c.Id == channelId).Team2 = channelConfig.Team2;
        }

        public string ReadyMatch(ulong channelId, int? serverId = null)
        {
            var channel = Channels.First(c => c.Id == channelId);
            if ((channel.Positions.Count() * 2) != (channel.SignedPlayers.Count())) return "All positions must be filled";

            var sb = new StringBuilder();
            sb.Append($"Match Ready! Join the server ASAP!");
            sb.Append(Environment.NewLine);
            foreach (var player in channel.SignedPlayers)
            {
                sb.Append($"{player.DiscordUserMention ?? player.Name} ");
            }
            sb.AppendLine(GenerateTeamList(channelId));
            ResetMatch(channelId);
            return sb.ToString();
        }

        public string GenerateTeamList(ulong channelId)
        {
            var teamList = new StringBuilder();
            var channel = Channels.First(c => c.Id == channelId);
            teamList.Append($"***Team {channel.Team1.Name}***");
            teamList.Append("```");
            foreach (var position in channel.Positions)
            {
                var player = channel.Team1.Players.FirstOrDefault(p => p.Value == position).Key;
                var playerName = player != null ? player.Name : "";
                teamList.AppendFormat("{0}:{1} ", position, playerName);
            }
            teamList.Append("```");

            if (channel.Team2.IsMix)
            {
                teamList.Append($"***Team {channel.Team2.Name}***");
                teamList.Append("```");
                foreach (var position in channel.Positions)
                {
                    var player = channel.Team2.Players.FirstOrDefault(p => p.Value == position).Key;
                    var playerName = player != null ? player.Name : "";
                    teamList.AppendFormat("{0}:{1} ", position, playerName);
                }
                teamList.Append("```");
            }
            else if (channel.Team2.Name != null && !string.IsNullOrEmpty(channel.Team2.Name))
            {
                teamList.Append($"***Vs Team {channel.Team2.Name}***");
            }
            return teamList.ToString();
        }
    }
}
