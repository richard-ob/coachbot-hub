using CoachBot.Domain.Services;
using CoachBot.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : Controller
    {
        private readonly PositionService _positionService;

        public PositionController(PositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpGet]
        public IEnumerable<Position> Get()
        {
            return _positionService.GetPositions();
        }
    }
}
