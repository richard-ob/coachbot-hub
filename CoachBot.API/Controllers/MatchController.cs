using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Services;
using CoachBot.Models;
using CoachBot.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MatchController : Controller
    {
        private readonly MatchService _matchService;
        private readonly MatchStatisticsService _matchStatisticsService;
        private readonly TournamentService _tournamentService;
        private readonly PlayerService _playerService;

        public MatchController(MatchService matchService, MatchStatisticsService matchStatisticsService, TournamentService tournamentService, PlayerService playerService)
        {
            _matchService = matchService;
            _matchStatisticsService = matchStatisticsService;
            _tournamentService = tournamentService;
            _playerService = playerService;
        }

        [HttpGet("{id}")]
        public Match Get(int id)
        {
            return _matchService.GetMatch(id);
        }

        [HttpPost]
        public PagedResult<Match> PagedMatchList([FromBody]PagedMatchRequestDto pagedRequest)
        {
            return _matchService.GetMatches(pagedRequest.Page, pagedRequest.PageSize, pagedRequest.SortOrderFull, pagedRequest.Filters);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMatch([FromBody]Match match, int id)
        {
            var matchToUpdate = _matchService.GetMatch(id);
            var hasHubAccess = !_playerService.IsAdminOrOwner(User.GetSteamId());
            if (matchToUpdate.TournamentId.HasValue)
            {
                if (!_tournamentService.IsTournamentOrganiser((int)matchToUpdate.TournamentId, User.GetSteamId()) && !hasHubAccess)
                {
                    return Unauthorized();
                }
            }
            else if (!hasHubAccess)
            {
                return Unauthorized();
            }

            _matchService.UpdateMatch(match);

            return Ok();
        }

        [HttpGet("{id}/player-of-the-match")]
        public PlayerOfTheMatchStatistics GetPlayerOfTheMatch(int id)
        {
            return _matchStatisticsService.GetPlayerOfTheMatch(id);
        }
    }
}