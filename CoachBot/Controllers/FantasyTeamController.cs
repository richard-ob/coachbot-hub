using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Controllers
{
    public class FantasyTeamController
    {
        private readonly FantasyService _fantasyService;

        public FantasyTeamController(FantasyService fantasyService)
        {
            _fantasyService = fantasyService;
        }

        [HttpGet("tournament/{tournamentEditionId}")]
        public IEnumerable<FantasyTeam> GetFantasyTeams(int tournamentEditionId)
        {
            return _fantasyService.GetFantasyTeams(tournamentEditionId);
        }

        [HttpGet("{id}")]
        public FantasyTeam GetFantasyTeam(int id)
        {
            return _fantasyService.GetFantasyTeam(id);
        }

        [HttpPost]
        public void CreateFantasyTeam(FantasyTeam fantasyTeam)
        {
            _fantasyService.CreateFantasyTeam(fantasyTeam);
        }

        [HttpPut]
        public void UpdateFantasyTeam(FantasyTeam fantasyTeam)
        {
            _fantasyService.UpdateFantasyTeam(fantasyTeam);
        }

        [HttpPost("{id}/selections")]
        public void AddFantasyTeamSelection(FantasyTeam fantasyTeam)
        {
            _fantasyService.UpdateFantasyTeam(fantasyTeam);
        }

        [HttpDelete("{id}/selections/{fantasyTeamSelectionId}")]
        public void RemoveFantasyTeamSelection(int fantasyTeamSelectionId)
        {
            _fantasyService.RemoveFantasyTeamSelection(fantasyTeamSelectionId);
        }

    }
}
