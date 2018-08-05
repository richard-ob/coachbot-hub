using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CoachBot.Controllers
{

    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class AnnouncementController : Controller
    {
        private readonly AnnouncementService _announcementService;
        private readonly BotService _botService;

        public AnnouncementController(AnnouncementService announcementService, BotService botService)
        {
            _announcementService = announcementService;
            _botService = botService;
        }

        [HttpPost]
        public void Say([FromBody]ChatMessage message)
        {
            if (!_botService.UserIsOwningGuildAdmin(ulong.Parse(User.Claims.First().Value)))
            {
                throw new Exception();
            }
            _announcementService.Say(message.Message);
        }
    }

}
