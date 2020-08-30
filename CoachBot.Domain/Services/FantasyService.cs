using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using CoachBot.Model;
using CoachBot.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CoachBot.Domain.Services
{
    public class FantasyService
    {
        private const int SQUAD_SIZE = 11;
        private const int MAX_GKS = 1;
        private const int MAX_DEFS = 3;
        private const int MAX_MIDS = 1;
        private const int MAX_ATTACKERS = 3;

        private readonly CoachBotContext _coachBotContext;
        private readonly IBackgroundTaskQueue _queue;
        private readonly IServiceProvider serviceProvider;
        private readonly PlayerProfileService _playerProfileService;

        public FantasyService(CoachBotContext coachBotContext, IBackgroundTaskQueue queue, IServiceProvider serviceProvider, PlayerProfileService playerProfileService)
        {
            _coachBotContext = coachBotContext;
            _queue = queue;
            this.serviceProvider = serviceProvider;
            _playerProfileService = playerProfileService;
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

        public IEnumerable<FantasyTeamSummary> GetFantasyTeamSummaries(int tournamentId)
        {
            return _coachBotContext.FantasyTeams
                .Where(f => f.TournamentId == tournamentId)
                .Select(s => new FantasyTeamSummary()
                {
                    FantasyTeamId = s.Id,
                    FantasyTeamName = s.Name,
                    PlayerId = (int)s.PlayerId,
                    PlayerName = s.Player.Name,
                    TournamentName = s.Tournament.Name,
                    TournamentId = (int)s.TournamentId,
                    IsComplete = s.IsComplete,
                    FantasyTeamStatus =
                        !s.Tournament.TournamentStages.Any(t => t.TournamentGroups.Any(g => g.TournamentGroupMatches.Any(m => m.Match.KickOff < DateTime.UtcNow))) ? FantasyTeamStatus.Open
                        : s.Tournament.TournamentStages.Any(t => t.TournamentGroups.Any(g => g.TournamentGroupMatches.Any(m => m.Match.KickOff > DateTime.UtcNow))) ? FantasyTeamStatus.Pending
                        : FantasyTeamStatus.Settled
                }).ToList();
        }

        public FantasyTeamSummary GetFantasyTeamSummary(int fantasyTeamId)
        {
            return _coachBotContext.FantasyTeams
                .Where(f => f.Id == fantasyTeamId)
                .Select(s => new FantasyTeamSummary()
                {
                    FantasyTeamId = s.Id,
                    FantasyTeamName = s.Name,
                    PlayerId = (int)s.PlayerId,
                    PlayerName = s.Player.Name,
                    TournamentName = s.Tournament.Name,
                    TournamentId = (int)s.TournamentId,
                    IsComplete = s.IsComplete,
                    FantasyTeamStatus =
                        !s.Tournament.TournamentStages.Any(t => t.TournamentGroups.Any(g => g.TournamentGroupMatches.Any(m => m.Match.KickOff < DateTime.UtcNow))) ? FantasyTeamStatus.Open
                        : s.Tournament.TournamentStages.Any(t => t.TournamentGroups.Any(g => g.TournamentGroupMatches.Any(m => m.Match.KickOff > DateTime.UtcNow))) ? FantasyTeamStatus.Pending
                        : FantasyTeamStatus.Settled
                }).FirstOrDefault();
        }

        public List<FantasyTeamSummary> GetFantasyTeamSummariesForPlayer(ulong steamUserId)
        {
            return _coachBotContext.FantasyTeams
                .Where(f => f.Player.SteamID == steamUserId)
                .Select(s => new FantasyTeamSummary()
                {
                    FantasyTeamId = s.Id,
                    FantasyTeamName = s.Name,
                    PlayerId = (int)s.PlayerId,
                    PlayerName = s.Player.Name,
                    TournamentName = s.Tournament.Name,
                    TournamentId = (int)s.TournamentId,
                    IsComplete = s.IsComplete,
                    FantasyTeamStatus =
                        !s.Tournament.TournamentStages.Any(t => t.TournamentGroups.Any(g => g.TournamentGroupMatches.Any(m => m.Match.KickOff < DateTime.UtcNow))) ? FantasyTeamStatus.Open
                        : s.Tournament.TournamentStages.Any(t => t.TournamentGroups.Any(g => g.TournamentGroupMatches.Any(m => m.Match.KickOff > DateTime.UtcNow))) ? FantasyTeamStatus.Pending
                        : FantasyTeamStatus.Settled
                }).ToList();
        }

        public List<FantasyPlayerPerformance> GetFantasyTeamPlayerPerformances(int fantasyTeamId, int? fantasyPhaseId = null)
        {
            var fantasySelections = _coachBotContext.FantasyTeamSelections
                .Where(f => f.FantasyTeamId == fantasyTeamId)
                .Include(f => f.FantasyPlayer)
                    .ThenInclude(f => f.Player)
                .Include(f => f.FantasyPlayer)
                    .ThenInclude(f => f.Team)
                    .ThenInclude(f => f.BadgeImage)
                .ToList();
            var fantasySelectionIds = fantasySelections.Select(f => f.FantasyPlayerId);

            var playerPerformances =_coachBotContext.FantasyPlayerPhases
                .Where(f => fantasySelectionIds.Any(s => s == f.FantasyPlayerId))
                .Where(f => fantasyPhaseId == null || f.TournamentPhaseId == fantasyPhaseId)
                .Select(n => new
                {
                    n.FantasyPlayerId,
                    n.FantasyPlayer.PlayerId,
                    n.FantasyPlayer.Player.Name,
                    n.FantasyPlayer.Team.TeamCode,
                    n.FantasyPlayer.Team.BadgeImage.Url,
                    n.FantasyPlayer.PositionGroup,
                    n.Points,
                    n.FantasyPlayer.Rating
                })
                .GroupBy(p => new { p.FantasyPlayerId, p.PlayerId, p.Name, p.PositionGroup, p.TeamCode, p.Url, p.Rating }, (key, s) => new FantasyPlayerPerformance()
                {
                    FantasyPlayer = new FantasyPlayer()
                    {
                        Id = key.FantasyPlayerId,
                        Rating = key.Rating,
                        PositionGroup = key.PositionGroup,
                        Player = new Player()
                        {
                            Id = key.PlayerId.Value,
                            Name = key.Name,

                        },
                        Team = new Team()
                        {
                            TeamCode = key.TeamCode,
                            BadgeImage = new AssetImage()
                            {
                                Url = key.Url
                            }
                        }
                    },
                    IsFlex = fantasySelections.First(f => f.FantasyPlayer.PlayerId == key.PlayerId.Value).IsFlex,
                    Points = s.Sum(c => c.Points),
                })
                .ToList();

            var missingPlayers = fantasySelections
                .Where(f => !playerPerformances.Any(p => p.FantasyPlayer.Id == f.FantasyPlayerId))
                .Select(f => new FantasyPlayerPerformance()
                {
                    FantasyPlayer = new FantasyPlayer()
                    {
                        Rating = f.FantasyPlayer.Rating,
                        PositionGroup = f.FantasyPlayer.PositionGroup,
                        Player = f.FantasyPlayer.Player,
                        PlayerId = f.FantasyPlayer.PlayerId,
                        Team = f.FantasyPlayer.Team,
                        TeamId = f.FantasyPlayer.TeamId
                    },
                    IsFlex = fantasySelections.First(s => s.FantasyPlayer.PlayerId == f.FantasyPlayer.PlayerId).IsFlex,
                    Points = 0
                }).ToList();

            return playerPerformances.Concat(missingPlayers).ToList();
        }

        public void CreateFantasyTeam(FantasyTeam fantasyTeam, ulong steamId)
        {
            CheckHasTournamentStarted((int)fantasyTeam.TournamentId);
            fantasyTeam.Player = null;
            fantasyTeam.PlayerId = _coachBotContext.Players.Single(p => p.SteamID == steamId).Id;
            fantasyTeam.IsComplete = false;
            fantasyTeam.FantasyTeamSelections = null;
            fantasyTeam.Tournament = null;
            _coachBotContext.FantasyTeams.Add(fantasyTeam);
            _coachBotContext.SaveChanges();
        }

        public void UpdateFantasyTeam(FantasyTeam fantasyTeam, ulong steamId)
        {
            CanUpdateTeam(fantasyTeam.Id, steamId);
            var existingFantasyTeam = _coachBotContext.FantasyTeams.Find(fantasyTeam.Id);
            existingFantasyTeam.Name = fantasyTeam.Name;
            _coachBotContext.SaveChanges();
        }

        public void DeleteFantasyTeam(int fantasyTeamId, ulong steamId)
        {
            CanUpdateTeam(fantasyTeamId, steamId);
            var fantasyTeam = _coachBotContext.FantasyTeams.Find(fantasyTeamId);
            _coachBotContext.FantasyTeams.Remove(fantasyTeam);
            _coachBotContext.SaveChanges();
        }

        public void AddFantasyTeamSelection(FantasyTeamSelection fantasyTeamSelection, ulong steamId)
        {
            CanUpdateTeam((int)fantasyTeamSelection.FantasyTeamId, steamId);
            CanAddPlayer(fantasyTeamSelection);
            fantasyTeamSelection.FantasyPlayer = null;
            fantasyTeamSelection.FantasyTeam = null;

            if (_coachBotContext.FantasyTeams.Any(f => f.Id == fantasyTeamSelection.FantasyTeamId && f.FantasyTeamSelections.Count() == SQUAD_SIZE - 1))
            {
                var fantasyTeam = _coachBotContext.FantasyTeams.Single(f => f.Id == fantasyTeamSelection.FantasyTeamId);
                fantasyTeam.IsComplete = true;
            }

            _coachBotContext.FantasyTeamSelections.Add(fantasyTeamSelection);
            _coachBotContext.SaveChanges();
        }

        public void RemoveFantasyTeamSelection(int fantasyTeamSelectionId, ulong steamId)
        {
            var selection = _coachBotContext.FantasyTeamSelections.Find(fantasyTeamSelectionId);
            CanUpdateTeam((int)selection.FantasyTeamId, steamId);

            if (_coachBotContext.FantasyTeams.Any(f => f.Id == selection.FantasyTeamId && f.FantasyTeamSelections.Count() == 11))
            {
                var fantasyTeam = _coachBotContext.FantasyTeams.Single(f => f.Id == selection.FantasyTeamId);
                fantasyTeam.IsComplete = false;
            }

            _coachBotContext.FantasyTeamSelections.Remove(selection);
            _coachBotContext.SaveChanges();
        }

        public Model.Dtos.PagedResult<FantasyPlayer> GetFantasyPlayers(int page, int pageSize, string sortOrder, PlayerStatisticFilters playerStatisticFilters)
        {
            return GetFantasyPlayersQueryable(playerStatisticFilters).GetPaged(page, pageSize, sortOrder);
        }

        public List<Tournament> GetAvailableTournamentsForUser(ulong steamUserId)
        {
            return _coachBotContext.Tournaments
                .Include(t => t.TournamentSeries)
                .Where(t => !_coachBotContext.Matches.Any(m => m.TournamentId == t.Id && m.KickOff < DateTime.UtcNow))
                .Where(t => !_coachBotContext.FantasyTeams.Any(ft => ft.TournamentId == t.Id && ft.Player.SteamID == steamUserId))
                .Where(t => _coachBotContext.FantasyPlayers.Any(f => f.TournamentId == t.Id))
                .Where(t => t.IsPublic)
                .ToList();
        }

        public IList<FantasyTeamRank> GetFantasyTeamRankings(int tournamentId)
        {
            return _coachBotContext
                 .FantasyTeamRanks
                 .FromSql($@"SELECT FantasyTeams.Id AS FantasyTeamId,
                                    FantasyTeams.Name AS FantasyTeamName,
                                    Players.Id AS PlayerId,
                                    Players.Name AS PlayerName,
                                    FantasyTeams.TournamentId AS TournamentId,
                                    ISNULL(SUM(FantasyPlayerPhases.Points), 0) AS Points,
                                    CONVERT(INT, DENSE_RANK() OVER(ORDER BY SUM(FantasyPlayerPhases.Points) DESC)) AS Rank
                            FROM dbo.FantasyTeams FantasyTeams
                            INNER JOIN dbo.FantasyTeamSelections FantasyTeamSelections
                                ON FantasyTeams.Id = FantasyTeamSelections.FantasyTeamId
                            INNER JOIN dbo.Players Players
                                ON Players.Id = FantasyTeams.PlayerId
                            LEFT OUTER JOIN dbo.FantasyPlayerPhases FantasyPlayerPhases
                                ON FantasyPlayerPhases.FantasyPlayerId = FantasyTeamSelections.FantasyPlayerId
                            WHERE FantasyTeams.TournamentId = {tournamentId} AND (SELECT COUNT(*) FROM dbo.FantasyTeamSelections WHERE FantasyTeamId = FantasyTeams.Id) = 11
                            GROUP BY FantasyTeams.Id, FantasyTeams.Name, FantasyTeams.TournamentId, Players.Id, Players.Name
                            ORDER BY SUM(POINTS) DESC")
                .ToList();
        }

        public IList<FantasyTeamRank> GetFantasyTeamPhaseRankings(int tournamentPhaseId)
        {
            return _coachBotContext
                    .FantasyTeamRanks
                    .FromSql($@"SELECT FantasyTeams.Id AS FantasyTeamId,
                                    FantasyTeams.Name AS FantasyTeamName,
                                    Players.Id AS PlayerId,
                                    Players.Name AS PlayerName,
                                    FantasyTeams.TournamentId AS TournamentId,
                                    SUM(FantasyPlayerPhases.Points) AS Points,
                                    CONVERT(INT, DENSE_RANK() OVER(ORDER BY SUM(FantasyPlayerPhases.Points) DESC)) AS Rank
                            FROM dbo.FantasyTeams FantasyTeams
                            INNER JOIN dbo.FantasyTeamSelections FantasyTeamSelections
                                ON FantasyTeams.Id = FantasyTeamSelections.FantasyTeamId
                            INNER JOIN dbo.FantasyPlayerPhases FantasyPlayerPhases
                                ON FantasyPlayerPhases.FantasyPlayerId = FantasyTeamSelections.FantasyPlayerId
                            INNER JOIN dbo.Players Players
                                ON Players.Id = FantasyTeams.PlayerId
                            WHERE FantasyPlayerPhases.TournamentPhaseId = {tournamentPhaseId} AND (SELECT COUNT(*) FROM dbo.FantasyTeamSelections WHERE FantasyTeamId = FantasyTeams.Id) = 11
                            GROUP BY FantasyTeams.Id, FantasyTeams.Name, FantasyTeams.TournamentId, FantasyPlayerPhases.TournamentPhaseId, Players.Id, Players.Name
                            ORDER BY SUM(POINTS) DESC")
                   .ToList();
        }

        public IList<FantasyPlayerRank> GetFantasyPlayerRankings(int tournamentId)
        {
            return _coachBotContext
                    .FantasyPlayerRanks
                    .FromSql($@"SELECT  Players.Id AS PlayerId,
                                        Players.Name AS PlayerName,
                                        FantasyPlayers.Rating,
                                        ISNULL(SUM(FantasyPlayerPhases.Points), 0) AS Points,
                                        CONVERT(INT, DENSE_RANK() OVER(ORDER BY SUM(FantasyPlayerPhases.Points) DESC)) AS Rank,
                                        ISNULL(SUM(PlayerMatchStatistics.Goals), 0) AS Goals,
                                        ISNULL(SUM(PlayerMatchStatistics.Assists), 0)  AS Assists,
                                        ISNULL(SUM(CASE WHEN PlayerMatchStatistics.GoalsConceded = 0 THEN 1 ELSE 0 END), 0)  AS CleanSheets,
                                        ISNULL(SUM(PlayerMatchStatistics.OwnGoals), 0)  AS OwnGoals,
                                        ISNULL(SUM(PlayerMatchStatistics.YellowCards), 0)  AS YellowCards,
                                        ISNULL(SUM(PlayerMatchStatistics.RedCards), 0)  AS RedCards,
                                        ISNULL(SUM(PlayerMatchStatistics.KeeperSaves), 0)  AS KeeperSaves,
                                        ISNULL(SUM(PlayerMatchStatistics.GoalsConceded), 0) AS GoalsConceded,
                                        ISNULL(SUM(PlayerMatchStatistics.SecondsPlayed), 0) AS SecondsPlayed,
                                        (SELECT COUNT(*) FROM dbo.FantasyTeamSelections WHERE FantasyPlayerId = FantasyPlayers.Id) AS PickCount
                                FROM dbo.FantasyPlayers FantasyPlayers
                                INNER JOIN dbo.Players Players
                                    ON Players.Id = FantasyPlayers.PlayerId
                                LEFT OUTER JOIN dbo.FantasyPlayerPhases FantasyPlayerPhases
                                    ON FantasyPlayerPhases.FantasyPlayerId = FantasyPlayers.Id
                                LEFT OUTER JOIN dbo.PlayerMatchStatistics PlayerMatchStatistics
                                    ON PlayerMatchStatistics.Id = FantasyPlayerPhases.PlayerMatchStatisticsId
                                WHERE FantasyPlayers.TournamentId = {tournamentId}
                                GROUP BY Players.Id, Players.Name, FantasyPlayers.Rating, FantasyPlayers.Id
                                ORDER BY SUM(POINTS) DESC")
                   .ToList();
        }

        public IList<FantasyPlayerRank> GetFantasyPlayerPhaseRankings(int tournamentPhaseId)
        {
            return _coachBotContext
                    .FantasyPlayerRanks
                    .FromSql($@"SELECT  Players.Id AS PlayerId,
                                        Players.Name AS PlayerName,
                                        FantasyPlayers.Rating,
                                        SUM(FantasyPlayerPhases.Points) AS Points,
                                        CONVERT(INT, DENSE_RANK() OVER(ORDER BY SUM(FantasyPlayerPhases.Points) DESC)) AS Rank,
                                        SUM(PlayerMatchStatistics.Goals) AS Goals,
                                        SUM(PlayerMatchStatistics.Assists) AS Assists,
                                        SUM(CASE WHEN PlayerMatchStatistics.GoalsConceded = 0 THEN 1 ELSE 0 END) AS CleanSheets,
                                        SUM(PlayerMatchStatistics.OwnGoals) AS OwnGoals,
                                        SUM(PlayerMatchStatistics.YellowCards) AS YellowCards,
                                        SUM(PlayerMatchStatistics.RedCards) AS RedCards,
                                        SUM(PlayerMatchStatistics.KeeperSaves) AS KeeperSaves,
                                        SUM(PlayerMatchStatistics.GoalsConceded) AS GoalsConceded,
                                        SUM(PlayerMatchStatistics.SecondsPlayed) AS SecondsPlayed,
                                        NULL AS PickCount
                                FROM dbo.FantasyPlayers FantasyPlayers
                                INNER JOIN dbo.FantasyPlayerPhases FantasyPlayerPhases
                                    ON FantasyPlayerPhases.FantasyPlayerId = FantasyPlayers.Id
                                INNER JOIN dbo.Players Players
                                    ON Players.Id = FantasyPlayers.PlayerId
                                INNER JOIN dbo.PlayerMatchStatistics PlayerMatchStatistics
                                    ON PlayerMatchStatistics.Id = FantasyPlayerPhases.PlayerMatchStatisticsId
                                WHERE FantasyPlayerPhases.TournamentPhaseId = {tournamentPhaseId}
                                GROUP BY Players.Id, Players.Name, FantasyPlayers.Rating
                                ORDER BY SUM(POINTS) DESC")
                   .ToList();
        }

        public FantasyPlayerRank GetFantasyPlayerRankSpotlight(int tournamentId)
        {
            // INFO: If tournament has ended, just return the highest ranked
            if (_coachBotContext.Tournaments.Any(t => t.Id == tournamentId && t.EndDate != null)) {
                return GetFantasyPlayerRankings(tournamentId).FirstOrDefault();
            }

            var recentPhase = _coachBotContext.FantasyPlayerPhases
                .Where(f => f.TournamentPhase.TournamentStage.TournamentId == tournamentId && f.TournamentPhase.TournamentGroupMatches.Any(m => m.Match.MatchStatistics != null))
                .OrderByDescending(f => f.Id).FirstOrDefault();

            if (recentPhase == null) return null;

            return GetFantasyPlayerPhaseRankings((int)recentPhase.TournamentPhaseId).FirstOrDefault();
        }

        public FantasyTeamRank GetFantasyTeamRankSpotlight(int tournamentId)
        {
            // INFO: If tournament has ended, just return the highest ranked
            if (_coachBotContext.Tournaments.Any(t => t.Id == tournamentId && t.EndDate != null))
            {
                return GetFantasyTeamRankings(tournamentId).FirstOrDefault();
            }

            var recentPhase = _coachBotContext.FantasyPlayerPhases
                .Where(f => f.TournamentPhase.TournamentStage.TournamentId == tournamentId && f.TournamentPhase.TournamentGroupMatches.Any(m => m.Match.MatchStatistics != null))
                .OrderByDescending(f => f.Id).FirstOrDefault();

            if (recentPhase == null) return null;

            return GetFantasyTeamPhaseRankings((int)recentPhase.TournamentPhaseId).FirstOrDefault();
        }

        public void GenerateFantasyPlayersSnapshot(int tournamentId)
        {
            if (!_coachBotContext.TournamentGroupTeams.Any(t => t.TournamentGroup.TournamentStage.TournamentId == tournamentId))
            {
                throw new Exception("There are no teams added to this tournament yet");
            }

            var players = _coachBotContext.Players
                .Include(p => p.Teams)
                .Where(p => 
                    p.Rating > 0 &&
                    p.Teams.Any(pt => pt.IsCurrentTeam && _coachBotContext.TournamentGroupTeams.Any(tgt => tgt.TournamentGroup.TournamentStage.TournamentId == tournamentId && tgt.TeamId == pt.TeamId)) &&
                    !_coachBotContext.FantasyPlayers.Any(fp => fp.PlayerId == p.Id && fp.TournamentId == tournamentId)
                );

            foreach (var player in players)
            {
                var teamId = player.Teams.Where(t => t.IsCurrentTeam && _coachBotContext.TournamentGroupTeams.Any(tg => tg.TeamId == t.TeamId && tg.TournamentGroup.TournamentStage.TournamentId == tournamentId)).Select(t => t.TeamId).First();
                var fantasyPlayer = new FantasyPlayer()
                {
                    TournamentId = tournamentId,
                    PlayerId = player.Id,
                    PositionGroup = GetPositionGroup(player.Id, teamId),
                    Rating = player.Rating,
                    TeamId = teamId
                };
                _coachBotContext.FantasyPlayers.Add(fantasyPlayer);
            }

            _coachBotContext.SaveChanges();
        }

        private PositionGroup GetPositionGroup(int playerId, int teamId)
        {
            var position = _playerProfileService.GetMostCommonPosition(playerId, teamId);

            if (position == null)
            {
                return PositionGroup.Unknown;
            }

            return MapPositionToPositionGroup(position.Name);
        }

        private static float GetRandomRating()
        {
            Random r = new Random();
            string beforePoint = r.Next(4, 9).ToString();
            string afterPoint = r.Next(0, 9).ToString();
            string combined = beforePoint + "." + afterPoint;
            return float.Parse(combined);
        }

        private static PositionGroup GetRandomPositionGroup()
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
            var position = playerPositionMatchStatistics.OrderByDescending(p => p.SecondsPlayed).Select(p => p.Position.Name).First();

            return MapPositionToPositionGroup(position);
        }

        private PositionGroup MapPositionToPositionGroup(string positionName)
        {
            var defPositions = new string[] { "LWB", "LB", "LCB", "SWP", "CB", "RCB", "RB", "RWB" };
            var midPositions = new string[] { "LM", "LCM", "CDM", "CM", "CAM", "RCM", "RM" };
            var attackPositions = new string[] { "LW", "LF", "CF", "SS", "ST", "RF", "RW" };

            if (positionName == "GK")
            {
                return PositionGroup.Goalkeeper;
            }
            else if (defPositions.Any(p => p == positionName))
            {
                return PositionGroup.Defence;
            }
            else if (midPositions.Any(p => p == positionName))
            {
                return PositionGroup.Midfield;
            }
            else if (attackPositions.Any(p => p == positionName))
            {
                return PositionGroup.Attack;
            }

            return PositionGroup.Unknown;
        }

        private IQueryable<FantasyPlayer> GetFantasyPlayersQueryable(PlayerStatisticFilters playerStatisticFilters)
        {
            var queryable = _coachBotContext
                    .FantasyPlayers
                    .AsNoTracking()
                    .Include(fp => fp.Player)
                    .Include(t => t.Team)
                    .ThenInclude(t => t.BadgeImage)
                    .Where(p => p.PositionGroup != PositionGroup.Unknown)
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

        private void CanUpdateTeam(int fantasyTeamId, ulong steamId)
        {
            var fantasyTeam = _coachBotContext.FantasyTeams.Include(f => f.Player).Single(f => f.Id == fantasyTeamId);

            if (fantasyTeam.Player.SteamID != steamId)
            {
                throw new UnauthorizedAccessException("This team is owned by a different player");
            }

            CheckHasTournamentStarted((int)fantasyTeam.TournamentId);
        }

        private void CanAddPlayer(FantasyTeamSelection fantasyTeamSelection)
        {
            var fantasyTeam = _coachBotContext.FantasyTeams
                .Include(f => f.FantasyTeamSelections)
                .ThenInclude(f => f.FantasyPlayer)
                .Include(f => f.Tournament)
                .Single(f => f.Id == fantasyTeamSelection.FantasyTeamId);
            var fantasyPlayer = _coachBotContext.FantasyPlayers.Find(fantasyTeamSelection.FantasyPlayerId);
            var positionGroup = fantasyTeamSelection.FantasyPlayer.PositionGroup;
            var isFlex = fantasyTeamSelection.IsFlex;

            // Unknown position group check
            if (fantasyPlayer.PositionGroup == PositionGroup.Unknown)
            {
                throw new Exception("This player has no known position group");
            }

            // Check player not already in team
            if (fantasyTeam.FantasyTeamSelections.Any(f => f.FantasyPlayerId == fantasyPlayer.Id))
            {
                throw new Exception("This player is already in the team");
            }

            // Check no more than 3 players from a given team
            if (fantasyTeam.FantasyTeamSelections.Count(f => f.FantasyPlayer.TeamId == fantasyPlayer.TeamId) == 3)
            {
                throw new Exception("The limit for players from this team has been exceeded");
            }

            // Check not over budget
            if (Math.Round(fantasyTeam.FantasyTeamSelections.Sum(f => f.FantasyPlayer.Rating), 2) + fantasyPlayer.Rating > fantasyTeam.Tournament.FantasyPointsLimit)
            {
                throw new Exception("The budget for the team would be exceeded if this player were added");
            }

            // Check not over position counts
            if (isFlex && fantasyTeam.FantasyTeamSelections.Count(f => f.IsFlex) == 3)
            {
                throw new Exception("The limit of flex players has already been met for this team");
            }
            else if (!isFlex && fantasyTeam.FantasyTeamSelections.Count(f => f.FantasyPlayer.PositionGroup == positionGroup && !f.IsFlex) == GetPositionGroupLimit(positionGroup))
            {
                throw new Exception($"The limit of {positionGroup.ToString()} players has already been met for this team");
            }
        }

        private int GetPositionGroupLimit(PositionGroup positionGroup)
        {
            switch (positionGroup)
            {
                case PositionGroup.Goalkeeper:
                    return MAX_GKS;

                case PositionGroup.Defence:
                    return MAX_DEFS;

                case PositionGroup.Midfield:
                    return MAX_MIDS;

                case PositionGroup.Attack:
                    return MAX_ATTACKERS;
            }

            throw new Exception("Unknown position group");
        }

        private void CheckHasTournamentStarted(int tournamentId)
        {
            if (_coachBotContext.Matches.Any(m => m.TournamentId == tournamentId && m.KickOff < DateTime.UtcNow))
            {
                throw new Exception("The tournament has started. Fantasy teams cannot be updated.");
            }
        }
    }
}