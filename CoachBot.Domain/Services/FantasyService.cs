using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CoachBot.Domain.Services
{
    public class FantasyService
    {
        private readonly CoachBotContext _coachBotContext;
        private readonly IBackgroundTaskQueue _queue;
        private readonly IServiceProvider serviceProvider;

        public FantasyService(CoachBotContext coachBotContext, IBackgroundTaskQueue queue, IServiceProvider serviceProvider)
        {
            _coachBotContext = coachBotContext;
            _queue = queue;
            this.serviceProvider = serviceProvider;
        }

        public IEnumerable<FantasyTeam> GetFantasyTeams(int tournamentId)
        {
            return _coachBotContext.FantasyTeams.Where(ft => ft.TournamentId == tournamentId).ToList();
        }

        public FantasyTeam GetFantasyTeam(int fantasyTeamId)
        {
            return _coachBotContext.FantasyTeams
                .Include(ft => ft.FantasyTeamSelections)
                    .ThenInclude(ft => ft.FantasyPlayer)
                    .ThenInclude(ft => ft.Player)
                .Include(ft => ft.FantasyTeamSelections)
                    .ThenInclude(ft => ft.FantasyPlayer)
                    .ThenInclude(ft => ft.Player.Country)
                .Include(ft => ft.FantasyTeamSelections)
                    .ThenInclude(ft => ft.FantasyPlayer)
                    .ThenInclude(ft => ft.Team)
                    .ThenInclude(ft => ft.BadgeImage)
                .Include(ft => ft.Player)
                .Include(ft => ft.Tournament)
                .First(ft => ft.Id == fantasyTeamId);
        }

        public void CreateFantasyTeam(FantasyTeam fantasyTeam, ulong steamId)
        {
            CheckHasTournamentStarted((int)fantasyTeam.TournamentId);
            fantasyTeam.Player = null;
            fantasyTeam.PlayerId = _coachBotContext.Players.Single(p => p.SteamID == steamId).Id;
            fantasyTeam.IsFinalised = false;
            fantasyTeam.FantasyTeamSelections = null;
            fantasyTeam.Tournament = null;
            _coachBotContext.FantasyTeams.Add(fantasyTeam);
            _coachBotContext.SaveChanges();
        }

        public void UpdateFantasyTeam(FantasyTeam fantasyTeam)
        {
            CanUpdateTeam(fantasyTeam.Id);
            var existingFantasyTeam = _coachBotContext.FantasyTeams.Find(fantasyTeam.Id);
            existingFantasyTeam.IsFinalised = fantasyTeam.IsFinalised;
            existingFantasyTeam.Name = fantasyTeam.Name;
            _coachBotContext.FantasyTeams.Update(existingFantasyTeam);
            _coachBotContext.SaveChanges();
        }

        public void DeleteFantasyTeam(int fantasyTeamId)
        {
            CanUpdateTeam(fantasyTeamId);
            var fantasyTeam = _coachBotContext.FantasyTeams.Find(fantasyTeamId);
            _coachBotContext.FantasyTeams.Remove(fantasyTeam);
            _coachBotContext.SaveChanges();
        }

        public void AddFantasyTeamSelection(FantasyTeamSelection fantasyTeamSelection)
        {
            CanUpdateTeam((int)fantasyTeamSelection.FantasyTeamId);
            fantasyTeamSelection.FantasyPlayer = null;
            fantasyTeamSelection.FantasyTeam = null;
            _coachBotContext.FantasyTeamSelections.Add(fantasyTeamSelection);
            _coachBotContext.SaveChanges();
        }

        public void RemoveFantasyTeamSelection(int fantasyTeamSelectionId)
        {
            var selection = _coachBotContext.FantasyTeamSelections.Find(fantasyTeamSelectionId);
            CanUpdateTeam((int)selection.FantasyTeamId);

            _coachBotContext.FantasyTeamSelections.Remove(selection);
            _coachBotContext.SaveChanges();
        }

        public Model.Dtos.PagedResult<FantasyPlayer> GetFantasyPlayers(int page, int pageSize, string sortOrder, PlayerStatisticFilters playerStatisticFilters)
        {          
            return GetFantasyPlayersQueryable(playerStatisticFilters).GetPaged(page, pageSize, sortOrder);
        }

        public List<FantasyTeam> GetFantasyTeamsForPlayer(ulong steamUserId)
        {
            return _coachBotContext.FantasyTeams
                .Where(t => t.Player.SteamID == steamUserId)
                .Include(t => t.Tournament)
                .ToList();
        }

        public List<Tournament> GetAvailableTournamentsForUser(ulong steamUserId)
        {
            return _coachBotContext.Tournaments
                .Include(t => t.TournamentSeries)
                .Where(t => !_coachBotContext.Matches.Any(m => m.TournamentId == t.Id && m.ScheduledKickOff < DateTime.Now))
                .Where(t => !_coachBotContext.FantasyTeams.Any(ft => ft.Player.SteamID == steamUserId))
                .ToList();
        }
        public IList<FantasyTeamRank> GetFantasyTeamRankings(int tournamentId)
        {
            return _coachBotContext
                 .FantasyTeamRanks
                 .FromSql($@"SELECT FantasyTeams.Id AS FantasyTeamId,
                                    FantasyTeams.Name AS FantasyTeamName,
                                    Players.Id AS PlayerId,
                                    Players.SteamID AS SteamID,
                                    FantasyTeams.TournamentId, SUM(FantasyPlayerPhases.Points) AS Points,
                                    CONVERT(INT, DENSE_RANK() OVER(ORDER BY SUM(FantasyPlayerPhases.Points) DESC)) AS Rank
                            FROM dbo.FantasyTeams FantasyTeams
                            INNER JOIN dbo.FantasyTeamSelections FantasyTeamSelections
                                ON FantasyTeams.Id = FantasyTeamSelections.FantasyTeamId
                            INNER JOIN dbo.FantasyPlayerPhases FantasyPlayerPhases
                                ON FantasyPlayerPhases.FantasyPlayerId = FantasyTeamSelections.FantasyPlayerId
                            INNER JOIN dbo.Players Players
                                ON Players.Id = FantasyTeams.PlayerId
                            WHERE FantasyTeams.TournamentId = {tournamentId}
                            GROUP BY FantasyTeams.Id, FantasyTeams.Name, FantasyTeams.TournamentId, Players.Id, Players.SteamID
                            ORDER BY SUM(POINTS) DESC")
                .ToList();
        }

        /*public IList<FantasyTeamRank> GetFantasyTeamRankings(int tournamentId)
        {
            return _coachBotContext
                 .FantasyTeams
                 .Where(f => f.TournamentId == tournamentId)
                 .AsNoTracking()
                 .Select(m => new
                 {
                     Id = m.Id,
                     Name = m.Name,
                     PlayerId = m.PlayerId,
                     Points = m.FantasyTeamSelections.Sum(p => p.FantasyPlayer.Phases.Sum(t => t.Points))
                 })
                 .GroupBy(p => new { p.Id, p.Name, p.PlayerId }, (key, s) => new FantasyTeamRank()
                 {
                     FantasyTeamName = key.Name,
                     FantasyTeamId = key.Id,
                     PlayerId = (int)key.PlayerId,
                     Points = s.Sum(x => x.Points)
                 }).ToList();
        }*/

        public void GenerateFantasyPlayersSnapshot(int tournamentId)
        {
            if (!_coachBotContext.TournamentGroupTeams.Any(t => t.TournamentGroup.TournamentStage.TournamentId == tournamentId))
            {
                throw new Exception("There are no teams added to this tournament yet");
            }

            if (_coachBotContext.FantasyPlayers.Any(t => t.TournamentId == tournamentId))
            {
                GenerateFantasyPhaseSnapshots(1);
                throw new Exception("Fantasy season already seeded");
            }

            foreach(var player in _coachBotContext.Players.Include(p => p.Teams).Where(p => p.Teams.Any(pt => pt.IsCurrentTeam 
                && _coachBotContext.TournamentGroupTeams.Any(tgt => tgt.TournamentGroup.TournamentStage.TournamentId == tournamentId && tgt.TeamId == pt.TeamId)))
            )
            {
                player.Rating = GetRandomRating();
                var fantasyPlayer = new FantasyPlayer()
                {
                    TournamentId = tournamentId,
                    PlayerId = player.Id,
                    PositionGroup = GetRandomPositionGroup(),
                    Rating = player.Rating,
                    TeamId = player.Teams.Where(t => t.IsCurrentTeam && _coachBotContext.TournamentGroupTeams.Any(tg => tg.TeamId == t.TeamId && tg.TournamentGroup.TournamentStage.TournamentId == tournamentId)).Select(t => t.TeamId).First()
                };
                _coachBotContext.FantasyPlayers.Add(fantasyPlayer);
            }
            _coachBotContext.SaveChanges();
        }

        static float GetRandomRating()
        {
            Random r = new Random();
            string beforePoint = r.Next(4, 9).ToString();
            string afterPoint = r.Next(0, 9).ToString();
            string combined = beforePoint + "." + afterPoint;
            return float.Parse(combined);
        }

        static PositionGroup GetRandomPositionGroup()
        {
            return (PositionGroup)new Random().Next(0, 4);
        }

        public void GenerateFantasyPhaseSnapshots(int tournamentPhaseId)
        {
            _queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = this.serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetService(typeof(CoachBotContext)) as CoachBotContext;
                    var tournamentId = context.TournamentPhases.Where(p => p.Id == tournamentPhaseId).Select(t => t.TournamentStage.TournamentId).Single();
                    context.FantasyPlayerPhases.RemoveRange(context.FantasyPlayerPhases.Where(f => f.TournamentPhaseId == tournamentPhaseId));
                    var matches = context.TournamentGroupMatches
                        .Include(m => m.Match)
                        .ThenInclude(m => m.PlayerMatchStatistics)
                        .Include(m => m.Match)
                        .ThenInclude(m => m.PlayerPositionMatchStatistics)
                        .ThenInclude(m => m.Position)
                        .Where(m => m.TournamentPhaseId == tournamentPhaseId);

                    Console.WriteLine("Generating fantasy phase snapshots..");
                    foreach (var match in matches)
                    {
                        foreach (var playerMatchStatistics in match.Match.PlayerMatchStatistics)
                        {
                            Console.WriteLine("Generating fantasy phase snapshot for " + playerMatchStatistics.Nickname);
                            var fantasyPlayer = context.FantasyPlayers.FirstOrDefault(f => f.TournamentId == tournamentId && f.PlayerId == playerMatchStatistics.PlayerId);
                            if (fantasyPlayer == null) continue;

                            var playerMatchPositionStatistics = match.Match.PlayerPositionMatchStatistics.Where(p => p.PlayerId == fantasyPlayer.PlayerId);

                            var fantasyPlayerPhase = new FantasyPlayerPhase()
                            {
                                PlayerMatchStatisticsId = playerMatchStatistics.Id,
                                TournamentPhaseId = tournamentPhaseId,
                                FantasyPlayerId = fantasyPlayer.Id,
                                Points = CalculateFantasyPoints(playerMatchStatistics, playerMatchPositionStatistics),
                                PositionGroup = DeterminePositionGroup(playerMatchPositionStatistics)
                            };

                            context.FantasyPlayerPhases.Add(fantasyPlayerPhase);
                        }
                    }
                    await context.SaveChangesAsync();
                    context.Dispose();
                }
            });
            Thread.Sleep(1000);
        }

        private int CalculateFantasyPoints(PlayerMatchStatistics playerMatchStatistics, IEnumerable<PlayerPositionMatchStatistics> playerMatchPositionStatistics)
        {
            var mainPosition = DeterminePositionGroup(playerMatchPositionStatistics);

            // Appearance
            var currentScore = 1; // Playing a game is 1 point
            if (playerMatchStatistics.SecondsPlayed / 60 > 60) currentScore += 2;

            // Goals
            if (mainPosition == PositionGroup.Goalkeeper) currentScore += playerMatchStatistics.Goals * 6; // GK goals scored
            if (mainPosition == PositionGroup.Defence) currentScore += playerMatchStatistics.Goals * 6; // Def goals scored
            if (mainPosition == PositionGroup.Midfield) currentScore += playerMatchStatistics.Goals * 5; // Mid goals scored
            if (mainPosition == PositionGroup.Attack) currentScore += playerMatchStatistics.Goals * 4; // Attack goals scored

            // Assists
            currentScore += playerMatchStatistics.Assists * 3; // Assists

            // Clean sheets
            if ((mainPosition == PositionGroup.Goalkeeper || mainPosition == PositionGroup.Defence) && playerMatchStatistics.GoalsConceded == 0) currentScore += 4;
            if (mainPosition == PositionGroup.Midfield && playerMatchStatistics.GoalsConceded == 0) currentScore += 1;

            // Saves
            if (playerMatchStatistics.KeeperSaves > 0) currentScore += (int)playerMatchStatistics.KeeperSaves / 3;

            // Goals Conceded
            if (mainPosition == PositionGroup.Goalkeeper || mainPosition == PositionGroup.Defence) currentScore -= playerMatchStatistics.GoalsConceded;

            // Cards
            currentScore -= playerMatchStatistics.YellowCards;
            currentScore -= playerMatchStatistics.RedCards * 3;

            // Own Goals
            currentScore -= playerMatchStatistics.OwnGoals * 2;

            // Pass Completion
            var passCompletion = playerMatchStatistics.Passes > 0 ? (playerMatchStatistics.PassesCompleted / playerMatchStatistics.Passes) * 100 : 0;
            if (passCompletion >= 70 && passCompletion < 80) currentScore += 1;
            if (passCompletion >= 80 && passCompletion < 90) currentScore += 3;
            if (passCompletion >= 90) currentScore += 5;

            // Interceptions
            if (playerMatchStatistics.Interceptions >= 10 && playerMatchStatistics.Interceptions < 15) currentScore += 1;
            if (playerMatchStatistics.Interceptions >= 15 && playerMatchStatistics.Interceptions < 20) currentScore += 3;
            if (playerMatchStatistics.Interceptions >= 20) currentScore += 5;

            return currentScore;
        }

        private PositionGroup DeterminePositionGroup(IEnumerable<PlayerPositionMatchStatistics> playerPositionMatchStatistics)
        {
            var defPositions = new string[] { "LWB", "LB", "LCB", "SWP", "CB", "RCB", "RB", "RWB" };
            var midPositions = new string[] { "LM", "LCM", "CDM", "CM", "CAM", "RCM", "RM" };
            var attackPositions = new string[] { "LW", "LF", "CF", "SS", "ST", "RF", "RW" };

            var position = playerPositionMatchStatistics.OrderByDescending(p => p.SecondsPlayed).Select(p => p.Position.Name).First();

            if (position == "GK")
            {
                return PositionGroup.Goalkeeper;
            }
            else if (defPositions.Any(p => p == position))
            {
                return PositionGroup.Defence;
            }
            else if (midPositions.Any(p => p == position))
            {
                return PositionGroup.Midfield;
            }
            else if (attackPositions.Any(p => p == position))
            {
                return PositionGroup.Attack;
            }

            throw new Exception("No position matched");
        }

        private IQueryable<FantasyPlayer> GetFantasyPlayersQueryable(PlayerStatisticFilters playerStatisticFilters)
        {
            var queryable = _coachBotContext
                    .FantasyPlayers
                    .AsNoTracking()
                    .Include(fp => fp.Player)
                    .Include(t => t.Team)
                    .ThenInclude(t => t.BadgeImage)
                    .Where(p => string.IsNullOrWhiteSpace(playerStatisticFilters.PlayerName) || p.Player.Name.Contains(playerStatisticFilters.PlayerName))
                    .Where(p => playerStatisticFilters.MaximumRating == null || p.Player.Rating <= playerStatisticFilters.MaximumRating)
                    .Where(p => playerStatisticFilters.MinimumRating == null || p.Player.Rating >= playerStatisticFilters.MinimumRating)
                    .Where(p => playerStatisticFilters.PositionGroup == null || p.PositionGroup == playerStatisticFilters.PositionGroup)
                    .Where(p => playerStatisticFilters.TeamId == null || p.TeamId == playerStatisticFilters.TeamId)
                    .Where(p => p.TournamentId == playerStatisticFilters.TournamentId);

            foreach (var excludedPlayer in playerStatisticFilters.ExcludePlayers)
            {
                queryable = queryable.Where(p => p.PlayerId != excludedPlayer);
            }

            return queryable;
        }

        private void CanUpdateTeam(int fantasyTeamId)
        {
            var fantasyTeam = _coachBotContext.FantasyTeams.Find(fantasyTeamId);

            if (fantasyTeam.IsFinalised)
            {
                throw new Exception("Teams cannot be updated after they have been finalised");
            }

            CheckHasTournamentStarted((int)fantasyTeam.TournamentId);
        }

        private void CheckHasTournamentStarted(int tournamentId)
        {
            if (_coachBotContext.Matches.Any(m => m.TournamentId == tournamentId && m.ScheduledKickOff < DateTime.Now))
            {
                throw new Exception("The tournament has started. Fantasy teams cannot be updated.");
            }
        }
    }
}
