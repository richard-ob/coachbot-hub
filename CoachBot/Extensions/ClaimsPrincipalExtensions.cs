using System;
using System.Linq;
using System.Security.Claims;

namespace CoachBot.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static ulong GetSteamId(this ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("No Steam ID claim found");
            }

            var steamId64 = ulong.Parse(userIdClaim.Value.Split("/").Last());

            if (steamId64 == 0)
            {
                throw new UnauthorizedAccessException("Invalid Steam ID claim");
            }

            return steamId64;
        }

        public static ulong GetDiscordUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return ulong.Parse(claimsPrincipal.Claims.ToList().First().Value);
        }
    }
}
