using CoachBot.Database;
using CoachBot.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var match = _coachBotContext.Matches.Single(m => m.Id == scorePrediction.MatchId);
            scorePrediction.PlayerId = player.Id;

            if (match.KickOff < DateTime.UtcNow)
            {
                throw new Exception("Prediction can't be submitted after match has already kicked off");
            }

            if (_coachBotContext.ScorePredictions.Any(s => s.PlayerId == scorePrediction.PlayerId && s.MatchId == scorePrediction.MatchId))
            {
                var existing = _coachBotContext.ScorePredictions.Single(s => s.PlayerId == scorePrediction.PlayerId && s.MatchId == scorePrediction.MatchId);
                existing.HomeGoalsPrediction = scorePrediction.HomeGoalsPrediction;
                existing.AwayGoalsPrediction = scorePrediction.AwayGoalsPrediction;
                _coachBotContext.SaveChanges();
            }
            else
            {
                _coachBotContext.Add(scorePrediction);
                _coachBotContext.SaveChanges();
            }
        }

        public void UpdateScorePrediction(ScorePrediction scorePrediction, ulong steamId)
        {
            var player = _coachBotContext.Players.Single(p => p.SteamID == steamId);
            var existing = _coachBotContext.ScorePredictions.Single(p => p.Id == scorePrediction.Id);
            var match = _coachBotContext.Matches.Single(m => m.Id == scorePrediction.MatchId);

            if (player.Id != existing.PlayerId)
            {
                throw new Exception("Predictions can only be updated by the player they belong to");
            }

            if (match.KickOff < DateTime.UtcNow)
            {
                throw new Exception("Prediction can't be submitted after match has already kicked off");
            }

            existing.HomeGoalsPrediction = scorePrediction.HomeGoalsPrediction;
            existing.AwayGoalsPrediction = scorePrediction.AwayGoalsPrediction;
            existing.UpdatedDate = DateTime.UtcNow;

            _coachBotContext.SaveChanges();
        }

        public List<ScorePrediction> GetScorePredictions(int? tournamentId = null, ulong? steamId = null, int? playerId = null)
        {
            return _coachBotContext.ScorePredictions
                .AsNoTracking()
                .Where(s => tournamentId == null || s.TournamentPhase.TournamentStage.TournamentId == tournamentId)
                .Where(s => steamId == null || s.Player.SteamID == steamId)
                .Where(s => playerId == null || s.PlayerId == playerId)
                .Include(m => m.Match)
                    .ThenInclude(m => m.TeamHome)
                    .ThenInclude(m => m.BadgeImage)
                .Include(m => m.Match)
                    .ThenInclude(m => m.TeamAway)
                    .ThenInclude(m => m.BadgeImage)
                .Include(m => m.Match)
                    .ThenInclude(m => m.MatchStatistics)
                .Include(s => s.TournamentPhase)
                .ToList();
        }

        public List<ScorePredictionLeaderboardPlayer> GetLeaderboard(int? tournamentId = null)
        {
            // TODO: Fix this so it doesn't load data into memory. Part of EF Core 3.1+ upgrade issues.
            return _coachBotContext
                .ScorePredictions
                .AsQueryable()
                .Where(s => tournamentId == null || s.TournamentPhase.TournamentStage.TournamentId == tournamentId)
                .Include(s => s.Match)
                .ThenInclude(s => s.MatchStatistics)
                .Include(s => s.Player)
                .Include(s => s.TournamentPhase)
                .ThenInclude(s => s.TournamentStage)
                .ToList()
                .Where(s => s.Match.MatchStatisticsId != null)
                .Select(m => new
                {
                    m.PlayerId,
                    m.Player.Name,
                    m.Match.MatchStatistics.MatchGoalsHome,
                    m.Match.MatchStatistics.MatchGoalsAway,
                    m.HomeGoalsPrediction,
                    m.AwayGoalsPrediction,
                    m.MatchId,
                    m.Player.Rating
                })
                .GroupBy(p => new { p.PlayerId, p.Name }, (key, s) => new ScorePredictionLeaderboardPlayer()
                {
                    PlayerId = key.PlayerId,
                    PlayerName = key.Name,
                    Points = s.Sum(p => p.MatchGoalsHome == p.HomeGoalsPrediction && p.MatchGoalsAway == p.AwayGoalsPrediction ? 1 : 0),
                    Predictions = s.Count()
                })
                .OrderByDescending(p => p.Points)
                .ToList();
        }

        public ScorePredictionLeaderboardPlayer GetMonthLeader()
        {
            // TODO: Fix this so it doesn't load data into memory. Part of EF Core 3.1+ upgrade issues.
            return _coachBotContext
                 .ScorePredictions
                 .AsQueryable()
                 .Where(s => s.CreatedDate > DateTime.UtcNow.AddMonths(-1))
                 .Where(s => s.Match.MatchStatisticsId != null)
                 .Include(s => s.Match)
                 .ThenInclude(s => s.MatchStatistics)
                 .Include(s => s.Player)
                 .ToList()
                 .Select(m => new
                 {
                     m.PlayerId,
                     m.Player.Name,
                     m.Match.MatchStatistics.MatchGoalsHome,
                     m.Match.MatchStatistics.MatchGoalsAway,
                     m.HomeGoalsPrediction,
                     m.AwayGoalsPrediction,
                     m.MatchId,
                     m.Player.Rating
                 })
                 .GroupBy(p => new { p.PlayerId, p.Name }, (key, s) => new ScorePredictionLeaderboardPlayer()
                 {
                     PlayerId = key.PlayerId,
                     PlayerName = key.Name,
                     Points = s.Sum(p => p.MatchGoalsHome == p.HomeGoalsPrediction && p.MatchGoalsAway == p.AwayGoalsPrediction ? 1 : 0),
                     Predictions = s.Count()
                 })
                 .OrderByDescending(s => s.Points)
                 .FirstOrDefault();
        }

    }
}