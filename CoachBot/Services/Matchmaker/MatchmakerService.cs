using Discord;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoachBot.Model;

namespace CoachBot.Services.Matchmaker
{
    public class MatchmakerService
    {
        public List<Channel> Channels { get; set; }

        public MatchmakerService()
        {
            Channels = new List<Channel>();
        }

        public void ConfigureChannel(ulong channelId, List<string> positions)
        {
            if (Channels.Any(c => c.Id.Equals(channelId))) return;

            var channel = new Channel()
            {
                Id = channelId,
                Positions = positions,
                Team1 = new Team()
                {
                    IsMix = true,
                    Name = "Test",
                    Players = new Dictionary<IUser, string>()
                },
                Team2 = new Team()
                {
                    IsMix = true,
                    Name = "Test",
                    Players = new Dictionary<IUser, string>()
                }
            };
            Channels.Add(channel);
        }

        public void AddPlayer(ulong channelId, IUser user, string position)
        {
            Channels.First(c => c.Id == channelId).Team1.Players.Add(user, position);
        }

        public void RemovePlayer(ulong channelId, IUser user)
        {
            Channels.First(c => c.Id == channelId).Team1.Players.Remove(user);
        }

        public string GenerateTeamList(ulong channelId)
        {
            var teamList = new StringBuilder();
            var channel = Channels.First(c => c.Id == channelId);
            foreach (var position in channel.Positions)
            {
                var player = channel.Team1.Players.FirstOrDefault(p => p.Value == position).Key;
                var playerName = player != null ? player.Username : "";
                teamList.AppendFormat("{0}: {1} ", position, playerName);
            }
            return teamList.ToString();
        }
    }
}
