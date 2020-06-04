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

        public List<Tournament> GetTournaments(bool excludeInactive = false)
        {
            return _coachBotContext.Tournaments
                .Include(t => t.TournamentEditions)
                .Where(t => !excludeInactive || t.IsActive)
                .ToList();
        }

        public Tournament GetTournament(int tournamentId)
        {
            return _coachBotContext.Tournaments
                .Include(t => t.TournamentEditions)
                .Single(t => t.Id == tournamentId);
        }

        public List<TournamentEdition> GetTournamentEditions(bool excludeInactive = false)
        {
            return _coachBotContext.TournamentEditions
                .Include(t => t.Tournament)
                .ThenInclude(t => t.Organisation)
                .Where(t => !excludeInactive || (t.EndDate == null || t.EndDate > DateTime.Now))
                .ToList();
        }

        public TournamentEdition GetTournamentEdition(int tournamentEditionId)
        {
            return _coachBotContext.TournamentEditions
                .Include(t => t.Tournament)
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

        public void CreateTournament(Tournament tournament)
        {
            _coachBotContext.Tournaments.Add(tournament);
            _coachBotContext.SaveChanges();
        }

        public void CreateTournamentEdition(TournamentEdition tournamentEdition, ulong? steamId = null)
        {
            tournamentEdition.IsPublic = false;
            _coachBotContext.TournamentEditions.Add(tournamentEdition);
            _coachBotContext.SaveChanges();

            if (steamId != null)
            {
                var player = _coachBotContext.Players.Single(p => p.SteamID == steamId);
                var staff = new TournamentEditionStaff()
                {
                    PlayerId = player.Id,
                    TournamentEditionId = tournamentEdition.Id,
                    Role = TournamentStaffRole.Organiser
                };
                _coachBotContext.TournamentEditionStaff.Add(staff);
                _coachBotContext.SaveChanges();
            }

            var tournament = _coachBotContext.TournamentEditions
                .Include(t => t.Tournament)
                .First(t => t.Id == tournamentEdition.Id);

            switch (tournament.Tournament.TournamentType)
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

        public void UpdateTournamentEdition(TournamentEdition tournamentEdition)
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
                .Where(t => t.TournamentGroup.TournamentStage.TournamentEditionId == tournamentEditionId)
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
                .Where(tg => tg.TournamentGroup.TournamentStage.TournamentEditionId == tournamentEditionId)
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

        public List<TournamentEditionStaff> GetTournamentEditionStaff(int tournamentEditionId)
        {
            return _coachBotContext.TournamentEditionStaff
                .Include(t => t.Player)
                .Include(t => t.TournamentEdition)
                .Where(g => g.TournamentEditionId == tournamentEditionId)
                .ToList();
        }

        public void CreateTournamentEditionStaff(TournamentEditionStaff tournamentEditionStaff)
        {
            _coachBotContext.TournamentEditionStaff.Add(tournamentEditionStaff);
            _coachBotContext.SaveChanges();
        }

        public void UpdateTournamentEditionStaff(TournamentEditionStaff tournamentEditionStaff)
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
                .Where(g => g.TournamentStage.TournamentEditionId == tournamentEditionId)
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
            var tournamentEditionId = _coachBotContext.TournamentPhases.Where(t => t.Id == tournamentPhaseId).Select(t => t.TournamentStage.TournamentEditionId).First();
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

        public void GenerateTournamentSchedule(int tournamentEditionId, int? tournamentStageId = null)
        {
            var tournament = _coachBotContext.TournamentEditions.Where(t => t.Id == tournamentEditionId).Select(t => t.Tournament).First();
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

        #region Round Robin
        private void GenerateKnockoutTournament(int tournamentEditionId)
        {
            var tournament = _coachBotContext.TournamentEditions.Include(t => t.Tournament).First(t => t.Id == tournamentEditionId);
            var tournamentStage = new TournamentStage()
            {
                TournamentEditionId = tournament.Id,
                Name = "Edition"
            };
            _coachBotContext.TournamentStages.Add(tournamentStage);
            _coachBotContext.SaveChanges();
        }

        private void GenerateKnockoutSchedule(int tournamentEditionId)
        {
            var tournamentEdition = _coachBotContext.TournamentEditions
                .Include(t => t.Tournament)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupTeams)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupMatches)
                    .ThenInclude(t => t.Match)
                .Include(t => t.TournamentEditionMatchDays)
                .First(t => t.Id == tournamentEditionId);

            if (!tournamentEdition.TournamentStages.Any(s => s.TournamentGroups.Any()))
            {
                throw new Exception("There are no groups for this tournament");
            }

            if (!tournamentEdition.TournamentStages.Any(s => s.TournamentGroups.Any(g => g.TournamentGroupTeams.Any())))
            {
                throw new Exception("There are no teams assigned to any groups");
            }

            var stage = tournamentEdition.TournamentStages.First();
            var numberOfTeams = _coachBotContext.TournamentGroupTeams
                .Where(t => t.TournamentGroup.TournamentStageId == stage.Id)
                .GroupBy(t => t.TournamentGroupId)
                .Max(t => t.Count());

            var teams = _coachBotContext.TournamentGroupTeams
                .Where(t => t.TournamentGroup.TournamentStageId == stage.Id)
                .Select(t => t.Team)
                .Distinct()
                .ToList();

            if (teams.Count != numberOfTeams)
            {
                throw new Exception("ERROR");
            }

            var brackets = BracketsHelper.GenerateBrackets(teams);
            var phases = new List<TournamentPhase>();
            var matches = new List<Match>();
            var currentMatchDay = DateTime.Now;
            foreach (var round in brackets.Select(b => b.RoundNo).Distinct())
            {
                var phase = new TournamentPhase()
                {
                    Name = "Round " + round
                };
                phases.Add(phase);
                currentMatchDay = currentMatchDay.AddDays(1);
                foreach (var matchup in brackets.Where(b => b.RoundNo == round && b.Bye == false))
                {
                    var match = new Match()
                    {
                        TeamHome = new Team()
                        {
                            Name = matchup.TeamNames != null ? matchup.TeamNames.Item1 : null
                        },
                        TeamAway = new Team()
                        {
                            Name = matchup.TeamNames != null ? matchup.TeamNames.Item2 : null
                        },
                        MatchType = MatchType.Competition,
                        ScheduledKickOff = currentMatchDay,
                        Format = tournamentEdition.Format,
                        TournamentId = tournamentEdition.Id
                    };
                    matches.Add(match);
                }
            }
        }
        #endregion

        #region Round Robin
        private void GenerateRoundRobinTournament(int tournamentEditionId)
        {
            var tournament = _coachBotContext.TournamentEditions.Include(t => t.Tournament).First(t => t.Id == tournamentEditionId);
            var tournamentStage = new TournamentStage()
            {
                TournamentEditionId = tournament.Id,
                Name = "Season"
            };
            _coachBotContext.TournamentStages.Add(tournamentStage);
            _coachBotContext.SaveChanges();
        }

        private void GenerateRoundRobinSchedule(int tournamentEditionId)
        {
            var tournamentEdition = _coachBotContext.TournamentEditions
                .Include(t => t.Tournament)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupTeams)
                .Include(t => t.TournamentStages)
                    .ThenInclude(t => t.TournamentGroups)
                    .ThenInclude(t => t.TournamentGroupMatches)
                    .ThenInclude(t => t.Match)
                .Include(t => t.TournamentEditionMatchDays)
                .First(t => t.Id == tournamentEditionId);

            if (!tournamentEdition.TournamentStages.Any(s => s.TournamentGroups.Any()))
            {
                throw new Exception("There are no groups for this tournament");
            }

            if (!tournamentEdition.TournamentStages.Any(s => s.TournamentGroups.Any(g => g.TournamentGroupTeams.Any())))
            {
                throw new Exception("There are no teams assigned to any groups");
            }

            /*if (tournamentEdition.TournamentEditionMatchDays == null || !tournamentEdition.TournamentEditionMatchDays.Any())
            {
                throw new Exception("There are no match days set for this tournament");
            }

            var numberOfMatchDays = tournamentEdition.TournamentEditionMatchDays.Count();*/
            foreach (var stage in tournamentEdition.TournamentStages)
            {
                var maxNumberOfTeams = _coachBotContext.TournamentGroupTeams
                    .Where(t => t.TournamentGroup.TournamentStageId == stage.Id)
                    .GroupBy(t => t.TournamentGroupId)
                    .Max(t => t.Count());

                _coachBotContext.TournamentPhases.AddRange(GenerateTournamentPhases(maxNumberOfTeams, stage.Id));
                foreach (var group in stage.TournamentGroups)
                {
                    foreach (var phase in stage.TournamentPhases)
                    {
                        var currentMatchDay = (DateTime)tournamentEdition.StartDate;
                        foreach (var groupTeam in group.TournamentGroupTeams)
                        {
                            currentMatchDay = currentMatchDay.AddDays(1);
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
                                        ScheduledKickOff = currentMatchDay,
                                        Format = tournamentEdition.Format,
                                        TournamentId = tournamentEdition.Id
                                    }
                                };
                                _coachBotContext.TournamentGroupMatches.Add(match);
                                _coachBotContext.SaveChanges();
                            }
                        }
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