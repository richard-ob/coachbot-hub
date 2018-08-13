﻿using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Linq;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly BotService _botService;
        private readonly StatisticsService _statisticsService;

        public UserController(BotService botService, StatisticsService statisticsService)
        {
            _botService = botService;
            _statisticsService = statisticsService;
        }

        [HttpGet]
        [Authorize]
        public User Get()
        {
            var user = new User();
            if(User.Claims.Any())
            {
                user.Name = User.Identity.Name;
                user.DiscordUserId = ulong.Parse(User.Claims.First().Value);
                user.IsAdministrator = _botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value));
                user.Channels = _botService.GetChannelsForUser(ulong.Parse(User.Claims.First().Value), false, false);
            }
            return user;
        }

        [HttpGet("statistics")]
        [Authorize]
        public UserStatistics GetUserStatistics()
        {
            var userId = ulong.Parse(User.Claims.First().Value);
            var user = new UserStatistics() {
                Appearances = _statisticsService.MatchHistory.Count(a => a.Players.Any(p => p.DiscordUserId == userId)),
                FirstAppearance = _statisticsService.MatchHistory.Where(a => a.Players.Any(p => p.DiscordUserId == userId)).OrderBy(m => m.MatchDate).FirstOrDefault().MatchDate,
                LastAppearance = _statisticsService.MatchHistory.Where(a => a.Players.Any(p => p.DiscordUserId == userId)).OrderByDescending(m => m.MatchDate).FirstOrDefault().MatchDate,
                Rank = 10
            };

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
