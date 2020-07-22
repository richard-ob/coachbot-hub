namespace CoachBot.LegacyImporter.Data
{
    public static class TeamCodes
    {
        public static string GetTeamCode(string teamName)
        {
            switch (teamName)
            {
                case "Czechoslovakia IOS":
                    return "Czech";

                case "DO-T":
                    return "DOT";

                case "Excel":
                    return "xls";

                case "False 11":
                    return "F11";

                case "French Empire National Team IOS":
                    return "FRA";

                case "Hydra":
                    return "Hydra";

                case "International Space Station":
                    return "ISS";

                case "IOSoccer":
                    return "IOS";

                case "IOSoccer North America":
                    return "IOSNA";

                case "Iosoccer Nordic":
                    return "Nordic";

                case "ITALIA IOS":
                    return "ITA";

                case "JuicyFruits*":
                    return "JF";

                case "NextGen":
                    return "nG";

                case "Natural Talent":
                    return "NT";

                case "Slavič":
                    return "Slav";

                case "THC ҂ [ MultiGaming Team ]":
                    return "THC";

                case "Portugal IOS":
                    return "POR";

                case "Turkiye.IOS":
                    return "TUR";

                case "Unity":
                    return "Unity";

                case "xGoal":
                    return "xGoal";
            }

            return teamName;
        }
    }
}