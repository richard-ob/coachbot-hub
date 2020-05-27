using CoachBot.Database;
using CoachBot.Domain.Model;
using Microsoft.EntityFrameworkCore;
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

        public List<ScorePrediction> GetScorePredictions(ulong steamId, int tournamentEditionId)
        {
            return _coachBotContext.ScorePredictions
                .Where(s => s.Player.SteamID == steamId)
                .Include(m => m.Match)
                    .ThenInclude(m => m.TeamHome)
                    .ThenInclude(m => m.BadgeImage)
                .Include(m => m.Match)
                    .ThenInclude(m => m.TeamAway)
                    .ThenInclude(m => m.BadgeImage)
                .Include(s => s.TournamentPhase)
                .ToList();
        }

        public List<ScorePredictionLeaderboardPlayer> GetLeaderboard(int tournamentEditionId)
        {
            return _coachBotContext
                 .ScorePredictions
                 .Where(s => s.TournamentPhase.TournamentStage.TournamentEditionId == tournamentEditionId)
                 .Where(s => s.Match.MatchStatistics != null)
                 .AsNoTracking()
                 .Select(m => new
                 {
                     m.PlayerId,
                     m.Player.Name,
                     m.Match.MatchStatistics.HomeGoals,
                     m.Match.MatchStatistics.AwayGoals,
                     HomeGoalsPrediction = m.HomeGoals,
                     AwayGoalsPrediction = m.AwayGoals,
                     m.MatchId,
                     m.Player.Rating
                 })
                 .GroupBy(p => new { p.PlayerId, p.Name }, (key, s) => new ScorePredictionLeaderboardPlayer() {
                     PlayerId = key.PlayerId,
                     PlayerName = key.Name,
                     Points = s.Sum(p => p.HomeGoals == p.HomeGoalsPrediction && p.AwayGoals == p.AwayGoalsPrediction ? 1 : 0)
                 })
                 .ToList();
        }
    }
}
