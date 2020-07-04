namespace CoachBot.LegacyImporter.Data
{
    public static class ServersData
    {
        public static int GetCorrectServerCountry(int countryId, string serverName)
        {
            if (serverName.Contains("London"))
            {
                return 34; // GB
            }

            return countryId > 0 ? countryId : 1;
        }
    }
}