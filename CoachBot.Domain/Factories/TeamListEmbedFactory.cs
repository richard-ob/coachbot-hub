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

        public static Embed GenerateEmbed(Channel channel, Matchup matchup, MatchTeamType teamType = MatchTeamType.Home)
        {
            var sb = new StringBuilder();
            var teamColor = new Color(DEFAULT_EMBED_HOME_TEAM_COLOUR);
            var emptyPos = ":grey_question:";

            Lineup team;
            Lineup oppositionTeam;
            if (teamType == MatchTeamType.Home)
            {
                team = matchup.LineupHome;
                oppositionTeam = matchup.LineupAway;
                teamColor = channel.Team.SystemColor;
            }
            else
            {
                team = matchup.LineupAway;
                oppositionTeam = matchup.LineupHome;
                if (matchup.IsMixMatch)
                {
                    teamColor = new Color(DEFAULT_EMBED_AWAY_TEAM_COLOUR);
                }
                else
                {
                    teamColor = channel.Team.SystemColor;
                }
            }

            var embedBuilder = new EmbedBuilder().WithTitle($"{channel.Team.BadgeEmote ?? channel.Team.Name}{(matchup.IsMixMatch && teamType == MatchTeamType.Away ? " #2" : "")} Team List");
            foreach (var channelPosition in channel.ChannelPositions.OrderBy(cp => cp.Ordinal))
            {
                var playerTeamPosition = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channelPosition.Position.Name);
                var playerName = playerTeamPosition?.Player.DiscordUserMention ?? playerTeamPosition?.Player.Name;
                if (string.IsNullOrEmpty(playerName))
                {
                    sb.Append($"{channelPosition.Position.Name}:{emptyPos} ");
                }
                else
                {
                    sb.Append($"{channelPosition.Position.Name}:**{playerName}** ");
                }
            }

            if (team.PlayerSubstitutes.Any()) sb.Append($"*Subs*: **{string.Join(", ", team.PlayerSubstitutes.Select(ps => ps.Player.DiscordUserMention ?? ps.Player.Name))}**");

            if (!matchup.IsMixMatch && oppositionTeam?.Channel != null)
            {
                sb.AppendLine("");
                sb.Append($"vs **{oppositionTeam.Channel.Team.DisplayName}**");
                if (!oppositionTeam.HasGk) sb.Append(" ***No GK***");
            }

            return embedBuilder.WithColor(teamColor).WithDescription(sb.ToString()).WithCurrentTimestamp().WithRequestedBy().Build();
        }
    }
}