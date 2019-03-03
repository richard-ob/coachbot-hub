using CoachBot.Model;
using Discord;
using System.Linq;

namespace CoachBot.Bot.Tools
{
    public static class FormationEmbed
    {
        public static Embed Generate(EmbedBuilder builder, string availablePlaceholderText, Team team, Channel channel)
        {
            var positionCount = channel.Positions.Count();
            if (positionCount == 8 && channel.Formation == Formation.ThreeThreeOne)
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
            else if (positionCount == 8 && channel.Formation == Formation.ThreeTwoTwo)
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
            else if (positionCount == 8 && channel.Formation == Formation.ThreeOneTwoOne)
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
            else if (positionCount == 8 && channel.Formation == Formation.ThreeOneThree)
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
            else if (positionCount == 4 && channel.Formation == Formation.TwoOne)
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
                if (positionCount % 3 == 2) // Ensure that two-column fields are three-columns to ugly alignment
                {
                    builder.AddInlineField("\u200B", "\u200B");
                }
            }

            return builder;
        }

        private static string AddPrefix(string position)
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
