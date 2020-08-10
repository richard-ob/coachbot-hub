using System.Text.RegularExpressions;

namespace CoachBot.Shared.Helpers
{
    public static class ServerAddressHelper
    {
        public static bool IsValidIpAddress(string ip)
        {
            var isMatch = Regex.Match(ip, @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]):[0-9]+$");

            return isMatch.Success;
        }

        public static bool IsValidIpAddressWithoutPort(string ip)
        {
            var isMatch = Regex.Match(ip, @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");

            return isMatch.Success;
        }

        public static bool IsValidHostname(string hostname)
        {
            var isMatch = Regex.Match(hostname, @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9]):[0-9]+$");

            return isMatch.Success;
        }

        public static bool IsValidAddress(string address)
        {
            return IsValidIpAddress(address) || IsValidHostname(address);
        }
    }
}
