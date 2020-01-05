using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using CoachBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoachBot.Domain.Services
{
    public class MatchStatisticsService
    {
        private readonly CoachBotContext _coachBotContext;
        private const double EXPECTED_PLAYERCOUNT_THRESHOLD_MULTIPLIER = 0.75;

        public MatchStatisticsService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public void SaveMatchData(MatchData matchData, int matchId, bool manualSave = false)
        {
            var match = _coachBotContext.Matches.Single(m => m.Id == matchId);
            var matchStatistics = new MatchStatistics()
            {
                MatchId = match.Id,
                MatchData = matchData
            };

            ValidateMatchData(matchData, match, manualSave);
            _coachBotContext.MatchStatistics.Add(matchStatistics);

            GenerateTeamStatisticTotals(match);
            GeneratePlayerStatisticTotals(match);

            _coachBotContext.SaveChanges();
        }

        public MatchStatistics GetMatchStatistics(int matchId)
        {
            return _coachBotContext.MatchStatistics.FirstOrDefault(m => m.MatchId == matchId);
        }

        public List<TeamStatisticTotals> GetTeamStatistics()
        {
            return null;
        }

        public List<PlayerStatisticTotals> GetPlayerStatistics()
        {
            return null;
        }

        #region Private Methods
        private void ValidateMatchData(MatchData matchData, Match match, bool manualSave)
        {
            // Validate match has correct player counts
            var expectedPlayerCount = match.SignedPlayers.Count();
            var actualPlayerCount = matchData.Players.Count();
            if (expectedPlayerCount * EXPECTED_PLAYERCOUNT_THRESHOLD_MULTIPLIER > actualPlayerCount)
            {
                throw new Exception($"Too few players present in match data. Expected at least {expectedPlayerCount}, found {actualPlayerCount}.");
            }

            // Validate match took place within an hour of the match ready time
            if (DateTime.Now.AddHours(-2) > match.ReadiedDate && !manualSave)
            {
                throw new Exception($"The match should finish no later than two hours after being readied.");
            }
        }

        private void GenerateTeamStatisticTotals(Match match)
        {
            var homeTeamStatisticTotals = _coachBotContext.TeamStatisticTotals.FirstOrDefault(t => t.ChannelId == match.TeamHome.ChannelId);
            if (homeTeamStatisticTotals == null)
            {
                homeTeamStatisticTotals = new TeamStatisticTotals
                {
                    ChannelId = (int)match.TeamHome.ChannelId
                };
                _coachBotContext.TeamStatisticTotals.Add(homeTeamStatisticTotals);
            }
            AddMatchDataTotalsToTeamStatisticTotals(ref homeTeamStatisticTotals, match.MatchStatistics.MatchData, MatchDataTeamType.Home);

            var awayTeamStatisticsTotals = _coachBotContext.TeamStatisticTotals.FirstOrDefault(t => t.ChannelId == match.TeamAway.ChannelId);
            if (awayTeamStatisticsTotals == null)
            {
                awayTeamStatisticsTotals = new TeamStatisticTotals
                {
                    ChannelId = (int)match.TeamAway.ChannelId
                };
                _coachBotContext.TeamStatisticTotals.Add(awayTeamStatisticsTotals);
            }
            AddMatchDataTotalsToTeamStatisticTotals(ref awayTeamStatisticsTotals, match.MatchStatistics.MatchData, MatchDataTeamType.Away);
        }

        private void AddMatchDataTotalsToTeamStatisticTotals(ref TeamStatisticTotals teamStatisticTotals, MatchData matchData, MatchDataTeamType matchDataTeamType)
        {
            // Game-generated statistics
            PropertyInfo[] properties = typeof(StatisticTotals).GetProperties();
            var matchDataTeamMatchTotal = matchData.Teams[(int)matchDataTeamType].MatchTotal;
            var matchDataStatisticType = 0;
            foreach (PropertyInfo property in properties)
            {
                property.SetValue(teamStatisticTotals.StatisticTotals, (int)property.GetValue(teamStatisticTotals.StatisticTotals) + matchDataTeamMatchTotal.Statistics[matchDataStatisticType]);
                matchDataStatisticType++;
                if (matchDataStatisticType > typeof(MatchDataStatisticType).GetEnumNames().Count() - 1)
                {
                    break;
                }
            }

            // Custom statistics
            teamStatisticTotals.StatisticTotals.Matches++;
            var teamGoals = matchData.GetMatchStatistic(MatchDataStatisticType.Goals, matchDataTeamType);
            var opponentGoals = matchData.GetMatchStatistic(MatchDataStatisticType.Goals, 1 - matchDataTeamType);
            if (teamGoals > opponentGoals) teamStatisticTotals.StatisticTotals.Wins++;
            if (teamGoals == opponentGoals) teamStatisticTotals.StatisticTotals.Draws++;
            if (opponentGoals > teamGoals) teamStatisticTotals.StatisticTotals.Losses++;
        }

        private void GeneratePlayerStatisticTotals(Match match)
        {
            foreach(var matchDataPlayer in match.MatchStatistics.MatchData.Players)
            {
                var player = _coachBotContext.Players.FirstOrDefault(p => p.SteamID == matchDataPlayer.Info.SteamId);
                var playerStatisticTotals = _coachBotContext.PlayerStatisticTotals.FirstOrDefault(p => player != null && p.PlayerId == player.Id) ?? new PlayerStatisticTotals();
                if (player == null)
                {
                    player = new Player()
                    {
                        Name = matchDataPlayer.Info.Name,
                        SteamID = matchDataPlayer.Info.SteamId
                    };
                    _coachBotContext.Players.Add(player);
                    playerStatisticTotals.Player = player;
                    _coachBotContext.PlayerStatisticTotals.Add(playerStatisticTotals);
                }
                AddMatchDataTotalsToPlayerStatisticTotals(ref playerStatisticTotals, match.MatchStatistics.MatchData, player.SteamID);
            }
        }

        private void AddMatchDataTotalsToPlayerStatisticTotals(ref PlayerStatisticTotals playerStatisticTotals, MatchData matchData, string steamId)
        {
            // Game-generated statistics
            PropertyInfo[] properties = typeof(StatisticTotals).GetProperties();
            foreach (var matchPeriod in matchData.Players.First(p => p.Info.SteamId == steamId).MatchPeriodData)
            {
                var matchDataStatisticType = 0;
                foreach (PropertyInfo property in properties)
                {
                    property.SetValue(playerStatisticTotals.StatisticTotals, (int)property.GetValue(playerStatisticTotals.StatisticTotals) + matchPeriod.Statistics[matchDataStatisticType]);
                    matchDataStatisticType++;
                    if (matchDataStatisticType > typeof(MatchDataStatisticType).GetEnumNames().Count() - 1)
                    {
                        break;
                    }
                }
            }

            // Custom statistics
            // TODO: work out what the JSON output uses for Team name string, so can figure out which team the player is on
            playerStatisticTotals.StatisticTotals.Matches++;
            var teamGoals = matchData.GetMatchStatistic(MatchDataStatisticType.Goals, MatchDataTeamType.Home);
            var opponentGoals = matchData.GetMatchStatistic(MatchDataStatisticType.Goals, MatchDataTeamType.Away);
            if (teamGoals > opponentGoals) playerStatisticTotals.StatisticTotals.Wins++;
            if (teamGoals == opponentGoals) playerStatisticTotals.StatisticTotals.Draws++;
            if (opponentGoals > teamGoals) playerStatisticTotals.StatisticTotals.Losses++;
        }
        #endregion
    }
}
