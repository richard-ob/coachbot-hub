using CoachBot.Database;
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
                .Where(t => !excludeInactive || t.IsActive)
                .ToList();
        }

        public List<TournamentEdition> GetTournamentEditions(bool excludeInactive = false)
        {
            return _coachBotContext.TournamentEditions
                .Where(t => !excludeInactive || (t.EndDate == null || t.EndDate > DateTime.Now))
                .ToList();
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

        public void CreateTournamentEdition(TournamentEdition tournamentEdition)
        {
            tournamentEdition.IsPublic = false;
            _coachBotContext.TournamentEditions.Add(tournamentEdition);
            _coachBotContext.SaveChanges();

            var tournamentType = _coachBotContext.TournamentEditions.Find(tournamentEdition.Id).Tournament.TournamentType;
            switch (tournamentType)
            {
                case TournamentType.RoundRobinLadder:
                    GenerateRoundRobinLadderTournament(tournamentEdition.Id);
                    break;
                default:
                    break;
            }
        }

        public void UpdateTournamentEdition(TournamentEdition tournamentEdition)
        {
            _coachBotContext.TournamentEditions.Update(tournamentEdition);
            _coachBotContext.SaveChanges();
        }

        public void CreateTournamentStage(TournamentStage tournamentStage)
        {
            _coachBotContext.TournamentStages.Add(tournamentStage);
            _coachBotContext.SaveChanges();
        }

        public void UpdateTournamentStage(TournamentStage tournamentStage)
        {
            _coachBotContext.TournamentStages.Update(tournamentStage);
            _coachBotContext.SaveChanges();
        }

        public void DeleteTournamentStage(int id)
        {
            var tournamentStage = _coachBotContext.TournamentStages.Find(id);
            _coachBotContext.TournamentStages.Update(tournamentStage);
            _coachBotContext.SaveChanges();
        }

        public void CreateTournamentPhase(TournamentPhase tournamentPhase)
        {
            _coachBotContext.TournamentPhases.Add(tournamentPhase);
            _coachBotContext.SaveChanges();
        }

        public void UpdateTournamentPhase(TournamentPhase tournamentPhase)
        {
            _coachBotContext.TournamentPhases.Update(tournamentPhase);
            _coachBotContext.SaveChanges();
        }

        public void DeleteTournamentPhase(int id)
        {
            var tournamentPhase = _coachBotContext.TournamentPhases.Find(id);
            _coachBotContext.TournamentPhases.Update(tournamentPhase);
            _coachBotContext.SaveChanges();
        }

        public void CreateTournamentGroup(TournamentGroup tournamentGroup)
        {
            _coachBotContext.TournamentGroups.Add(tournamentGroup);
            _coachBotContext.SaveChanges();
        }

        public void UpdateTournamentGroup(TournamentPhase tournamentPhase)
        {
            _coachBotContext.TournamentPhases.Update(tournamentPhase);
            _coachBotContext.SaveChanges();
        }

        public void DeleteTournamentGroup(int id)
        {
            var tournamentGroup = _coachBotContext.TournamentGroups.Find(id);
            _coachBotContext.TournamentGroups.Update(tournamentGroup);
            _coachBotContext.SaveChanges();
        }

        public void AddTournamentTeam(int teamId, int tournamentGroupId)
        {
            var tournamentGroupTeam = new TournamentGroupTeam()
            {
                TeamId = teamId,
                TournamentGroupId = tournamentGroupId
            };
            _coachBotContext.TournamentGroupTeams.Add(tournamentGroupTeam);
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

        public void GenerateTournamentSchedule(int tournamentEditionId, int? tournamentStageId)
        {
            var tournament = _coachBotContext.TournamentEditions.Where(t => t.Id == tournamentEditionId).Select(t => t.Tournament).First();
            switch (tournament.TournamentType)
            {
                case TournamentType.RoundRobinLadder:
                    break;
                default:
                    break;
            }
        }

        private void GenerateRoundRobinLadderTournament(int tournamentEditionId)
        {
            var tournament = _coachBotContext.TournamentEditions.Include(t => t.Tournament).First(t => t.Id == tournamentEditionId);
            var tournamentStage = new TournamentStage()
            {
                TournamentEditionId = tournament.Id,
                Name = "Round",
                TournamentGroups = new List<TournamentGroup>()
                {
                    new TournamentGroup()
                    {
                        Name = "S Tier"
                    }
                }
            };
            _coachBotContext.TournamentStages.Add(tournamentStage);
        }

        private void GenerateRoundRobinLadderSchedule(int tournamentEditionId)
        {
            var tournamentEdition = GetTournamentEdition(tournamentEditionId);

            if (tournamentEdition.TournamentEditionMatchDays == null || !tournamentEdition.TournamentEditionMatchDays.Any())
            {
                throw new Exception("There are no match days set for this tournament");
            }

            var numberOfMatchDays = tournamentEdition.TournamentEditionMatchDays.Count();
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
                        foreach (var team in group.TournamentGroupTeams)
                        {
                            currentMatchDay = currentMatchDay.AddDays(1);
                            var awayTeam = group.TournamentGroupTeams.Where(t =>
                                // Team doesn't already have a match in this phase
                                !_coachBotContext.TournamentGroupMatches.Any(m => 
                                    (t.TeamId == m.Match.TeamAwayId || t.TeamId == m.Match.TeamHomeId) && m.TournamentPhaseId == phase.Id && t.TeamId != team.Id
                                )
                                // Match-up hasn't already happened in a previous phase
                                && !group.TournamentGroupMatches.Any(m => (m.Match.TeamHomeId == t.TeamId && m.Match.TeamAwayId == team.Id) || (m.Match.TeamAwayId == t.TeamId && m.Match.TeamHomeId == team.Id)
                                )
                            ).FirstOrDefault();
                            if (awayTeam != null && !_coachBotContext.TournamentGroupMatches.Any(t => t.Match.TeamHomeId == team.Id || t.Match.TeamAwayId == team.Id))
                            {
                                var match = new TournamentGroupMatch()
                                {
                                    TournamentGroupId = group.Id,
                                    TournamentPhaseId = phase.Id,
                                    Match = new Match()
                                    {
                                        TeamHomeId = team.Id,
                                        
                                        ReadiedDate = currentMatchDay,
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
            }

            List<TournamentPhase> GenerateTournamentPhases(int numberOfteams, int tournamentStageId)
            {
                var phases = new List<TournamentPhase>();
                for (int i = 0; i < numberOfteams; i++)
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

        private TournamentEdition GetTournamentEdition(int tournamentEdition)
        {
            return _coachBotContext.TournamentEditions
                .Include(t => t.Tournament)
                .Include(t => t.TournamentStages)
                .Include(t => t.TournamentEditionMatchDays)
                .First();
        }
    }
}