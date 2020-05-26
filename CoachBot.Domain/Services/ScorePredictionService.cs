using CoachBot.Database;
using CoachBot.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoachBot.Domain.Services
{
    public class ScorePredictionService
    {
        private readonly CoachBotContext _coachBotContext;

        public ScorePredictionService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public void CreateScorePrediction(ScorePrediction scorePrediction, ulong steamId)
        {
            var player = _coachBotContext.Players.Single(p => p.SteamID == steamId);
            var tournamentPhase = _coachBotContext.TournamentPhases.Single(p => p.Id == scorePrediction.Id);
            scorePrediction.PlayerId = player.Id;

            if (_coachBotContext.TournamentPhases.Any(tp => tp.Id == scorePrediction.TournamentPhaseId && tp.TournamentGroupMatches.Any(g => g.Match.ScheduledKickOff < DateTime.Now)))
            {
                throw new Exception("This tournament phase has already started");
            }
                        
            _coachBotContext.Add(scorePrediction);
        }

        public void UpdateScorePrediction(ScorePrediction scorePrediction, ulong steamId)
        {
            var player = _coachBotContext.Players.Single(p => p.SteamID == steamId);
            var existing = _coachBotContext.ScorePredictions.Single(p => p.Id == scorePrediction.Id);

            if (player.Id != existing.PlayerId)
            {
                throw new Exception("Predictions can only be updated by the player they belong to");
            }

            existing.HomeGoals = scorePrediction.HomeGoals;
            existing.AwayGoals = scorePrediction.AwayGoals;
            existing.UpdatedDate = DateTime.Now;

            _coachBotContext.SaveChanges();
        }
    }
}
