using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Models;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class AnnouncementController : Controller
    {
        private readonly AnnouncementService _announcementService;

        public AnnouncementController(AnnouncementService announcementService, BotService botService)
        {
            _announcementService = announcementService;
        }

        [HubRolePermission(HubRole = PlayerHubRole.Owner)]
        [HttpPost]
        public async void SendAnnouncement([FromBody]AnnouncementDto announcementDto)
        {
            await _announcementService.SendGlobalMessage(announcementDto.Title, announcementDto.Message, announcementDto.RegionId);
        }
    }

}
