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

        public List<TournamentSeries> GetTournaments(bool excludeInactive = false)
        {
            return _coachBotContext.Tournaments
                .Include(t => t.Tournaments)
                .Where(t => !excludeInactive || t.IsActive)
                .ToList();
        }

        public TournamentSeries GetTournament(int tournamentId)
        {
            return _coachBotContext.Tournaments
                .Include(t => t.Tournaments)
                .Single(t => t.Id == tournamentId);
        }

        public List<Tournament> GetTournamentEditions(bool excludeInactive = false)
        {
            return _coachBotContext.TournamentEditions
                .Include(t => t.TournamentSeries)
                .ThenInclude(t => t.Organisation)
                .Where(t => !excludeInactive || (t.EndDate == null || t.EndDate > DateTime.Now))
                .ToList();
        }

        public Tournament GetTournamentEdition(int tournamentEditionId)
        {
            return _coachBotContext.TournamentEditions
                .Include(t => t.TournamentSeries)
                    .ThenInclude(t => t.TournamentLogo)
                .Include(t => t.TournamentEditionStaff)
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
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupMatches)
                    .ThenInclude(t => t.Match)
                    .ThenInclude(t => t.TeamAway)
                .Single(t => t.Id == tournamentEditionId);
        }

        public List<Organisation> GetOrganisations()
        {
            return _coachBotContext.Organisations.ToList();
        }

        public void CreateOrganisation(Organisation organisation)
        {
            _coachBotContext.Organisations.Add(organisation);
            _coachBotContext.SaveChanges();
        }

        public void CreateTournament(TournamentSeries tournament)
        {
            _coachBotContext.Tournaments.Add(tournament);
            _coachBotContext.SaveChanges();
        }

        public void CreateTournamentEdition(Tournament tournamentEdition, ulong? steamId = null)
        {
            tournamentEdition.IsPublic = false;
            _coachBotContext.TournamentEditions.Add(tournamentEdition);
            _coachBotContext.SaveChanges();

            if (steamId != null)
            {
                var player = _coachBotContext.Players.Single(p => p.SteamID == steamId);
                var staff = new TournamentStaff()
                {
                    PlayerId = player.Id,
                    TournamentId = tournamentEdition.Id,
                    Role = TournamentStaffRole.Organiser
                };
                _coachBotContext.TournamentEditionStaff.Add(staff);
                _coachBotContext.SaveChanges();
            }

            var tournament = _coachBotContext.TournamentEditions
                .Include(t => t.TournamentSeries)
                .First(t => t.Id == tournamentEdition.Id);

            switch (tournament.TournamentType)
            {
                case TournamentType.RoundRobin:
                    GenerateRoundRobinTournament(tournamentEdition.Id);
                    break;
                case TournamentType.Knockout:
                    GenerateKnockoutTournament(tournamentEdition.Id);
                    break;
                default:
                    break;
            }
        }

        public void UpdateTournamentEdition(Tournament tournamentEdition)
        {
            var existingTournamentEdition = _coachBotContext.TournamentEditions.Single(m => m.Id == tournamentEdition.Id);
            existingTournamentEdition.IsPublic = tournamentEdition.IsPublic;
            existingTournamentEdition.StartDate = tournamentEdition.StartDate;
            _coachBotContext.Update(existingTournamentEdition);

            var existingTournament = _coachBotContext.Tournaments.Single(t => t.Id == existingTournamentEdition.TournamentId);
            if (tournamentEdition.IsPublic)
            {
                existingTournament.IsPublic = true;
            }
            _coachBotContext.Update(existingTournament);

            _coachBotContext.SaveChanges();
        }

        public List<Team> GetTournamentTeams(int tournamentEditionId)
        {
            return _coachBotContext.TournamentGroupTeams
                .Where(t => t.TournamentGroup.TournamentStage.TournamentId == tournamentEditionId)
                .Select(t => t.Team)
                .Distinct()
                .ToList();
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
                .Where(m => !futureOnly || m.ScheduledKickOff > DateTime.Now)
                .ToList();
        }

        public TournamentPhase GetCurrentTournamentPhase(int tournamentEditionId)
        {
            return _coachBotContext.TournamentGroupMatches
                .Where(tg => tg.TournamentGroup.TournamentStage.TournamentId == tournamentEditionId)
                .Where(m => m.Match.ScheduledKickOff > DateTime.Now)
                .OrderBy(m => m.Match.ScheduledKickOff)
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

        public List<TournamentStaff> GetTournamentEditionStaff(int tournamentEditionId)
        {
            return _coachBotContext.TournamentEditionStaff
                .Include(t => t.Player)
                .Include(t => t.Tournament)
                .Where(g => g.TournamentId == tournamentEditionId)
                .ToList();
        }

        public void CreateTournamentEditionStaff(TournamentStaff tournamentEditionStaff)
        {
            _coachBotContext.TournamentEditionStaff.Add(tournamentEditionStaff);
            _coachBotContext.SaveChanges();
        }

        public void UpdateTournamentEditionStaff(TournamentStaff tournamentEditionStaff)
        {
            var existing = _coachBotContext.TournamentEditionStaff.Single(t => t.Id == tournamentEditionStaff.Id);
            existing.Role = tournamentEditionStaff.Role;
            _coachBotContext.TournamentEditionStaff.Update(existing);
            _coachBotContext.SaveChanges();
        }

        public void DeleteTournamentEditionStaff(int id)
        {
            var tournamentEditionStaff = _coachBotContext.TournamentEditionStaff.Find(id);
            _coachBotContext.TournamentEditionStaff.Remove(tournamentEditionStaff);
            _coachBotContext.SaveChanges();
        }

        public List<TournamentGroup> GetTournamentGroups(int tournamentEditionId)
        {
            return _coachBotContext.TournamentGroups
                .Include(t => t.TournamentGroupTeams)
                    .ThenInclude(tgt => tgt.Team)
                .Where(g => g.TournamentStage.TournamentId == tournamentEditionId)
                .ToList();
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
            var tournamentEditionId = _coachBotContext.TournamentPhases.Where(t => t.Id == tournamentPhaseId).Select(t => t.TournamentStage.TournamentId).First();
            match.TournamentId = tournamentEditionId;
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

        public List<TournamentMatchDaySlot> GetTournamentMatchDaySlots(int tournamentEditionId)
        {
            return _coachBotContext.TournamentEditionMatchDays.Where(t => t.TournamentId == tournamentEditionId).ToList();
        }

        public void CreateTournamentMatchDaySlot(TournamentMatchDaySlot tournamentEditionMatchDaySlot)
        {
            tournamentEditionMatchDaySlot.MatchTime = DateTimeHelper.RemoveSeconds(tournamentEditionMatchDaySlot.MatchTime);
            _coachBotContext.TournamentEditionMatchDays.Add(tournamentEditionMatchDaySlot);
            _coachBotContext.SaveChanges();
        }

        public void DeleteTournamentMatchDaySlot(int tournamentEditionMatchDaySlot)
        {
            var matchDaySlot = _coachBotContext.TournamentEditionMatchDays.Find(tournamentEditionMatchDaySlot);
            _coachBotContext.TournamentEditionMatchDays.Remove(matchDaySlot);
            _coachBotContext.SaveChanges();
        }

        public void GenerateTournamentSchedule(int tournamentEditionId, int? tournamentStageId = null)
        {
            var tournament = _coachBotContext.TournamentEditions.First(t => t.Id == tournamentEditionId);
            switch (tournament.TournamentType)
            {
                case TournamentType.RoundRobin:
                    GenerateRoundRobinSchedule(tournamentEditionId);
                    break;
                case TournamentType.Knockout:
                    GenerateKnockoutSchedule(tournamentEditionId);
                    break;
                default:
                    break;
            }
        }

        private DateTime GetMatchDaySlotDate(DateTime earliestDate, TournamentMatchDaySlot tournamentEditionMatchDaySlot)
        {
            int daysUntilMatchDay = ((int)tournamentEditionMatchDaySlot.MatchDay - (int)earliestDate.DayOfWeek + 8) % 7;
            var matchDaySlotDate = earliestDate.AddDays(daysUntilMatchDay);

            return matchDaySlotDate.Date + tournamentEditionMatchDaySlot.MatchTime.TimeOfDay;
        }

        private void RemoveMatchesForTournament(int tournamentEditionId)
        {
            var matches = _coachBotContext.TournamentGroupMatches.Where(t => t.TournamentGroup.TournamentStage.TournamentId == tournamentEditionId);
            if (matches != null & matches.Any())
            {
                _coachBotContext.TournamentGroupMatches.RemoveRange(matches);
            }
        }

        #region Knockout
        private void GenerateKnockoutTournament(int tournamentEditionId)
        {
            var tournament = _coachBotContext.TournamentEditions.Include(t => t.TournamentSeries).First(t => t.Id == tournamentEditionId);
            var tournamentStage = new TournamentStage()
            {
                TournamentId = tournament.Id,
                Name = "Edition"
            };
            _coachBotContext.TournamentStages.Add(tournamentStage);
            _coachBotContext.SaveChanges();
        }

        private void GenerateKnockoutSchedule(int tournamentEditionId)
        {
            var tournamentEdition = _coachBotContext.TournamentEditions
                .Include(t => t.TournamentSeries)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupTeams)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupMatches)
                    .ThenInclude(t => t.Match)
                .Include(t => t.TournamentMatchDays)
                .First(t => t.Id == tournamentEditionId);

            if (!tournamentEdition.TournamentStages.Any(s => s.TournamentGroups.Any()))
            {
                throw new Exception("There are no groups for this tournament");
            }

            if (!tournamentEdition.TournamentStages.Any(s => s.TournamentGroups.Any(g => g.TournamentGroupTeams.Any())))
            {
                throw new Exception("There are no teams assigned to any groups");
            }

            if (tournamentEdition.TournamentMatchDays == null || !tournamentEdition.TournamentMatchDays.Any())
            {
                throw new Exception("There are no match days set for this tournament");
            }

            RemoveMatchesForTournament(tournamentEditionId);

            var stage = tournamentEdition.TournamentStages.First();
            var teams = _coachBotContext.TournamentGroupTeams
                .Where(t => t.TournamentGroup.TournamentStageId == stage.Id)
                .Select(t => t.Team)
                .Distinct()
                .ToList();

            var brackets = BracketsHelper.GenerateBrackets(teams);
            var groupId = stage.TournamentGroups.First().Id;
            var matches = new List<TournamentGroupMatch>();
            var rounds = brackets.Max(b => b.RoundNo);
            DateTime earliestMatchDate = (DateTime)tournamentEdition.StartDate;
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
                    TournamentStageId = tournamentEdition.TournamentStages.Select(s => s.Id).First()
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
                var matchDaySlots = _coachBotContext.TournamentEditionMatchDays.Where(t => t.TournamentId == tournamentEditionId).OrderBy(t => t.MatchDay).OrderBy(t => t.MatchTime).ToList();
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

                    var homeTeamId = teams.Select(t => t.Id).LastOrDefault(t => !matches.Any(m => t == m.Match.TeamHomeId || t == m.Match.TeamAwayId));
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
                            TeamAwayId = awayTeamId,
                            MatchType = MatchType.Competition,
                            ScheduledKickOff = scheduledKickOff,
                            Format = tournamentEdition.Format,
                            TournamentId = tournamentEdition.Id
                        }
                    };
                    _coachBotContext.TournamentGroupMatches.Add(match);
                    matches.Add(match);
                    currentMatchNumberOfRound++;
                }
            }
            _coachBotContext.SaveChanges();
        }
        #endregion

        #region Round Robin
        private void GenerateRoundRobinTournament(int tournamentEditionId)
        {
            var tournament = _coachBotContext.TournamentEditions.Include(t => t.TournamentSeries).First(t => t.Id == tournamentEditionId);
            var tournamentStage = new TournamentStage()
            {
                TournamentId = tournament.Id,
                Name = "Season"
            };
            _coachBotContext.TournamentStages.Add(tournamentStage);
            _coachBotContext.SaveChanges();
        }

        private void GenerateRoundRobinSchedule(int tournamentEditionId)
        {
            var tournamentEdition = _coachBotContext.TournamentEditions
                .Include(t => t.TournamentSeries)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupTeams)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupMatches)
                    .ThenInclude(t => t.Match)
                .Include(t => t.TournamentMatchDays)
                .First(t => t.Id == tournamentEditionId);

            if (!tournamentEdition.TournamentStages.Any(s => s.TournamentGroups.Any()))
            {
                throw new Exception("There are no groups for this tournament");
            }

            if (!tournamentEdition.TournamentStages.Any(s => s.TournamentGroups.Any(g => g.TournamentGroupTeams.Any())))
            {
                throw new Exception("There are no teams assigned to any groups");
            }

            if (tournamentEdition.TournamentMatchDays == null || !tournamentEdition.TournamentMatchDays.Any())
            {
                throw new Exception("There are no match days set for this tournament");
            }

            var numberOfMatchDays = tournamentEdition.TournamentMatchDays.Count();
            DateTime earliestMatchDate = (DateTime)tournamentEdition.StartDate;
            foreach (var stage in tournamentEdition.TournamentStages)
            {
                var maxNumberOfTeams = _coachBotContext.TournamentGroupTeams
                    .Where(t => t.TournamentGroup.TournamentStageId == stage.Id)
                    .GroupBy(t => t.TournamentGroupId)
                    .Max(t => t.Count());

                _coachBotContext.TournamentPhases.AddRange(GenerateTournamentPhases(maxNumberOfTeams, stage.Id));
                foreach (var group in stage.TournamentGroups)
                {
                    var matchDaySlots = _coachBotContext.TournamentEditionMatchDays.Where(t => t.TournamentId == tournamentEditionId).OrderBy(t => t.MatchDay).OrderBy(t => t.MatchTime).ToList();
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
                                        ScheduledKickOff = scheduledKickOff,
                                        Format = tournamentEdition.Format,
                                        TournamentId = tournamentEdition.Id
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

            List<TournamentPhase> GenerateTournamentPhases(int numberOfteams, int tournamentStageId)
            {
                var phases = new List<TournamentPhase>();
                // INFO: Numbers of phases is number of teams minus 1, so this for logic is intentional
                for (int i = 1; i < numberOfteams; i++)
                {
                    var phase = new TournamentPhase()
                    {
                        Name = "Matchweek " + i.ToString(),
                        TournamentStageId = tournamentStageId
                    };
                    phases.Add(phase);
                }

                return phases;
            }
        }
        #endregion
    }
}