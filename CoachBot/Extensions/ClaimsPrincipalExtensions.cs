using System.Linq;
using System.Security.Claims;

namespace CoachBot.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static ulong GetDiscordUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return ulong.Parse(claimsPrincipal.Claims.ToList().First().Value);
        }
    }
}
