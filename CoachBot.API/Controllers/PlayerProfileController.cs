using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/player-profiles")]
    [ApiController]
    public class PlayerProfileController : Controller
    {
        private readonly PlayerProfileService _playerProfileService;

        public PlayerProfileController(PlayerProfileService playerProfileService)
        {
            _playerProfileService = playerProfileService;
        }

        [HttpGet("{id}")]
        public PlayerProfile Get(int id)
        {
            return _playerProfileService.GeneratePlayerProfile(id);
        }
    }
}
