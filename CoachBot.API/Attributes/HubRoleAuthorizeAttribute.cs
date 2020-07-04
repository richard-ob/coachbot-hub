using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoachBot.Attributes
{
    internal class HubRoleAuthorizeAttribute
    {
        public class HubRolePermission : AuthorizeAttribute, IAuthorizationFilter
        {
            public PlayerHubRole HubRole { get; set; }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var steamId = context.HttpContext.User.GetSteamId();

                if (steamId <= 0)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var playerService = context.HttpContext.RequestServices.GetService(typeof(PlayerService)) as PlayerService;
                var playerHubRole = playerService.GetPlayerHubRole(steamId);

                if (HubRole.Equals(PlayerHubRole.Administrator) && playerHubRole >= PlayerHubRole.Administrator)
                {
                    return;
                }
                else if (HubRole.Equals(PlayerHubRole.Manager) && playerHubRole >= HubRole)
                {
                    return;
                }

                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}