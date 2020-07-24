using CoachBot.Domain.Model;
using System.Linq;

namespace CoachBot.LegacyImporter
{
    public static class TeamTypes
    {
        public static TeamType GetTeamTypeForTeam(string guildName)
        {
            if (ClubTeams.Any(t => t == guildName))
            {
                return TeamType.Club;
            }
            if (DraftTeams.Any(t => t == guildName))
            {
                return TeamType.Draft;
            }
            if (NationalTeams.Any(t => t == guildName))
            {
                return TeamType.National;
            }

            return TeamType.Mix;
        }

        public static string[] ClubTeams => new string[]
        {
            // EU
            "Unity",
            "Natural Talent",
            "NextGen",
            "Excel",
            "International Space Station",
            "Passing Makes Perfect",
            "Hydra",
            "ArrowNewEra",
            "False 11",
            "THC ҂ [ MultiGaming Team ]",
            "Alliance",
            "Tempest",
            "xGoal",
            "ProSoccer", // Exclude
            "Wasps FC", // Exclude
            "CABRONES",
            "Cryptic",
            "Revolution",
            "Dark SunRise"
        };

        public static string[] DraftTeams => new string[]
        {
            // EU
            "Chelsea IOS - Janir's team",
            "Atlético De Madrid",
            "Olympique Lyonnais",
            "Barcelona",
            "Sport Lisboa e Benfica",
            "IOS Draft Arsenal",
            "Borussia Dortmund",
            "Bayern Munich",
            "Draft Team",
            "Camp Nou | Barca",
            "CITY OF MANCHESTER",
            "Sporting Lisbon",

            // SA

            // NA
            "-unlimited Potential-",
            "digitalStrikers"
        };

        public static string[] NationalTeams => new string[]
        {
            // EU
            "IOS FRANCE **",
            "Iosoccer Nordic",
            "Portugal IOS",
            "Turkiye.IOS",
            "ITALIA IOS",
            "Slavič",
            "Rest of the People",
            "Rest of the World IOS",
            "IOS ESPAÑA",
            "French Empire National Team IOS",
            "Czechoslovakia IOS"
        };

        public static string[] MixTeams => new string[]
        {
            // EU
            "IOSoccer",
            "Fluffy Ballz",
            "Baldies FC",
            "𝓒 𝓐 𝓢 𝓣 𝓔 𝓛 𝓘 𝓝",
            "JuicyFruits*",
            "Jiggy Talent",
            "MIX PLAYERS",
            "Tyralnia DODA",
            "DO-T",
            "EUNE Championship",
            "OG Momentum",
            "Wasps FC",
            "The Wasps Nest aka The Champs",

            // SA
            "IOSoccer Sudamerica",

            // NA
            "IOSoccer North America",

            // Korea
            "IOS League"
        };

        public static string[] OtherTeams => new string[]
        {
            // EU
            "Iosocer 1v1  CUP",

            // SA
            "Ball2D Sudamerica"
        };
    }
}