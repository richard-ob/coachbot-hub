using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.LegacyImporter.Data
{
    public static class TeamBadges
    {
        public static string GetBadgeImageUrl(ITextChannel discordChannel)
        {
            switch (discordChannel.Guild.Name)
            {
                case "NextGen":
                    return "http://www.iosoccer.co.uk/ng-badge.png";
                case "Cryptic":
                    return "http://www.iosoccer.co.uk/cryptic-badge.png";
                case "French Empire National Team IOS":
                    return "https://upload.wikimedia.org/wikipedia/en/thumb/1/12/France_national_football_team_seal.svg/188px-France_national_football_team_seal.svg.png";
            }

            return discordChannel.Guild.IconUrl.Replace(".jpg", ".png") + "?size=512";
        }
    }
}
