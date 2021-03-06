﻿using CoachBot.Domain.Model;
using CoachBot.Extensions;
using CoachBot.Model;
using CoachBot.Shared.Extensions;
using Discord;
using System.Linq;
using System.Text;

namespace CoachBot.Factories
{
    public static class TeamSheetEmbedFactory
    {
        private const uint DEFAULT_EMBED_HOME_TEAM_COLOUR = 0x2463b0;
        private const uint DEFAULT_EMBED_AWAY_TEAM_COLOUR = 0xd60e0e;
        private const string DEFAULT_KIT_EMOTE = "<:redshirt:318114878063902720>";
        private const string UNICODE_SPACE = "\u200B";

        public static Embed GenerateEmbed(Channel channel, Matchup matchup, MatchTeamType teamType = MatchTeamType.Home)
        {
            var teamList = new StringBuilder();
            var teamColor = new Color(DEFAULT_EMBED_HOME_TEAM_COLOUR);
            var embedFooterBuilder = new EmbedFooterBuilder();
            var availablePlaceholderText = !string.IsNullOrEmpty(channel.Team.KitEmote) && teamType == MatchTeamType.Home ? channel.Team.KitEmote : DEFAULT_KIT_EMOTE;

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
            }

            if (teamType == MatchTeamType.Home && channel.Team.Color != null && channel.Team.Color[0] == '#')
            {
                teamColor = new Color(ColorExtensions.FromHex(channel.Team.Color).R, ColorExtensions.FromHex(channel.Team.Color).G, ColorExtensions.FromHex(channel.Team.Color).B);
            }

            var builder = new EmbedBuilder()
                           .WithTitle($"{team.Channel?.Team.BadgeEmote ?? channel.Team.Name} Team Sheet")
                           .WithCurrentTimestamp()
                           .WithColor(teamColor);

            if (!matchup.IsMixMatch && oppositionTeam?.Channel != null)
            {
                var oppositionInfo = $"vs {oppositionTeam.Channel.Team.Name}";
                if (!oppositionTeam.HasGk) oppositionInfo += " ***No GK***";
                builder.WithDescription(oppositionInfo);
            }

            if (channel.ChannelPositions.Count() == 8 && channel.Formation == Formation.ThreeThreeOne)
            {
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player8 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(7).Position.Name);
                builder.AddField(player8 != null ? player8.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(7).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player7 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(6).Position.Name);
                builder.AddField(player7 != null ? player7.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(6).Position.Name));
                var player6 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(5).Position.Name);
                builder.AddField(player6 != null ? player6.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(5).Position.Name));
                var player5 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(4).Position.Name);
                builder.AddField(player5 != null ? player5.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(4).Position.Name));
                var player4 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(3).Position.Name);
                builder.AddField(player4 != null ? player4.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(3).Position.Name));
                var player3 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(2).Position.Name);
                builder.AddField(player3 != null ? player3.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(2).Position.Name));
                var player2 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(1).Position.Name);
                builder.AddField(player2 != null ? player2.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(1).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player1 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(0).Position.Name);
                builder.AddField(player1 != null ? player1.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(0).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
            }
            else if (channel.ChannelPositions.Count() == 8 && channel.Formation == Formation.ThreeTwoTwo)
            {
                var player8 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(7).Position.Name);
                builder.AddField(player8 != null ? player8.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(7).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player7 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(6).Position.Name);
                builder.AddField(player7 != null ? player7.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(6).Position.Name));
                var player6 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(5).Position.Name);
                builder.AddField(player6 != null ? player6.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(5).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player5 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(4).Position.Name);
                builder.AddField(player5 != null ? player5.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(4).Position.Name));
                var player4 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(3).Position.Name);
                builder.AddField(player4 != null ? player4.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(3).Position.Name));
                var player3 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(2).Position.Name);
                builder.AddField(player3 != null ? player3.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(2).Position.Name));
                var player2 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(1).Position.Name);
                builder.AddField(player2 != null ? player2.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(1).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player1 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(0).Position.Name);
                builder.AddField(player1 != null ? player1.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(0).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
            }
            else if (channel.ChannelPositions.Count() == 8 && channel.Formation == Formation.ThreeOneTwoOne)
            {
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player8 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(7).Position.Name);
                builder.AddField(player8 != null ? player8.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(7).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player7 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(6).Position.Name);
                builder.AddField(player7 != null ? player7.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(6).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player6 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(5).Position.Name);
                builder.AddField(player6 != null ? player6.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(5).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player5 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(4).Position.Name);
                builder.AddField(player5 != null ? player5.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(4).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player4 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(3).Position.Name);
                builder.AddField(player4 != null ? player4.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(3).Position.Name));
                var player3 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(2).Position.Name);
                builder.AddField(player3 != null ? player3.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(2).Position.Name));
                var player2 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(1).Position.Name);
                builder.AddField(player2 != null ? player2.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(1).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player1 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(0).Position.Name);
                builder.AddField(player1 != null ? player1.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(0).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
            }
            else if (channel.ChannelPositions.Count() == 8 && channel.Formation == Formation.ThreeOneThree)
            {
                var player8 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(7).Position.Name);
                builder.AddField(player8 != null ? player8.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(7).Position.Name));
                var player7 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(6).Position.Name);
                builder.AddField(player7 != null ? player7.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(6).Position.Name));
                var player6 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(5).Position.Name);
                builder.AddField(player6 != null ? player6.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(5).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player5 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(4).Position.Name);
                builder.AddField(player5 != null ? player5.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(4).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player4 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(3).Position.Name);
                builder.AddField(player4 != null ? player4.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(3).Position.Name));
                var player3 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(2).Position.Name);
                builder.AddField(player3 != null ? player3.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(2).Position.Name));
                var player2 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(1).Position.Name);
                builder.AddField(player2 != null ? player2.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(1).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player1 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(0).Position.Name);
                builder.AddField(player1 != null ? player1.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(0).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
            }
            else if (channel.ChannelPositions.Count() == 4 && channel.Formation == Formation.TwoOne)
            {
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player4 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(3).Position.Name);
                builder.AddField(player4 != null ? player4.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(3).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player3 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(2).Position.Name);
                builder.AddField(player3 != null ? player3.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(2).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player2 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(1).Position.Name);
                builder.AddField(player2 != null ? player2.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(1).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                var player1 = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(0).Position.Name);
                builder.AddField(player1 != null ? player1.Player.Name : availablePlaceholderText, AddNumericPrefix(channel.ChannelPositions.OrderBy(cp => cp.Ordinal).ElementAt(0).Position.Name));
                builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
            }
            else
            {
                foreach (var position in channel.ChannelPositions.OrderBy(cp => cp.Ordinal))
                {
                    var player = team.PlayerLineupPositions.FirstOrDefault(p => p.Position.Name == position.Position.Name);
                    builder.AddField(player != null ? player.Player.Name : availablePlaceholderText, AddNumericPrefix(position.Position.Name));
                }
                if (channel.ChannelPositions.Count() % 3 == 2) // Ensure that two-column fields are three-columns to ugly alignment
                {
                    builder.AddField(UNICODE_SPACE, UNICODE_SPACE);
                }
            }

            if (teamType == MatchTeamType.Home && team.PlayerSubstitutes.Any()) builder.AddField("Subs", string.Join(", ", matchup.LineupHome.PlayerSubstitutes.Select(ps => ps.Player.DiscordUserMention ?? ps.Player.Name)));

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