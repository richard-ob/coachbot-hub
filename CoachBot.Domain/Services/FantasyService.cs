using CoachBot.Database;
using CoachBot.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoachBot.Domain.Services
{
    public class FantasyService
    {
        private readonly CoachBotContext _coachBotContext;

        public FantasyService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public IEnumerable<FantasyTeam> GetFantasyTeams(int tournamentEditionId)
        {
            return _coachBotContext.FantasyTeams.Where(ft => ft.TournamentEditionId == tournamentEditionId).ToList();
        }

        public FantasyTeam GetFantasyTeam(int fantasyTeamId)
        {
            return _coachBotContext.FantasyTeams
                .Include(ft => ft.FantasyTeamSelections)
                .First(ft => ft.Id == fantasyTeamId);
        }

        public void CreateFantasyTeam(FantasyTeam fantasyTeam)
        {
            _coachBotContext.FantasyTeams.Add(fantasyTeam);
            _coachBotContext.SaveChanges();
        }

        public void UpdateFantasyTeam(FantasyTeam fantasyTeam)
        {
            var existingFantasyTeam = _coachBotContext.FantasyTeams.Find(fantasyTeam.Id);
            //existingFantasyTeam.? = fantasyTeam.?
            _coachBotContext.FantasyTeams.Update(existingFantasyTeam);
            _coachBotContext.SaveChanges();
        }

        public void DeleteFantasyTeam(int fantasyTeamId)
        {
            var fantasyTeam = _coachBotContext.FantasyTeams.Find(fantasyTeamId);
            _coachBotContext.FantasyTeams.Remove(fantasyTeam);
            _coachBotContext.SaveChanges();
        }

        public void AddFantasyTeamSelection(FantasyTeamSelection fantasyTeamSelection)
        {
            _coachBotContext.FantasyTeamSelections.Add(fantasyTeamSelection);
            _coachBotContext.SaveChanges();
        }

        public void RemoveFantasyTeamSelection(int fantasyTeamSelectionId)
        {
            var selection = _coachBotContext.FantasyTeamSelections.Find(fantasyTeamSelectionId);
            _coachBotContext.FantasyTeamSelections.Remove(selection);
            _coachBotContext.SaveChanges();
        }

    }
}
