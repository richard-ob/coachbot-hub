using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Model;
using CoachBot.Models;
using CoachBot.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PlayerController : Controller
    {
        private readonly PlayerService _playerService;
        private readonly MatchStatisticsService _matchStatisticsService;

        public PlayerController(PlayerService playerService, MatchStatisticsService matchStatisticsService)
        {
            _playerService = playerService;
            _matchStatisticsService = matchStatisticsService;
        }

        [HttpGet("{id}")]
        public Player Get(int id)
        {
            return _playerService.GetPlayer(id);
        }

        [HttpGet]
        [Route("@me")]
        public Player Get()
        {
            ulong steamId;
            
            try
            {
                steamId = User.GetSteamId();
            }
            catch
            {
                return null;
            }

            return _playerService.GetPlayerBySteamId(steamId, createIfNotExists: true, playerName: User.Identity.Name);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Player)]
        [HttpPost]
        [Route("@me")]
        public IActionResult UpdatePlayerProfile([FromBody]Player playerProfileUpdateDto)
        {
            var player = _playerService.GetPlayerBySteamId(User.GetSteamId());

            if (player == null)
            {
                return NotFound();
            }

            // TODO: Switch to AutoMapper
            player.Name = playerProfileUpdateDto.Name;
            player.CountryId = playerProfileUpdateDto.CountryId;
            player.DisableDMNotifications = playerProfileUpdateDto.DisableDMNotifications;
            player.PlayingSince = playerProfileUpdateDto.PlayingSince;
            // player.Positions = playerProfileUpdateDto.Positions.ToList(); // TODO: Decide if to revive this

            _playerService.UpdatePlayer(player);

            return Ok();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost]
        [Route("update-rating")]
        public IActionResult UpdatePlayerRating([FromBody]Player playerRatingUpdateDto)
        {
            var player = _playerService.GetPlayer(playerRatingUpdateDto.Id);

            if (player == null)
            {
                return NotFound();
            }

            // TODO: Switch to AutoMapper
            player.Rating = playerRatingUpdateDto.Rating;

            _playerService.UpdatePlayer(player);

            return Ok();
        }

        [HttpPost]
        public PagedResult<Player> PagedMatchList([FromBody]PagedPlayerRequestDto pagedRequest)
        {
            return _playerService.GetPlayers(pagedRequest.Page, pagedRequest.PageSize, pagedRequest.SortOrderFull);
        }

        [HttpGet("{playerId}/team-history")]
        public List<PlayerTeamStatisticsTotals> GetPlayerTeamStatisticsHistory(int playerId)
        {
            return _matchStatisticsService.GetPlayerTeamStatistics(playerId);
        }

        [HttpGet("search")]
        public List<Player> SearchFor(string playerName)
        {
            return _playerService.SearchPlayersByName(playerName);
        }
    }
}