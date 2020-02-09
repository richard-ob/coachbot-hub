using CoachBot.Domain.Model;
using CoachBot.Model;
using CoachBot.Tools;
using Discord;
using System.Linq;
using System.Text;

namespace CoachBot.Factories
{
    public static class TeamListEmbedFactory
    {
        private const uint DEFAULT_EMBED_HOME_TEAM_COLOUR = 0x2463b0;
        private const uint DEFAULT_EMBED_AWAY_TEAM_COLOUR = 0xd60e0e;

        public static Embed GenerateEmbed(Channel channel, Match match, TeamType teamType = TeamType.Home)
        {
            var sb = new StringBuilder();
            var teamColor = new Color(DEFAULT_EMBED_HOME_TEAM_COLOUR);
            var emptyPos = ":grey_question:";

            Team team;
            Channel oppositionChannel = null;
            if (teamType == TeamType.Home)
            {
                team = match.TeamHome;
                oppositionChannel = match.TeamAway?.Channel;
                teamColor = channel.SystemColor;
            }
            else
            {
                team = match.TeamAway;
                oppositionChannel = match.TeamHome?.Channel;
                if (match.IsMixMatch)
                {
                    teamColor = new Color(DEFAULT_EMBED_AWAY_TEAM_COLOUR);
                }
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

            return embedBuilder.WithColor(teamColor).WithDescription(sb.ToString()).WithCurrentTimestamp().WithRequestedBy().Build();
        }
    }
}
