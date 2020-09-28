using CoachBot.Database;
using CoachBot.Domain.Helpers;
using CoachBot.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class TournamentService
    {
        private readonly CoachBotContext _coachBotContext;

        public TournamentService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public List<TournamentSeries> GetTournamentSeries(bool excludeInactive = false)
        {
            return _coachBotContext.TournamentSeries
                .Include(t => t.Tournaments)
                .Where(t => !excludeInactive || t.IsActive)
                .ToList();
        }

        public TournamentSeries GetTournamentSeries(int tournamentSeriesId)
        {
            return _coachBotContext.TournamentSeries
                .Include(t => t.Tournaments)
                .Single(t => t.Id == tournamentSeriesId);
        }

        public List<Tournament> GetTournaments(bool excludeInactive = false)
        {
            return _coachBotContext.Tournaments
                .Include(t => t.TournamentSeries)
                .ThenInclude(t => t.Organisation)
                .ThenInclude(o => o.LogoImage)
                .Where(t => !excludeInactive || (t.EndDate == null || t.EndDate > DateTime.UtcNow))
                .Where(t => t.IsPublic)
                .ToList();
        }

        public List<Tournament> GetPastTournaments()
        {
            return _coachBotContext.Tournaments
                .Include(t => t.TournamentSeries)
                    .ThenInclude(t => t.Organisation)
                    .ThenInclude(o => o.LogoImage)
                .Include(t => t.WinningTeam)
                .ThenInclude(t => t.BadgeImage)
                .Where(t => t.EndDate < DateTime.UtcNow)
                .Where(t => t.IsPublic)
                .ToList();
        }

        public Tournament GetTournamentSimple(int tournamentId)
        {
            return _coachBotContext.Tournaments.Single(t => t.Id == tournamentId);
        }

        public Tournament GetTournament(int tournamentId)
        {
            return _coachBotContext.Tournaments
                .Include(t => t.TournamentSeries)
                    .ThenInclude(t => t.TournamentLogo)
                .Include(t => t.TournamentSeries)
                    .ThenInclude(t => t.Organisation)
                    .ThenInclude(t => t.LogoImage)
                .Include(t => t.TournamentStaff)
                    .ThenInclude(t => t.Player)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentPhases)
                    .ThenInclude(t => t.TournamentGroupMatches)
                    .ThenInclude(t => t.Match)
                    .ThenInclude(t => t.TeamHome)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentPhases)
                    .ThenInclude(t => t.TournamentGroupMatches)
                    .ThenInclude(t => t.Match)
                    .ThenInclude(t => t.TeamAway)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupTeams)
                    .ThenInclude(t => t.Team)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupMatches)
                    .ThenInclude(t => t.Match)
                    .ThenInclude(t => t.TeamHome)
                    .ThenInclude(t => t.BadgeImage)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupMatches)
                    .ThenInclude(t => t.Match)
                    .ThenInclude(t => t.TeamAway)
                    .ThenInclude(t => t.BadgeImage)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupMatches)
                    .ThenInclude(t => t.Match)
                    .ThenInclude(t => t.MatchStatistics)
                .Include(t => t.WinningTeam)
                .Single(t => t.Id == tournamentId);
        }

        public List<Tournament> GetTournamentsForTeam(int teamId)
        {
            return _coachBotContext.Tournaments
                .Include(t => t.TournamentSeries)
                    .ThenInclude(t => t.Organisation)
                    .ThenInclude(o => o.LogoImage)
                .Include(t => t.WinningTeam)
                    .ThenInclude(t => t.BadgeImage)
                .Where(t => _coachBotContext.TournamentGroupTeams.Any(tgt => tgt.TeamId == teamId && tgt.TournamentGroup.TournamentStage.TournamentId == t.Id))
                .Where(t => t.IsPublic)
                .OrderByDescending(t => t.StartDate)
                .ToList();
        }

        public List<Organisation> GetOrganisations()
        {
            return _coachBotContext.Organisations.ToList();
        }

        public Organisation GetOrganisation(int id)
        {
            return _coachBotContext.Organisations.Single(o => o.Id == id);
        }

        public void CreateOrganisation(Organisation organisation)
        {
            _coachBotContext.Organisations.Add(organisation);
            _coachBotContext.SaveChanges();
        }

        public void UpdateOrganisation(Organisation organisation)
        {
            var existingOrganisation = _coachBotContext.Organisations.Single(o => o.Id == organisation.Id);

            existingOrganisation.LogoImageId = organisation.LogoImageId;
            existingOrganisation.Name = organisation.Name;
            existingOrganisation.Acronym = organisation.Acronym;
            existingOrganisation.BrandColour = organisation.BrandColour;

            _coachBotContext.SaveChanges();
        }

        public void RemoveOrganisation(int id)
        {
            var organisation = _coachBotContext.Organisations.Single(o => o.Id == id);
            _coachBotContext.Organisations.Remove(organisation);
            _coachBotContext.SaveChanges();
        }

        public void CreateTournamentSeries(TournamentSeries tournament)
        {
            _coachBotContext.TournamentSeries.Add(tournament);
            _coachBotContext.SaveChanges();
        }

        public void CreateTournament(Tournament tournament, ulong? steamId = null)
        {
            tournament.IsPublic = false;
            _coachBotContext.Tournaments.Add(tournament);
            _coachBotContext.SaveChanges();

            if (steamId != null)
            {
                var player = _coachBotContext.Players.Single(p => p.SteamID == steamId);
                var staff = new TournamentStaff()
                {
                    PlayerId = player.Id,
                    TournamentId = tournament.Id,
                    Role = TournamentStaffRole.Organiser
                };
                _coachBotContext.TournamentStaff.Add(staff);
                _coachBotContext.SaveChanges();
            }

            switch (tournament.TournamentType)
            {
                case TournamentType.RoundRobin:
                    GenerateRoundRobinTournament(tournament.Id);
                    break;

                case TournamentType.Knockout:
                    GenerateKnockoutTournament(tournament.Id);
                    break;

                case TournamentType.RoundRobinAndKnockout:
                    GenerateRoundRobinAndKnockoutTournament(tournament.Id);
                    break;

                default:
                    break;
            }
        }

        public void UpdateTournament(Tournament tournament)
        {
            var existingTournament = _coachBotContext.Tournaments.Single(m => m.Id == tournament.Id);
            existingTournament.IsPublic = tournament.IsPublic;
            existingTournament.StartDate = tournament.StartDate;

            var tournamentSeries = _coachBotContext.TournamentSeries.Single(ts => ts.Tournaments.Any(t => ts.Id == tournament.TournamentSeriesId));
            if (tournament.IsPublic)
            {
                tournamentSeries.IsPublic = true;
            }

            _coachBotContext.SaveChanges();
        }

        public List<Team> GetTournamentTeams(int tournamentId)
        {
            return _coachBotContext.TournamentGroupTeams
                .Where(t => t.TournamentGroup.TournamentStage.TournamentId == tournamentId)
                .Select(t => t.Team)
                .Distinct()
                .OrderBy(t => t.Name)
                .ToList();
        }

        public TournamentStage GetTournamentStage(int tournamentStageId)
        {
            return _coachBotContext.TournamentStages.FirstOrDefault(t => t.Id == tournamentStageId);
        }

        public void UpdateTournamentStage(TournamentStage tournamentStage)
        {
            _coachBotContext.TournamentStages.Update(tournamentStage);
            _coachBotContext.SaveChanges();
        }

        public List<Match> GetTournamentPhaseMatches(int tournamentPhaseId, bool futureOnly = false)
        {
            return _coachBotContext.Matches
                .Where(m => _coachBotContext.TournamentPhases.Any(p => p.TournamentGroupMatches.Any(gm => gm.TournamentPhaseId == tournamentPhaseId)))
                .Where(m => !futureOnly || m.KickOff > DateTime.UtcNow)
                .ToList();
        }

        public TournamentPhase GetCurrentTournamentPhase(int tournamentId)
        {
            return _coachBotContext.TournamentGroupMatches
                .Where(tg => tg.TournamentGroup.TournamentStage.TournamentId == tournamentId)
                .Where(m => m.Match.KickOff > DateTime.UtcNow)
                .OrderBy(m => m.Match.KickOff)
                .Select(m => m.TournamentPhase)
                .Include(p => p.TournamentStage)
                .Include(t => t.TournamentGroupMatches)
                    .ThenInclude(m => m.Match)
                    .ThenInclude(m => m.TeamHome)
                    .ThenInclude(m => m.BadgeImage)
                .Include(t => t.TournamentGroupMatches)
                    .ThenInclude(m => m.Match)
                    .ThenInclude(m => m.TeamAway)
                    .ThenInclude(m => m.BadgeImage)
                .Include(t => t.TournamentGroupMatches)
                    .ThenInclude(t => t.TournamentGroup)
                .FirstOrDefault();
        }

        public void UpdateTournamentPhase(TournamentPhase tournamentPhase)
        {
            _coachBotContext.TournamentPhases.Update(tournamentPhase);
            _coachBotContext.SaveChanges();
        }

        public List<TournamentStaff> GetTournamentStaff(int tournamentId)
        {
            return _coachBotContext.TournamentStaff
                .Include(t => t.Player)
                .Include(t => t.Tournament)
                .Where(g => g.TournamentId == tournamentId)
                .ToList();
        }

        public TournamentStaff GetTournamentStaffMember(int tournamentStaffId)
        {
            return _coachBotContext.TournamentStaff
                .Include(t => t.Player)
                .Include(t => t.Tournament)
                .First(s => s.Id == tournamentStaffId);
        }

        public void CreateTournamentStaff(TournamentStaff tournamentStaff)
        {
            _coachBotContext.TournamentStaff.Add(tournamentStaff);
            _coachBotContext.SaveChanges();
        }

        public void UpdateTournamentStaff(TournamentStaff tournamentStaff)
        {
            var existing = _coachBotContext.TournamentStaff.Single(t => t.Id == tournamentStaff.Id);
            existing.Role = tournamentStaff.Role;
            _coachBotContext.TournamentStaff.Update(existing);
            _coachBotContext.SaveChanges();
        }

        public void DeleteTournamentStaff(int id)
        {
            var tournamentStaff = _coachBotContext.TournamentStaff.Find(id);
            _coachBotContext.TournamentStaff.Remove(tournamentStaff);
            _coachBotContext.SaveChanges();
        }

        public List<TournamentGroup> GetTournamentGroups(int tournamentId)
        {
            return _coachBotContext.TournamentGroups
                .Include(t => t.TournamentGroupTeams)
                    .ThenInclude(tgt => tgt.Team)
                .Where(g => g.TournamentStage.TournamentId == tournamentId)
                .ToList();
        }

        public TournamentGroup GetTournamentGroup(int tournamentGroupId)
        {
            return _coachBotContext.TournamentGroups
                .Include(g => g.TournamentStage)
                .First(g => g.Id == tournamentGroupId);
        }

        public void CreateTournamentGroup(TournamentGroup tournamentGroup)
        {
            _coachBotContext.TournamentGroups.Add(tournamentGroup);
            _coachBotContext.SaveChanges();
        }

        public void UpdateTournamentGroup(TournamentGroup tournamentGroup)
        {
            _coachBotContext.TournamentGroups.Update(tournamentGroup);
            _coachBotContext.SaveChanges();
        }

        public void DeleteTournamentGroup(int id)
        {
            var tournamentGroup = _coachBotContext.TournamentGroups.Find(id);
            _coachBotContext.TournamentGroups.Remove(tournamentGroup);
            _coachBotContext.SaveChanges();
        }

        public void AddTournamentTeam(int teamId, int tournamentGroupId)
        {
            var tournamentStageId = _coachBotContext.TournamentGroups.Where(t => t.Id == tournamentGroupId).Select(t => t.TournamentStage.Id).Single();
            var tournamentStageTeams = _coachBotContext.TournamentGroupTeams.Where(t => t.TournamentGroup.TournamentStageId == tournamentStageId).Select(t => t.TeamId);

            if (tournamentStageTeams.Any(t => t == teamId))
            {
                throw new Exception("A team can only be in one group for any given tournament stage");
            }

            var team = _coachBotContext.Teams.First(t => t.Id == teamId);
            if (team.Inactive)
            {
                throw new Exception("Team must be active to join a tournament");
            }

            var tournamentGroupTeam = new TournamentGroupTeam()
            {
                TeamId = teamId,
                TournamentGroupId = tournamentGroupId
            };
            _coachBotContext.TournamentGroupTeams.Add(tournamentGroupTeam);
            _coachBotContext.SaveChanges();
        }

        public void RemoveTournamentTeam(int teamId, int tournamentGroupId)
        {
            var tournamentGroupTeam = _coachBotContext.TournamentGroupTeams.FirstOrDefault(t => t.TeamId == teamId && t.TournamentGroupId == tournamentGroupId);
            _coachBotContext.TournamentGroupTeams.Remove(tournamentGroupTeam);
            _coachBotContext.SaveChanges();
        }

        public void AddTournamentMatch(Match match, int tournamentGroupId, int tournamentPhaseId)
        {
            var tournamentId = _coachBotContext.TournamentPhases.Where(t => t.Id == tournamentPhaseId).Select(t => t.TournamentStage.TournamentId).First();
            match.TournamentId = tournamentId;
            match.MatchType = MatchType.Competition;
            _coachBotContext.Add(match);

            var tournamentGroupMatch = new TournamentGroupMatch()
            {
                Match = match,
                TournamentGroupId = tournamentGroupId,
                TournamentPhaseId = tournamentPhaseId
            };
            _coachBotContext.TournamentGroupMatches.Add(tournamentGroupMatch);
            _coachBotContext.SaveChanges();
        }

        public List<TournamentMatchDaySlot> GetTournamentMatchDaySlots(int tournamentId)
        {
            return _coachBotContext.TournamentMatchDays.Where(t => t.TournamentId == tournamentId).ToList();
        }

        public void CreateTournamentMatchDaySlot(TournamentMatchDaySlot tournamentMatchDaySlot)
        {
            tournamentMatchDaySlot.MatchTime = DateTimeHelper.RemoveSeconds(tournamentMatchDaySlot.MatchTime);
            _coachBotContext.TournamentMatchDays.Add(tournamentMatchDaySlot);
            _coachBotContext.SaveChanges();
        }

        public void DeleteTournamentMatchDaySlot(int tournamentMatchDaySlot)
        {
            var matchDaySlot = _coachBotContext.TournamentMatchDays.Find(tournamentMatchDaySlot);
            _coachBotContext.TournamentMatchDays.Remove(matchDaySlot);
            _coachBotContext.SaveChanges();
        }

        public List<TournamentGroupStanding> GetTournamentGroupStandings(int tournamentGroupId)
        {
            var statisticResults = _coachBotContext
                 .TeamMatchStatistics
                 .Where(t => _coachBotContext.TournamentGroupMatches.Any(tg => tg.MatchId == t.MatchId && tg.TournamentGroupId == tournamentGroupId))
                 .Where(t => t.Match.MatchStatistics != null)
                 .AsNoTracking()
                 .Select(m => new StandingGroupSubEntry()
                 {
                     TeamId = (int)m.TeamId,
                     GoalsConceded = m.GoalsConceded,
                     GoalsScored = m.Goals,
                     MatchOutcome = m.MatchOutcome,
                     SmallUrl = m.Team.BadgeImage.SmallUrl,
                     TeamName = m.Team.Name
                 }).ToList();

            var overrideResultsHome = _coachBotContext.Matches
                   .Where(m => _coachBotContext.TournamentGroupMatches.Any(tg => tg.MatchId == m.Id && tg.TournamentGroupId == tournamentGroupId))
                   .Where(m => m.MatchStatistics.MatchData == null && m.MatchStatistics.HomeGoals != null && m.MatchStatistics.AwayGoals != null)
                   .Select(m => new StandingGroupSubEntry()
                   {
                       TeamId = (int) m.TeamHomeId,
                       GoalsConceded = (int)m.MatchStatistics.AwayGoals,
                       GoalsScored = (int)m.MatchStatistics.HomeGoals,
                       MatchOutcome = m.MatchStatistics.GetMatchOutcomeTypeForTeam(MatchDataTeamType.Home),
                       SmallUrl = m.TeamHome.BadgeImage.SmallUrl,
                       TeamName = m.TeamHome.Name
                   }).ToList();

            var overrideResultsAway = _coachBotContext.Matches
                   .Where(m => _coachBotContext.TournamentGroupMatches.Any(tg => tg.MatchId == m.Id && tg.TournamentGroupId == tournamentGroupId))
                   .Where(m => m.MatchStatistics.MatchData == null && m.MatchStatistics.HomeGoals != null && m.MatchStatistics.AwayGoals != null)
                   .Select(m => new StandingGroupSubEntry()
                   {
                       TeamId = (int)m.TeamAwayId,
                       GoalsConceded = (int)m.MatchStatistics.HomeGoals,
                       GoalsScored = (int)m.MatchStatistics.AwayGoals,
                       MatchOutcome = m.MatchStatistics.GetMatchOutcomeTypeForTeam(MatchDataTeamType.Away),
                       SmallUrl = m.TeamAway.BadgeImage.SmallUrl,
                       TeamName = m.TeamAway.Name
                   }).ToList();

            var standings = statisticResults
                 .Concat(overrideResultsHome)
                 .Concat(overrideResultsAway)
                 .GroupBy(p => new { p.TeamId, p.TeamName, p.SmallUrl }, (key, s) => new TournamentGroupStanding()
                 {
                     GoalsScored = s.Sum(p => p.GoalsScored),
                     GoalsConceded = s.Sum(p => p.GoalsConceded),
                     MatchesPlayed = s.Count(),
                     Wins = s.Sum(p => (int)p.MatchOutcome == (int)MatchOutcomeType.Win ? 1 : 0),
                     Losses = s.Sum(p => (int)p.MatchOutcome == (int)MatchOutcomeType.Loss ? 1 : 0),
                     Draws = s.Sum(p => (int)p.MatchOutcome == (int)MatchOutcomeType.Draw ? 1 : 0),
                     TeamId = (int)key.TeamId,
                     TeamName = key.TeamName,
                     BadgeImageUrl = key.SmallUrl,
                     GoalDifference = s.Sum(p => p.GoalsScored) - s.Sum(p => p.GoalsConceded),
                     Points = s.Sum(p => p.MatchOutcome == MatchOutcomeType.Win ? 3 : p.MatchOutcome == MatchOutcomeType.Draw ? 1 : 0)
                 })
                 .ToList()
                 .OrderByDescending(s => s.Points)
                 .ThenByDescending(s => s.GoalDifference);

            var unpositioned = _coachBotContext
                 .TournamentGroupTeams
                 .Where(t => t.TournamentGroupId == tournamentGroupId)
                 .Where(t => !standings.Any(s => s.TeamId == t.TeamId))
                 .Select(t => new TournamentGroupStanding()
                 {
                     TeamId = t.TeamId, 
                     TeamName = t.Team.Name,
                     BadgeImageUrl = t.Team.BadgeImage.SmallUrl
                 }).ToList();

            return standings.Concat(unpositioned)
                 .Select((s, i) => new TournamentGroupStanding()
                 {
                     GoalsScored = s.GoalsScored,
                     GoalsConceded = s.GoalsConceded,
                     MatchesPlayed = s.MatchesPlayed,
                     Wins = s.Wins,
                     Losses = s.Losses,
                     Draws = s.Draws,
                     TeamId = s.TeamId,
                     TeamName = s.TeamName,
                     BadgeImageUrl = s.BadgeImageUrl,
                     GoalDifference = s.GoalDifference,
                     Points = s.Points,
                     Position = i + 1
                 }).ToList();
        }

        public struct StandingGroupSubEntry
        {
            public int TeamId { get; set; }
            public int GoalsScored {get; set; }
            public int GoalsConceded { get; set; }
            public MatchOutcomeType MatchOutcome { get; set; }
            public string SmallUrl { get; set; }
            public string TeamName { get; set; }
        }

        public void GenerateTournamentSchedule(int tournamentId, int? tournamentStageId = null)
        {
            var tournament = _coachBotContext.Tournaments.First(t => t.Id == tournamentId);
            switch (tournament.TournamentType)
            {
                case TournamentType.RoundRobin:
                    GenerateRoundRobinSchedule(tournamentId);
                    break;

                case TournamentType.Knockout:
                    GenerateKnockoutSchedule(tournamentId);
                    break;

                case TournamentType.RoundRobinAndKnockout:
                    GenerateRoundRobinAndKnockoutSchedule(tournamentId);
                    break;

                default:
                    break;
            }
        }

        public void ManageTournamentProgress(int tournamentId, int matchId)
        {
            var tournament = _coachBotContext.Tournaments.First(t => t.Id == tournamentId);
            switch (tournament.TournamentType)
            {
                case TournamentType.Knockout:
                    ManageKnockoutProgress(tournamentId, matchId);
                    break;

                case TournamentType.RoundRobinAndKnockout:
                    ManageRoundRobinAndKnockoutProgress(tournamentId, matchId);
                    break;

                default:
                    break;
            }
        }

        public bool IsTournamentOrganiser(int tournamentId, ulong steamId)
        {
            return _coachBotContext.TournamentStaff.Any(t => t.TournamentId == tournamentId && t.Player.SteamID == steamId && t.Role == TournamentStaffRole.Organiser);
        }

        private DateTime GetMatchDaySlotDate(DateTime earliestDate, TournamentMatchDaySlot tournamentMatchDaySlot)
        {
            int daysUntilMatchDay = ((int)tournamentMatchDaySlot.MatchDay - (int)earliestDate.DayOfWeek + 8) % 7;
            var matchDaySlotDate = earliestDate.AddDays(daysUntilMatchDay);

            return matchDaySlotDate.Date + tournamentMatchDaySlot.MatchTime.TimeOfDay;
        }

        private void RemoveMatchesForTournament(int tournamentId)
        {
            var tournamentMatches = _coachBotContext.TournamentGroupMatches.Where(t => t.TournamentGroup.TournamentStage.TournamentId == tournamentId);
            if (tournamentMatches.Any(t => t.Match.MatchStatisticsId != null))
            {
                throw new Exception("Cannot regenerate schedule as matches have already taken place");
            }

            if (tournamentMatches != null & tournamentMatches.Any())
            {
                _coachBotContext.TournamentGroupMatches.RemoveRange(tournamentMatches);
                var matches = _coachBotContext.Matches.Where(t => t.TournamentId == tournamentId);
                if (matches.Count() < 200)
                {
                    _coachBotContext.Matches.RemoveRange(matches);
                }
            }
            _coachBotContext.TournamentPhases.RemoveRange(_coachBotContext.TournamentPhases.Where(t => t.TournamentStage.TournamentId == tournamentId));
            _coachBotContext.SaveChanges();
        }

        // TODO: Refactor and move out into separate classes

        #region Round Robin and Knockout
        private void ManageRoundRobinAndKnockoutProgress(int tournamentId, int matchId)
        {
            var tournamentGroupMatch = _coachBotContext.TournamentGroupMatches
                .Include(t => t.TournamentPhase)
                .ThenInclude(t => t.TournamentStage)
                .Include(t => t.TournamentGroup)
                .Single(m => m.MatchId == matchId);
            var stages = _coachBotContext.TournamentStages.Include(t => t.TournamentGroups).ThenInclude(t => t.TournamentGroupTeams).Where(t => t.TournamentId == tournamentId).OrderBy(t => t.Id);
            if (tournamentGroupMatch.TournamentPhase.TournamentStageId == stages.First().Id) // INFO: This means its the group stage
            {
                var stage = stages.First();
                var groups = stage.TournamentGroups.OrderBy(t => t.Id);
                if (groups.Count() != 2) return; // INFO: We don't support knockout tournaments with more than two groups
                var phases = _coachBotContext.TournamentPhases.Where(t => t.TournamentStageId == tournamentGroupMatch.TournamentPhase.TournamentStageId).OrderBy(t => t.Id);
                var knockoutGroup = _coachBotContext.TournamentGroups.First(t => t.TournamentStageId == stages.Last().Id);
                if (tournamentGroupMatch.TournamentPhaseId != phases.Last().Id) return; // INFO: Unless its the last phase/round of matches, don't do anything
                if (_coachBotContext.TournamentGroupMatches.Where(t => t.TournamentPhase.TournamentStageId == stage.Id && t.MatchId != tournamentGroupMatch.MatchId).Any(t => t.Match.MatchStatisticsId == null)) return; // Don't do anything unless all matches are complete

                var knockoutFixtures = _coachBotContext.TournamentGroupMatches
                    .Where(t => t.TournamentGroupId == knockoutGroup.Id)
                    .Where(t => t.TournamentPhaseId == phases.OrderBy(p => p.Id).First(p => p.TournamentStageId == stages.Last().Id).Id) // INFO: First phase of knockout fixtures only
                    .Include(t => t.Match)
                    .OrderBy(t => t.Id)
                    .ToList();

                var qualifiedTeamsGroupA = new List<int>();
                var qualifiedTeamsGroupB = new List<int>();
                var numberOfQualifiers = GetNumberOfQualifyingKnockoutTeams(groups.First().TournamentGroupTeams.Count(), groups.Count());
                qualifiedTeamsGroupA.AddRange(GetTournamentGroupStandings(groups.First().Id).Select(t => t.TeamId).Take(numberOfQualifiers));
                qualifiedTeamsGroupB.AddRange(GetTournamentGroupStandings(groups.Last().Id).Select(t => t.TeamId).Take(numberOfQualifiers));

                var groupA = true;
                foreach (var knockoutFixture in knockoutFixtures)
                {
                    if (groupA)
                    {
                        knockoutFixture.Match.TeamHomeId = qualifiedTeamsGroupA.First();
                        knockoutFixture.Match.TeamAwayId = qualifiedTeamsGroupB.Last();
                        qualifiedTeamsGroupA = qualifiedTeamsGroupA.Where(t => t != knockoutFixture.Match.TeamHomeId).ToList();
                        qualifiedTeamsGroupB = qualifiedTeamsGroupB.Where(t => t != knockoutFixture.Match.TeamAwayId).ToList();
                    }
                    else
                    {
                        knockoutFixture.Match.TeamHomeId = qualifiedTeamsGroupB.First();
                        knockoutFixture.Match.TeamAwayId = qualifiedTeamsGroupA.Last();
                        qualifiedTeamsGroupA = qualifiedTeamsGroupA.Where(t => t != knockoutFixture.Match.TeamAwayId).ToList();
                        qualifiedTeamsGroupB = qualifiedTeamsGroupB.Where(t => t != knockoutFixture.Match.TeamHomeId).ToList();
                    }
                    groupA = !groupA;
                }
            }
            else
            {
                ManageKnockoutProgress(tournamentId, matchId);
            }
            _coachBotContext.SaveChanges();
        }

        private void GenerateRoundRobinAndKnockoutTournament(int tournamentId)
        {
            var tournament = _coachBotContext.Tournaments.Include(t => t.TournamentSeries).First(t => t.Id == tournamentId);
            var groupStage = new TournamentStage()
            {
                TournamentId = tournament.Id,
                Name = "Group Stage"
            };
            _coachBotContext.TournamentStages.Add(groupStage);

            var knockoutStage = new TournamentStage()
            {
                TournamentId = tournament.Id,
                Name = "Knockout Stage"
            };
            _coachBotContext.TournamentStages.Add(knockoutStage);

            _coachBotContext.SaveChanges();
        }

        private void GenerateRoundRobinAndKnockoutSchedule(int tournamentId)
        {
            var tournament = _coachBotContext.Tournaments
                .Include(t => t.TournamentSeries)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupTeams)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupMatches)
                    .ThenInclude(t => t.Match)
                .Include(t => t.TournamentMatchDays)
                .First(t => t.Id == tournamentId);

            if (tournament.StartDate == null) throw new Exception("Start date must be set");

            RemoveMatchesForTournament(tournamentId);

            var groupStage = tournament.TournamentStages.First();
            GenerateRoundRobinMatches(tournament, groupStage, tournament.StartDate.Value);

            var knockoutStage = tournament.TournamentStages.Last();
            var earliestMatchDate = _coachBotContext.TournamentGroupMatches.Where(t => t.TournamentPhase.TournamentStage.TournamentId == tournamentId).Max(m => m.Match.KickOff).Value;
            var numberOfGroups = _coachBotContext.TournamentGroups.Count(g => g.TournamentStageId == groupStage.Id);
            var numberOfTeams = _coachBotContext.TournamentGroups.Where(g => g.TournamentStageId == groupStage.Id).Max(g => g.TournamentGroupTeams.Count());

            var knockoutGroupSize = GetNumberOfQualifyingKnockoutTeams(numberOfTeams, numberOfGroups);
            var teams = new List<Team>();
            for (int i = 1; i <= knockoutGroupSize; i++)
            {
                teams.Add(new Team());
            }
            GenerateKnockoutMatches(tournament, knockoutStage, teams, (DateTime)earliestMatchDate, false);

           
        }

        private int GetNumberOfQualifyingKnockoutTeams(int groupSize, int groups = 1)
        {
            switch (groupSize)
            {
                case 3:
                    return 2 * groups;
                case 4:
                    return 2 * groups;
                case 5:
                    return 3 * groups;
                case 6:
                    return 4 * groups;
                case 7:
                    return 4 * groups;
                case 8:
                    return 5 * groups;
            }
            throw new ArgumentException("Group size not supported");
        }
        #endregion

        #region Knockout
        private void ManageKnockoutProgress(int tournamentId, int matchId)
        {
            var match = _coachBotContext.TournamentGroupMatches.Include(m => m.Match).ThenInclude(m => m.MatchStatistics).Single(m => m.MatchId == matchId);
            var currentPhase = _coachBotContext.TournamentPhases.Include(p => p.TournamentGroupMatches).Include(t => t.TournamentStage).FirstOrDefault(m => m.Id == match.TournamentPhaseId);
            var nextPhase = _coachBotContext.TournamentPhases.Include(p => p.TournamentGroupMatches).ThenInclude(m => m.Match).FirstOrDefault(m => m.Id == currentPhase.Id + 1);

            if (nextPhase != null)
            {
                var currentPhaseMatchIndex = currentPhase.TournamentGroupMatches.OrderBy(m => m.Id).ToList().FindIndex(m => m.MatchId == match.MatchId);
                if (currentPhase.TournamentGroupMatches.Count == nextPhase.TournamentGroupMatches.Count)
                {
                    var nextMatch = nextPhase.TournamentGroupMatches.ElementAt(currentPhaseMatchIndex);
                    if (nextMatch.Match.TeamAwayId == null)
                    {
                        var winner = match.Match.MatchStatistics.KnockoutMatchWinner;
                        if (winner.Equals(MatchDataTeamType.Home))
                        {
                            if (nextMatch.Match.TeamAwayId != null) throw new Exception("Unhandled knockout tournament scenario. Please investigate.");
                            nextMatch.Match.TeamAwayId = match.Match.TeamHomeId;
                        }
                        else
                        {
                            if (nextMatch.Match.TeamAwayId != null) throw new Exception("Unhandled knockout tournament scenario. Please investigate.");
                            nextMatch.Match.TeamAwayId = match.Match.TeamAwayId;
                        }
                        _coachBotContext.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("Unhandled knockout tournament scenario. Please investigate.");
                    }
                }
                else if (currentPhase.TournamentGroupMatches.Count / 2 == nextPhase.TournamentGroupMatches.Count)
                {
                    var nextMatchIndex = currentPhaseMatchIndex / 2;
                    var nextMatch = nextPhase.TournamentGroupMatches.ElementAt(nextMatchIndex);

                    int winningTeamId;
                    var winner = match.Match.MatchStatistics.KnockoutMatchWinner;
                    if (winner.Equals(MatchDataTeamType.Home))
                    {
                        winningTeamId = match.Match.TeamHomeId.Value;
                    }
                    else
                    {
                        winningTeamId = match.Match.TeamAwayId.Value;
                    }

                    if (currentPhaseMatchIndex % 2 == 0) // Even, meaning they will become the home team
                    {
                        if (nextMatch.Match.TeamAwayId != null) throw new Exception("Unhandled knockout tournament scenario. Please investigate.");
                        nextMatch.Match.TeamHomeId = winningTeamId;
                    }
                    else
                    {
                        if (nextMatch.Match.TeamHomeId != null) throw new Exception("Unhandled knockout tournament scenario. Please investigate.");
                        nextMatch.Match.TeamAwayId = winningTeamId;
                    }
                    _coachBotContext.SaveChanges();
                }
                else
                {
                    throw new Exception("Unhandled knockout tournament scenario. Please investigate.");
                }
            }
            else
            {
                var tournament = _coachBotContext.Tournaments.Single(t => t.Id == currentPhase.TournamentStage.TournamentId);

                var winner = match.Match.MatchStatistics.KnockoutMatchWinner;
                if (winner.Equals(MatchDataTeamType.Home))
                {
                    tournament.WinningTeamId = match.Match.TeamHomeId.Value;
                }
                else
                {
                    tournament.WinningTeamId = match.Match.TeamAwayId.Value;
                }

                tournament.EndDate = DateTime.UtcNow;
                _coachBotContext.SaveChanges();
            }

        }

        private void GenerateKnockoutTournament(int tournamentId)
        {
            var tournament = _coachBotContext.Tournaments.Include(t => t.TournamentSeries).First(t => t.Id == tournamentId);
            var tournamentStage = new TournamentStage()
            {
                TournamentId = tournament.Id,
                Name = "Edition"
            };
            _coachBotContext.TournamentStages.Add(tournamentStage);
            _coachBotContext.SaveChanges();
        }

        private void GenerateKnockoutSchedule(int tournamentId)
        {
            var tournament = _coachBotContext.Tournaments
                .Include(t => t.TournamentSeries)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupTeams)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupMatches)
                    .ThenInclude(t => t.Match)
                .Include(t => t.TournamentMatchDays)
                .First(t => t.Id == tournamentId);

            if (!tournament.TournamentStages.Any(s => s.TournamentGroups.Any()))
            {
                throw new Exception("There are no groups for this tournament");
            }

            if (!tournament.TournamentStages.Any(s => s.TournamentGroups.Any(g => g.TournamentGroupTeams.Any())))
            {
                throw new Exception("There are no teams assigned to any groups");
            }

            if (tournament.TournamentMatchDays == null || !tournament.TournamentMatchDays.Any())
            {
                throw new Exception("There are no match days set for this tournament");
            }

            RemoveMatchesForTournament(tournamentId);

            var stage = tournament.TournamentStages.First();
            var earliestMatchDate = (DateTime)tournament.StartDate;
            var teams = _coachBotContext.TournamentGroupTeams
              .Where(t => t.TournamentGroup.TournamentStageId == stage.Id)
              .Select(t => t.Team)
              .Distinct()
              .ToList();
            GenerateKnockoutMatches(tournament, stage, teams, earliestMatchDate, true);
        }

        #endregion Knockout

        #region Round Robin

        private void GenerateRoundRobinTournament(int tournamentId)
        {
            var tournament = _coachBotContext.Tournaments.Include(t => t.TournamentSeries).First(t => t.Id == tournamentId);
            var tournamentStage = new TournamentStage()
            {
                TournamentId = tournament.Id,
                Name = "Season"
            };
            _coachBotContext.TournamentStages.Add(tournamentStage);
            _coachBotContext.SaveChanges();
        }

        private void GenerateRoundRobinSchedule(int tournamentId)
        {
            var tournament = _coachBotContext.Tournaments
                .Include(t => t.TournamentSeries)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupTeams)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupMatches)
                    .ThenInclude(t => t.Match)
                .Include(t => t.TournamentMatchDays)
                .First(t => t.Id == tournamentId);

            if (!tournament.TournamentStages.Any(s => s.TournamentGroups.Any()))
            {
                throw new Exception("There are no groups for this tournament");
            }

            if (!tournament.TournamentStages.Any(s => s.TournamentGroups.Any(g => g.TournamentGroupTeams.Any())))
            {
                throw new Exception("There are no teams assigned to any groups");
            }

            if (tournament.TournamentMatchDays == null || !tournament.TournamentMatchDays.Any())
            {
                throw new Exception("There are no match days set for this tournament");
            }

            var numberOfMatchDays = tournament.TournamentMatchDays.Count();
            DateTime earliestMatchDate = (DateTime)tournament.StartDate;
            foreach (var stage in tournament.TournamentStages)
            {
                GenerateRoundRobinMatches(tournament, stage, earliestMatchDate);
            }
            
        }

        #endregion Round Robin

        #region Tournament Helpers
        public void GenerateRoundRobinMatches(Tournament tournament, TournamentStage stage, DateTime startDate)
        {
            var maxNumberOfTeams = _coachBotContext.TournamentGroupTeams
                    .Where(t => t.TournamentGroup.TournamentStageId == stage.Id)
                    .GroupBy(t => t.TournamentGroupId)
                    .Max(t => t.Count());
            var generatedPhases = GenerateTournamentPhases(maxNumberOfTeams, stage.Id);
            foreach(var generatedPhase in generatedPhases)
            {
                _coachBotContext.TournamentPhases.Add(generatedPhase);
                _coachBotContext.SaveChanges();
            }
            foreach (var group in stage.TournamentGroups)
            {
                var earliestMatchDate = startDate;
                var matchDaySlots = _coachBotContext.TournamentMatchDays.Where(t => t.TournamentId == tournament.Id).OrderBy(t => t.MatchDay).OrderBy(t => t.MatchTime).ToList();
                var currentSlotIndex = 0;
                var currentPhaseNumber = 1;
                foreach (var phase in stage.TournamentPhases)
                {
                    if (currentPhaseNumber > 1)
                    {
                        int daysUntilNextMonday = (((int)DayOfWeek.Monday - (int)earliestMatchDate.DayOfWeek + 7) % 7) + 1;
                        earliestMatchDate = earliestMatchDate.AddDays(daysUntilNextMonday);
                    }
                    foreach (var groupTeam in group.TournamentGroupTeams)
                    {
                        DateTime scheduledKickOff;
                        if (currentSlotIndex == matchDaySlots.Count() - 1)
                        {
                            scheduledKickOff = GetMatchDaySlotDate(earliestMatchDate, matchDaySlots.Last()); // Default to re-using the last slot, rather than jumping to next week
                        }
                        else
                        {
                            scheduledKickOff = GetMatchDaySlotDate(earliestMatchDate, matchDaySlots.ElementAt(currentSlotIndex));
                            currentSlotIndex++;
                        }
                        earliestMatchDate = scheduledKickOff;

                        var awayTeam = group.TournamentGroupTeams.Where(t =>
                            t.TeamId != groupTeam.TeamId
                            // Team doesn't already have a match in this phase
                            && !_coachBotContext.TournamentGroupMatches.Any(m =>
                                (t.TeamId == m.Match.TeamAwayId || t.TeamId == m.Match.TeamHomeId) && m.TournamentPhaseId == phase.Id
                            )
                            // Match-up hasn't already happened in a previous phase
                            && !group.TournamentGroupMatches.Any(m => (m.Match.TeamHomeId == t.TeamId && m.Match.TeamAwayId == groupTeam.TeamId) || (m.Match.TeamAwayId == t.TeamId && m.Match.TeamHomeId == groupTeam.TeamId)
                            )
                        ).FirstOrDefault();
                        if (awayTeam != null && !_coachBotContext.TournamentGroupMatches.Any(t => t.TournamentPhaseId == phase.Id && (t.Match.TeamHomeId == groupTeam.TeamId || t.Match.TeamAwayId == groupTeam.TeamId)))
                        {
                            var match = new TournamentGroupMatch()
                            {
                                TournamentGroupId = group.Id,
                                TournamentPhaseId = phase.Id,
                                Match = new Match()
                                {
                                    TeamHomeId = groupTeam.TeamId,
                                    TeamAwayId = awayTeam.TeamId,
                                    MatchType = MatchType.Competition,
                                    KickOff = scheduledKickOff,
                                    Format = tournament.Format,
                                    TournamentId = tournament.Id
                                }
                            };
                            _coachBotContext.TournamentGroupMatches.Add(match);
                            _coachBotContext.SaveChanges();
                        }
                    }
                    currentPhaseNumber++;
                }
            }
            _coachBotContext.SaveChanges();
            
        }

        public void GenerateKnockoutMatches(Tournament tournament, TournamentStage stage, List<Team> teams, DateTime earliestMatchDate, bool teamsKnown)
        {
            var brackets = BracketsHelper.GenerateBrackets(teams.Count);
            var groupId = stage.TournamentGroups.First().Id;
            var matches = new List<TournamentGroupMatch>();
            var rounds = brackets.Max(b => b.RoundNo);
            foreach (var round in brackets.Select(b => b.RoundNo).Distinct())
            {
                var roundsFromFinal = rounds - round;
                var roundName = "Round " + round;
                switch (roundsFromFinal)
                {
                    case 0:
                        roundName = "Final";
                        break;

                    case 1:
                        roundName = "Semi Final";
                        break;

                    case 2:
                        roundName = "Quarter Final";
                        break;

                    default:
                        break;
                }

                var phase = new TournamentPhase()
                {
                    Name = roundName,
                    TournamentStageId = stage.Id
                };
                _coachBotContext.TournamentPhases.Add(phase);

                if (round > 1)
                {
                    int daysUntilNextMonday = (((int)DayOfWeek.Monday - (int)earliestMatchDate.DayOfWeek + 7) % 7) + 1;
                    earliestMatchDate = earliestMatchDate.AddDays(daysUntilNextMonday);
                }

                var lastRoundByes = brackets.Where(b => brackets.Any(x => x.Bye == true && b.RoundNo == round - 1) && b.RoundNo == round - 1 && b.Bye == false).Count();
                var currentMatchNumberOfRound = 1;
                var matchesInRound = brackets.Where(b => b.RoundNo == round && b.Bye == false).Count();
                var matchesInLastRound = brackets.Where(b => b.RoundNo == round - 1 && b.Bye == false).Count();
                var matchDaySlots = _coachBotContext.TournamentMatchDays.Where(t => t.TournamentId == tournament.Id).OrderBy(t => t.MatchDay).OrderBy(t => t.MatchTime).ToList();
                var currentSlotIndex = 0;
                foreach (var matchup in brackets.Where(b => b.RoundNo == round && b.Bye == false))
                {
                    DateTime scheduledKickOff;
                    if (currentSlotIndex == matchDaySlots.Count() - 1)
                    {
                        scheduledKickOff = GetMatchDaySlotDate(earliestMatchDate, matchDaySlots.Last()); // Default to re-using the last slot, rather than jumping to next week
                    }
                    else
                    {
                        scheduledKickOff = GetMatchDaySlotDate(earliestMatchDate, matchDaySlots.ElementAt(currentSlotIndex));
                        currentSlotIndex++;
                    }

                    earliestMatchDate = scheduledKickOff;

                    int? homeTeamId = null;
                    if (teamsKnown)
                    {
                        teams.Select(t => t.Id).LastOrDefault(t => !matches.Any(m => t == m.Match.TeamHomeId || t == m.Match.TeamAwayId));
                    }
                    int? awayTeamId = null;
                    if (lastRoundByes == 0)
                    {
                        awayTeamId = teams.Select(t => t.Id).LastOrDefault(t => !matches.Any(m => t == m.Match.TeamHomeId || t == m.Match.TeamAwayId) && t != homeTeamId);
                    }
                    else
                    {
                        lastRoundByes--;
                    }

                    string homePlaceholder = "TBC";
                    string awayPlaceholder = "TBC";
                    switch (roundsFromFinal)
                    {
                        case 0:
                            homePlaceholder = "Winner of SF1";
                            awayPlaceholder = "Winner of SF2";
                            break;

                        case 1:
                            homePlaceholder = currentMatchNumberOfRound == 1 ? "Winner of QF1" : "Winner of QF3";
                            awayPlaceholder = currentMatchNumberOfRound == 1 ? "Winner of QF2" : "Winner of QF4";
                            break;

                        default:
                            break;
                    }

                    var match = new TournamentGroupMatch()
                    {
                        TournamentGroupId = groupId,
                        TournamentPhaseId = phase.Id,
                        TeamHomePlaceholder = homePlaceholder,
                        TeamAwayPlaceholder = awayPlaceholder,
                        Match = new Match()
                        {
                            TeamHomeId = homeTeamId > 0 ? homeTeamId : (int?)null,
                            TeamAwayId = awayTeamId > 0 ? awayTeamId : (int?)null,
                            MatchType = MatchType.Competition,
                            KickOff = scheduledKickOff,
                            Format = tournament.Format,
                            TournamentId = tournament.Id
                        }
                    };
                    _coachBotContext.TournamentGroupMatches.Add(match);
                    matches.Add(match);
                    currentMatchNumberOfRound++;
                }
            }
            _coachBotContext.SaveChanges();
        }

        private List<TournamentPhase> GenerateTournamentPhases(int numberOfteams, int tournamentStageId)
        {
            var phases = new List<TournamentPhase>();
            // INFO: Numbers of phases is number of teams minus 1, so this for logic is intentional
            for (int i = 1; i <= numberOfteams; i++)
            {
                var phase = new TournamentPhase()
                {
                    Name = "Round " + i.ToString(),
                    TournamentStageId = tournamentStageId
                };
                phases.Add(phase);
            }

            return phases;
        }
        #endregion
    }
}