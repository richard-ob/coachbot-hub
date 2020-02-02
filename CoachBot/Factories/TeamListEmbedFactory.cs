using CoachBot.Domain.Model;
using CoachBot.Extensions;
using CoachBot.Model;
using Discord;
using System.Linq;
using System.Text;

namespace CoachBot.Factories
{
    public static class TeamListEmbedFactory
    {
        private const uint DEFAULT_EMBED_TEAM_COLOUR = 0x2463b0;

        public static Embed GenerateEmbed(Channel channel, Match match, TeamType teamType = TeamType.Home)
        {
            var sb = new StringBuilder();
            var teamColor = new Color(DEFAULT_EMBED_TEAM_COLOUR);
            var emptyPos = ":grey_question:";

            Team team;
            Channel oppositionChannel = null;
            if (match.IsMixMatch && teamType == TeamType.Home)
            {
                team = match.TeamHome;
            }
            else if (teamType == TeamType.Away)
            {
                team = match.TeamAway;
                teamColor = new Color(0xd60e0e);
            }
            else if (match.TeamAway?.ChannelId == channel.Id)
            {
                team = match.TeamAway;
                oppositionChannel = match.TeamHome?.Channel;
            }
            else
            {
                team = match.TeamHome;
                oppositionChannel = match.TeamAway?.Channel;
            }

            if (teamType == TeamType.Home && channel.Color != null && channel.Color[0] == '#')
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

            if (!match.IsMixMatch && oppositionChannel != null)
            {
                sb.AppendLine("");
                sb.Append($"vs {oppositionChannel.Name}");
            }

            return embedBuilder.WithColor(teamColor).WithDescription(sb.ToString()).WithCurrentTimestamp().Build();
        }
    }
}
