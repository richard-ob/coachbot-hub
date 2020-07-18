using System.Text.RegularExpressions;

namespace CoachBot.Domain.Helpers
{
    public static class SteamIdHelper
    {
        public static ulong? ConvertSteamIDToSteamID64(string steamId)
        {
            var match = Regex.Match(steamId, @"^STEAM_[0-5]:[01]:\d+$", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                return null;
            }

            var split = steamId.Split(":");

            var v = (ulong)76561197960265728;
            var y = ulong.Parse(split[1]);
            var z = ulong.Parse(split[2]);

            var w = (z * 2) + v + y;

            return w;
        }
    }
}