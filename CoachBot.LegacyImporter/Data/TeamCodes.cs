namespace CoachBot.LegacyImporter.Data
{
    public static class TeamCodes
    {
        public static string GetTeamCode(string teamName)
        {
            switch (teamName)
            {
                case "THC ҂ [ MultiGaming Team ]":
                    return "THC";

                case "Natural Talent":
                    return "NT";

                case "Turkiye.IOS":
                    return "TUR";

                case "IOSoccer":
                    return "IOS";

                case "IOSoccer North America":
                    return "IOSNA";

                case "ITALIA IOS":
                    return "ITA";

                case "Portugal IOS":
                    return "POR";

                case "Unity":
                    return "Unity";

                case "NextGen":
                    return "nG";

                case "Slavič":
                    return "Slav";

                case "Hydra":
                    return "Hydra";

                case "French Empire National Team IOS":
                    return "FRA";

                case "JuicyFruits*":
                    return "JF";

                case "xGoal":
                    return "xGoal";

                case "DO-T":
                    return "DOT";

                case "Iosoccer Nordic":
                    return "Nordic";

                case "___________":
                    return "________";
            }

            return teamName;
        }
    }
}