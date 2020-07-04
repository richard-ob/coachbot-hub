using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
        public List<ScorePrediction> GetScorePredictions(int tournamentId)
        {
            return _scorePredictionService.GetScorePredictions(tournamentId, User.GetSteamId());
        }

        [HttpGet("tournament/{tournamentId}/player/{playerId}")]
        public List<ScorePrediction> GetScorePredictions(int tournamentId, int playerId)
        {
            return _scorePredictionService.GetScorePredictions(tournamentId, null, playerId);
        }

        [HttpGet("tournament/{tournamentId}/leaderboard")]
        public List<ScorePredictionLeaderboardPlayer> GetScorePredictionLeaderboard(int tournamentId)
        {
            return _scorePredictionService.GetLeaderboard(tournamentId);
        }

        [Authorize]
        [HttpPost]
        public void CreateScorePrediction(ScorePrediction scorePrediction)
        {
            _scorePredictionService.CreateScorePrediction(scorePrediction, User.GetSteamId());
        }

        [Authorize]
        [HttpPut]
        public void UpdateScorePrediction(ScorePrediction scorePrediction)
        {
            _scorePredictionService.CreateScorePrediction(scorePrediction, User.GetSteamId());
        }
    }
}