using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.LegacyImporter.Data
{
    public static class BadgeEmote
    {
        public static string GetBadgeEmote(string teamName, string badgeEmote)
        {
            switch(teamName)
            {
                case "IOSoccer":
                    return "<:ios:455023721472720896>";
                case "CABRONES":
                    return "<:GOAT:732236286185832589>";
                case "Natural Talent":
                    return "<:NT:336649661286973450>";
                case "Portugal IOS":
                    return "<:POR:616341272264704010>";
                case "Slavič":
                    return "<:slavs:651834089472655380>";
                case "xGoal":
                    return "<:xgoal:715570220940656807>";
                case "Dreamsent":
                    return "<:dreamsent:737348216277958676>";
            }

            return badgeEmote;
        }
    }
}
