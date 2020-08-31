﻿using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Services;
using CoachBot.Models;
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

        public MatchController(MatchService matchService, MatchStatisticsService matchStatisticsService)
        {
            _matchService = matchService;
            _matchStatisticsService = matchStatisticsService;
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

        [HubRolePermission(HubRole = PlayerHubRole.Manager)]
        [HttpPut("{id}")]
        public void UpdateMatch([FromBody]Match match)
        {
            _matchService.UpdateMatch(match);
        }

        [HttpGet("{id}/player-of-the-match")]
        public PlayerOfTheMatchStatistics GetPlayerOfTheMatch(int id)
        {
            return _matchStatisticsService.GetPlayerOfTheMatch(id);
        }
    }
}