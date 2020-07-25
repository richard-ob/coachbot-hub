using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/score-predictions")]
    [ApiController]
    public class ScorePredictionController : Controller
    {
        private readonly ScorePredictionService _scorePredictionService;

        public ScorePredictionController(ScorePredictionService scorePredictionService)
        {
            _scorePredictionService = scorePredictionService;
        }

        [HttpGet("tournament/{tournamentId}")]
        public List<ScorePrediction> GetScorePredictionsByTournament(int tournamentId)
        {
            return _scorePredictionService.GetScorePredictions(tournamentId, User.GetSteamId());
        }

        [HttpGet("tournament/{tournamentId}/player/{playerId}")]
        public List<ScorePrediction> GetScorePredictions(int playerId, int tournamentId)
        {
            return _scorePredictionService.GetScorePredictions(tournamentId, null, playerId);
        }

        [HttpGet("player/{playerId}")]
        public List<ScorePrediction> GetScorePredictionsByPlayer(int playerId)
        {
            return _scorePredictionService.GetScorePredictions(null, null, playerId);
        }

        [HttpGet("tournament/{tournamentId}/leaderboard")]
        public List<ScorePredictionLeaderboardPlayer> GetScorePredictionLeaderboard(int tournamentId)
        {
            return _scorePredictionService.GetLeaderboard(tournamentId);
        }

        [HttpGet("leaderboard")]
        public List<ScorePredictionLeaderboardPlayer> GetScorePredictionLeaderboard()
        {
            return _scorePredictionService.GetLeaderboard();
        }

        [HttpGet("leaderboard/month-leader")]
        public ScorePredictionLeaderboardPlayer GetScorePredictionMonthLeader()
        {
            return _scorePredictionService.GetMonthLeader();
        }
        
        [HubRolePermission(HubRole = PlayerHubRole.Player)]
        [HttpPost]
        public void CreateScorePrediction(ScorePrediction scorePrediction)
        {
            _scorePredictionService.CreateScorePrediction(scorePrediction, User.GetSteamId());
        }

        [HubRolePermission(HubRole = PlayerHubRole.Player)]
        [HttpPut]
        public void UpdateScorePrediction(ScorePrediction scorePrediction)
        {
            _scorePredictionService.CreateScorePrediction(scorePrediction, User.GetSteamId());
        }
    }
}