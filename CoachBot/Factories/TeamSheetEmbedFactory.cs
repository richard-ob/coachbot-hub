using CoachBot.Domain.Model;
using CoachBot.Extensions;
using CoachBot.Model;
using Discord;
using System.Linq;
using System.Text;

namespace CoachBot.Factories
{
    public static class TeamSheetEmbedFactory
    {
        private const uint DEFAULT_EMBED_TEAM_COLOUR = 0x2463b0;

        public static Embed GenerateEmbed(Channel channel, Match match, TeamType teamType = TeamType.Home)
        {
            var teamColor = new Color(DEFAULT_EMBED_TEAM_COLOUR);
            var teamList = new StringBuilder();
            var embedFooterBuilder = new EmbedFooterBuilder();
            var availablePlaceholderText = !string.IsNullOrEmpty(channel.KitEmote) && teamType == TeamType.Home ? channel.KitEmote : "<:redshirt:318114878063902720>";
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

            var builder = new EmbedBuilder()
                           .WithTitle($"{team.Channel?.BadgeEmote ?? channel.Name} Team Sheet")
                           .WithCurrentTimestamp()
                           .WithColor(teamColor);

            if (!match.IsMixMatch && oppositionChannel != null)
            {
                builder.WithDescription($"vs {oppositionChannel.Name}");
            }

            if (channel.ChannelPositions.Count() == 8 && channel.Formation == Formation.ThreeThreeOne)
            {
                builder.AddInlineField("\u200B", "\u200B");
                var player8 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[7].Position.Name);
                builder.AddInlineField(player8 != null ? player8.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[7].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player7 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[6].Position.Name);
                builder.AddInlineField(player7 != null ? player7.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[6].Position.Name));
                var player6 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[5].Position.Name);
                builder.AddInlineField(player6 != null ? player6.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[5].Position.Name));
                var player5 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[4].Position.Name);
                builder.AddInlineField(player5 != null ? player5.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[4].Position.Name));
                var player4 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[3].Position.Name);
                builder.AddInlineField(player4 != null ? player4.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[3].Position.Name));
                var player3 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[2].Position.Name);
                builder.AddInlineField(player3 != null ? player3.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[2].Position.Name));
                var player2 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[1].Position.Name);
                builder.AddInlineField(player2 != null ? player2.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[1].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player1 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[0].Position.Name);
                builder.AddInlineField(player1 != null ? player1.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[0].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
            }
            else if (channel.ChannelPositions.Count() == 8 && channel.Formation == Formation.ThreeTwoTwo)
            {
                var player8 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[7].Position.Name);
                builder.AddInlineField(player8 != null ? player8.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[7].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player7 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[6].Position.Name);
                builder.AddInlineField(player7 != null ? player7.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[6].Position.Name));
                var player6 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[5].Position.Name);
                builder.AddInlineField(player6 != null ? player6.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[5].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player5 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[4].Position.Name);
                builder.AddInlineField(player5 != null ? player5.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[4].Position.Name));
                var player4 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[3].Position.Name);
                builder.AddInlineField(player4 != null ? player4.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[3].Position.Name));
                var player3 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[2].Position.Name);
                builder.AddInlineField(player3 != null ? player3.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[2].Position.Name));
                var player2 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[1].Position.Name);
                builder.AddInlineField(player2 != null ? player2.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[1].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player1 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[0].Position.Name);
                builder.AddInlineField(player1 != null ? player1.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[0].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
            }
            else if (channel.ChannelPositions.Count() == 8 && channel.Formation == Formation.ThreeOneTwoOne)
            {
                builder.AddInlineField("\u200B", "\u200B");
                var player8 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[7].Position.Name);
                builder.AddInlineField(player8 != null ? player8.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[7].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player7 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[6].Position.Name);
                builder.AddInlineField(player7 != null ? player7.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[6].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player6 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[5].Position.Name);
                builder.AddInlineField(player6 != null ? player6.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[5].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player5 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[4].Position.Name);
                builder.AddInlineField(player5 != null ? player5.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[4].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player4 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[3].Position.Name);
                builder.AddInlineField(player4 != null ? player4.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[3].Position.Name));
                var player3 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[2].Position.Name);
                builder.AddInlineField(player3 != null ? player3.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[2].Position.Name));
                var player2 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[1].Position.Name);
                builder.AddInlineField(player2 != null ? player2.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[1].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player1 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[0].Position.Name);
                builder.AddInlineField(player1 != null ? player1.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[0].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
            }
            else if (channel.ChannelPositions.Count() == 8 && channel.Formation == Formation.ThreeOneThree)
            {
                var player8 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[7].Position.Name);
                builder.AddInlineField(player8 != null ? player8.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[7].Position.Name));
                var player7 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[6].Position.Name);
                builder.AddInlineField(player7 != null ? player7.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[6].Position.Name));
                var player6 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[5].Position.Name);
                builder.AddInlineField(player6 != null ? player6.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[5].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player5 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[4].Position.Name);
                builder.AddInlineField(player5 != null ? player5.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[4].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player4 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[3].Position.Name);
                builder.AddInlineField(player4 != null ? player4.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[3].Position.Name));
                var player3 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[2].Position.Name);
                builder.AddInlineField(player3 != null ? player3.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[2].Position.Name));
                var player2 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[1].Position.Name);
                builder.AddInlineField(player2 != null ? player2.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[1].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player1 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[0].Position.Name);
                builder.AddInlineField(player1 != null ? player1.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[0].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
            }
            else if (channel.ChannelPositions.Count() == 4 && channel.Formation == Formation.TwoOne)
            {
                builder.AddInlineField("\u200B", "\u200B");
                var player4 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[3].Position.Name);
                builder.AddInlineField(player4 != null ? player4.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[3].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player3 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[2].Position.Name);
                builder.AddInlineField(player3 != null ? player3.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[2].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player2 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[1].Position.Name);
                builder.AddInlineField(player2 != null ? player2.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[1].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
                var player1 = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.ToArray()[0].Position.Name);
                builder.AddInlineField(player1 != null ? player1.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.ToArray()[0].Position.Name));
                builder.AddInlineField("\u200B", "\u200B");
            }
            else
            {
                foreach (var position in channel.ChannelPositions)
                {
                    var player = team.PlayerTeamPositions.FirstOrDefault(p => p.Position.Name == position.Position.Name);
                    builder.AddInlineField(player != null ? player.Player.Name : availablePlaceholderText, AddNumericPrefix(position.Position.Name));
                }
                if (channel.ChannelPositions.Count() % 3 == 2) // Ensure that two-column fields are three-columns to ugly alignment
                {
                    builder.AddInlineField("\u200B", "\u200B");
                }
            }

            if (teamType == TeamType.Home && team.PlayerSubstitutes.Any()) builder.AddField("Subs", string.Join(", ", match.TeamHome.PlayerSubstitutes.Select(ps => ps.Player.DiscordUserMention ?? ps.Player.Name)));

            return builder.Build();
        }

        private static string AddNumericPrefix(string position)
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