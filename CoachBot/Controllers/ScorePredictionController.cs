using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

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

        [HttpGet("tournament/{tournamentEditionId}")]
        public List<ScorePrediction> GetScorePredictions(int tournamentEditionId)
        {
            return _scorePredictionService.GetScorePredictions(tournamentEditionId, User.GetSteamId());
        }

        [HttpGet("tournament/{tournamentEditionId}/player/{playerId}")]
        public List<ScorePrediction> GetScorePredictions(int tournamentEditionId, int playerId)
        {
            return _scorePredictionService.GetScorePredictions(tournamentEditionId, null, playerId);
        }

        [HttpGet("tournament/{tournamentEditionId}/leaderboard")]
        public List<ScorePredictionLeaderboardPlayer> GetScorePredictionLeaderboard(int tournamentEditionId)
        {
            return _scorePredictionService.GetLeaderboard(tournamentEditionId);
        }

        [HttpPost]
        public void CreateScorePrediction(ScorePrediction scorePrediction)
        {
            _scorePredictionService.CreateScorePrediction(scorePrediction, User.GetSteamId());
        }

        [HttpPut]
        public void UpdateScorePrediction(ScorePrediction scorePrediction)
        {
            _scorePredictionService.CreateScorePrediction(scorePrediction, User.GetSteamId());
        }

    }
}
