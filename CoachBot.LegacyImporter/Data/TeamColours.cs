using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.LegacyImporter.Data
{
    public static class TeamColours
    {
        public static string GetColour(string teamName, string teamColour)
        {
            switch (teamName)
            {
                case "Natural Talent":
                    return "#e5ac0e";
                case "Dark SunRise":
                    return "#010101";
                case "Cryptic":
                    return "#18729a";
                case "Revolution":
                    return "#c70830";
                case "Tempest":
                    return "#1c9347";
                case "IOSoccer":
                    return "#124364";
                case "Portugal IOS":
                    return "#9c0a0a";
                case "NextGen":
                    return "#532580";
                case "Dreamsent":
                    return "#941273";
            }

            return teamColour;
        }
    }
}
