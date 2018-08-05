using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachBot.Controllers
{

    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class AnnouncementController : Controller
    {
        private readonly AnnouncementService _announcementService;

        public AnnouncementController(AnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        [HttpPost]
        public void Say([FromBody]ChatMessage message)
        {
            _announcementService.Say(message.Message);
        }
    }

}
