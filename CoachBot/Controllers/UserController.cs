using CoachBot.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        public UserController()
        {
        }

        [HttpGet]
        public User Get()
        {
            var user = new User();
            if(User.Claims.Any())
            {
                user.Name = User.Identity.Name;
            }
            return user;
        }

        [HttpGet("/login")]
        public IActionResult LogIn()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "http://localhost:4200" }, Discord.OAuth2.DiscordDefaults.AuthenticationScheme);
        }

        [HttpGet("/unauthorized")]
        public IActionResult AccessDenied()
        {
            return new UnauthorizedResult();
        }
    }
}
