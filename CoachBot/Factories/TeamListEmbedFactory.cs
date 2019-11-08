using CoachBot.Domain.Model;
using CoachBot.Extensions;
using CoachBot.Model;
using Discord;
using System.Linq;
using System.Text;

namespace CoachBot.Factories
{
    public class TeamListEmbedFactory
    {
        private readonly Channel channel;
        private readonly Match match;
        private readonly TeamType teamType;

        public TeamListEmbedFactory(Channel channel, Match match, TeamType teamType = TeamType.Home) {
            this.channel = channel;
            this.match = match;
            this.teamType = teamType;
        }

        public Embed GenerateEmbed()
        {
            var sb = new StringBuilder();
            var teamColor = new Color(0x2463b0);
            var emptyPos = ":grey_question:";

            var team = teamType == TeamType.Home ? match.TeamHome : match.TeamAway;

            if (match.IsMixMatch && teamType == TeamType.Away)
            {
                teamColor = new Color(0xd60e0e);
            }
            else if(channel.Color != null && channel.Color[0] == '#')
            {
                teamColor = new Color(ColorExtensions.FromHex(channel.Color).R, ColorExtensions.FromHex(channel.Color).G, ColorExtensions.FromHex(channel.Color).B);
            }            

            var embedBuilder = new EmbedBuilder().WithTitle($"{channel.BadgeEmote ?? channel.Name}{(match.IsMixMatch && teamType == TeamType.Away ? " #2" : "")} Team List");
            foreach (var channelPosition in channel.ChannelPositions)
            {
                var playerTeamPosition = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channelPosition.Position.Name);
                var playerName = playerTeamPosition?.Player.DiscordUserMention ?? playerTeamPosition?.Player.Name ?? emptyPos;
                sb.Append($"{channelPosition.Position.Name}:**{playerName}** ");
            }

            if (team.PlayerSubstitutes.Any()) sb.Append($"*Subs*: **{string.Join(", ", team.PlayerSubstitutes.Select(ps => ps.Player.DiscordUserMention ?? ps.Player.Name))}**");

            if (match.TeamAway?.ChannelId != null && match.TeamAway.ChannelId > 0 && !match.IsMixMatch)
            {
                sb.AppendLine("");
                sb.Append($"vs {channel.Name}");
            }

            return embedBuilder.WithColor(teamColor).WithDescription(sb.ToString()).WithCurrentTimestamp().Build();
        }
    }
}
