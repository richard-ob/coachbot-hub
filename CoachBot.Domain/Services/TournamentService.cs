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
                .Where(t => !excludeInactive || (t.EndDate == null || t.EndDate > DateTime.Now))
                .ToList();
        }

        public Tournament GetTournament(int tournamentId)
        {
            return _coachBotContext.Tournaments
                .Include(t => t.TournamentSeries)
                    .ThenInclude(t => t.TournamentLogo)
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
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupMatches)
                    .ThenInclude(t => t.Match)
                    .ThenInclude(t => t.TeamAway)
                .Single(t => t.Id == tournamentId);
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
                default:
                    break;
            }
        }

        public void UpdateTournament(Tournament tournament)
        {
            var existingTournament = _coachBotContext.Tournaments.Single(m => m.Id == tournament.Id);
            existingTournament.IsPublic = tournament.IsPublic;
            existingTournament.StartDate = tournament.StartDate;

            var tournamentSeries = _coachBotContext.TournamentSeries.Single(ts => ts.Tournaments.Any(t => ts.Id == tournament.Id));
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

        public TournamentPhase GetCurrentTournamentPhase(int tournamentId)
        {
            return _coachBotContext.TournamentGroupMatches
                .Where(tg => tg.TournamentGroup.TournamentStage.TournamentId == tournamentId)
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

        public List<TournamentStaff> GetTournamentStaff(int tournamentId)
        {
            return _coachBotContext.TournamentStaff
                .Include(t => t.Player)
                .Include(t => t.Tournament)
                .Where(g => g.TournamentId == tournamentId)
                .ToList();
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
                default:
                    break;
            }
        }

        private DateTime GetMatchDaySlotDate(DateTime earliestDate, TournamentMatchDaySlot tournamentMatchDaySlot)
        {
            int daysUntilMatchDay = ((int)tournamentMatchDaySlot.MatchDay - (int)earliestDate.DayOfWeek + 8) % 7;
            var matchDaySlotDate = earliestDate.AddDays(daysUntilMatchDay);

            return matchDaySlotDate.Date + tournamentMatchDaySlot.MatchTime.TimeOfDay;
        }

        private void RemoveMatchesForTournament(int tournamentId)
        {
            var matches = _coachBotContext.TournamentGroupMatches.Where(t => t.TournamentGroup.TournamentStage.TournamentId == tournamentId);
            if (matches != null & matches.Any())
            {
                _coachBotContext.TournamentGroupMatches.RemoveRange(matches);
            }
        }

        #region Knockout
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
            var teams = _coachBotContext.TournamentGroupTeams
                .Where(t => t.TournamentGroup.TournamentStageId == stage.Id)
                .Select(t => t.Team)
                .Distinct()
                .ToList();

            var brackets = BracketsHelper.GenerateBrackets(teams);
            var groupId = stage.TournamentGroups.First().Id;
            var matches = new List<TournamentGroupMatch>();
            var rounds = brackets.Max(b => b.RoundNo);
            DateTime earliestMatchDate = (DateTime)tournament.StartDate;
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
                    TournamentStageId = tournament.TournamentStages.Select(s => s.Id).First()
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
                var matchDaySlots = _coachBotContext.TournamentMatchDays.Where(t => t.TournamentId == tournamentId).OrderBy(t => t.MatchDay).OrderBy(t => t.MatchTime).ToList();
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
                            TeamAwayId = awayTeamId > 0 ? awayTeamId : (int?)null,
                            MatchType = MatchType.Competition,
                            ScheduledKickOff = scheduledKickOff,
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
        #endregion

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
                var maxNumberOfTeams = _coachBotContext.TournamentGroupTeams
                    .Where(t => t.TournamentGroup.TournamentStageId == stage.Id)
                    .GroupBy(t => t.TournamentGroupId)
                    .Max(t => t.Count());

                _coachBotContext.TournamentPhases.AddRange(GenerateTournamentPhases(maxNumberOfTeams, stage.Id));
                foreach (var group in stage.TournamentGroups)
                {
                    var matchDaySlots = _coachBotContext.TournamentMatchDays.Where(t => t.TournamentId == tournamentId).OrderBy(t => t.MatchDay).OrderBy(t => t.MatchTime).ToList();
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