﻿using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Models;
using CoachBot.Models.Dto;
using CoachBot.Shared.Extensions;
using CoachBot.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/match-statistics")]
    [ApiController]
    [AllowAnonymous]
    public class MatchStatisticController : Controller
    {
        private readonly MatchService _matchService;
        private readonly MatchStatisticsService _matchStatisticsService;
        private readonly PlayerService _playerService;
        private readonly TournamentService _tournamentService;

        public MatchStatisticController(MatchService matchService, MatchStatisticsService matchStatisticsService, PlayerService playerService, TournamentService tournamentService)
        {
            _matchService = matchService;
            _matchStatisticsService = matchStatisticsService;
            _playerService = playerService;
            _tournamentService = tournamentService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Submit(MatchStatisticsDto matchStatisticsDto)
        {
            var base64EncodedBytes = Convert.FromBase64String(matchStatisticsDto.Access_Token);
            var token = Encoding.UTF8.GetString(base64EncodedBytes);
            var serverAddress = token.Split("_")[0];
            var matchId = int.Parse(token.Split("_")[1]);
            var homeTeamCode = token.Split("_")[2];
            var awayTeamCode = token.Split("_")[3];
            var match = _matchService.GetMatch(matchId);
            var sourceAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
            var map = matchStatisticsDto.MatchData.MatchInfo.MapName;

            if (matchStatisticsDto.MatchData.Players.Count < 6)
            {
                return BadRequest();
            }

            if (matchStatisticsDto.MatchData.Players.Any(p => p.Info.SteamId == "BOT" && p.Info.Name.ToLower().Contains("bot")))
            {
                return BadRequest();
            }

            if (matchStatisticsDto.MatchData.MatchInfo.Periods == 1)
            {
                return BadRequest();
            }

            if (!string.IsNullOrWhiteSpace(map) && int.TryParse(map[0].ToString(), out int matchFormat) && matchFormat > matchStatisticsDto.MatchData.Players.Count)
            {
                return BadRequest();
            }

            if (match == null)
            {
                _matchStatisticsService.SaveUnlinkedMatchData(matchStatisticsDto.MatchData, matchStatisticsDto.Access_Token, sourceAddress);
                return BadRequest();
            }

            if (match.TeamHome.TeamCode != homeTeamCode || match.TeamAway.TeamCode != awayTeamCode)
            {
                _matchStatisticsService.SaveUnlinkedMatchData(matchStatisticsDto.MatchData, matchStatisticsDto.Access_Token, sourceAddress);

                return NoContent();
            }

            if (serverAddress == null)
            {
                _matchStatisticsService.SaveUnlinkedMatchData(matchStatisticsDto.MatchData, matchStatisticsDto.Access_Token, sourceAddress);

                return NoContent();
            }

            if (ServerAddressHelper.IsValidIpAddress(match.Server.Address) && ServerAddressHelper.IsValidIpAddressWithoutPort(sourceAddress) && match.Server.Address.Split(".")[0] != sourceAddress.Split(".")[0]) // INFO: We only compare the first group of the IP, as the IP may not match exactly in some instances
            {
                _matchStatisticsService.SaveUnlinkedMatchData(matchStatisticsDto.MatchData, matchStatisticsDto.Access_Token, sourceAddress);

                return NoContent();
            }

            if (match.MatchStatistics != null)
            {
                // INFO: This could be either be a rematch or where the server has persisted the token after map change etc.
                var matchStatisticsId = _matchStatisticsService.SaveUnlinkedMatchData(matchStatisticsDto.MatchData, matchStatisticsDto.Access_Token, sourceAddress);

                if (matchStatisticsDto.MatchData.Teams[0].MatchTotal.Name == match.TeamHome.Name && matchStatisticsDto.MatchData.Teams[1].MatchTotal.Name == match.TeamAway.Name)
                {
                    // INFO: We can be pretty confident this is a rematch
                    _matchStatisticsService.CreateMatchFromMatchData(matchStatisticsId);
                }

                return NoContent();
            }

            _matchStatisticsService.SaveMatchData(matchStatisticsDto.MatchData, matchStatisticsDto.Access_Token, sourceAddress, matchId);

            return NoContent();
        }

        [Authorize]
        [HttpPost("{matchId}")]
        public IActionResult ManualSubmit([FromBody]MatchStatisticsDto matchStatisticsDto, int matchId)
        {
            var match = _matchService.GetMatch(matchId);
            var sourceAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            if (match == null)
            {
                return BadRequest();
            }

            if (match.MatchStatistics != null)
            {
                return BadRequest();
            }

            var hasHubAccess = _playerService.IsAdminOrOwner(User.GetSteamId());
            if (match.TournamentId.HasValue)
            {
                if (!_tournamentService.IsTournamentOrganiser((int)match.TournamentId, User.GetSteamId()) && !hasHubAccess)
                {
                    return Unauthorized();
                }
            }
            else if (!hasHubAccess)
            {
                return Unauthorized();
            }

            _matchStatisticsService.SaveMatchData(matchStatisticsDto.MatchData, matchStatisticsDto.Access_Token, sourceAddress, matchId, true);

            return Ok();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Owner)]
        [HttpPost("{id}/create-match")]
        public IActionResult CreateMatchFromMatchData(int id)
        {
            _matchStatisticsService.CreateMatchFromMatchData(id);

            return Ok();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Owner)]
        [HttpGet("unlinked")]
        public List<MatchStatistics> GetUnlinkedMatchStatistics()
        {
            return _matchStatisticsService.GetUnlinkedMatchData();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost("{id}/unlink")]
        public IActionResult Unlink(int id)
        {
            _matchStatisticsService.UnlinkMatchStatistics(id);

            return Ok();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost("{matchId}/override")]
        public IActionResult CreateMatchResultOverride([FromBody]MatchResultOverrideDto matchResultOverrideDto, int matchId)
        {
            var matchToUpdate = _matchService.GetMatch(matchId);
            var hasHubAccess = _playerService.IsAdminOrOwner(User.GetSteamId());
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

            _matchStatisticsService.CreateMatchResultOverride(matchId, matchResultOverrideDto.HomeGoals, matchResultOverrideDto.AwayGoals);

            return Ok();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost("{id}/swap-teams")]
        public IActionResult SwapTeams(int id)
        {
            _matchStatisticsService.SwapTeams(id);

            return Ok();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost("{matchId}/regenerate")]
        public IActionResult Regenerate(int matchId)
        {
            _matchStatisticsService.RegenerateStatsForMatch(matchId);
            return Ok();
        }
    }
}