using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/tournament-groups")]
    [ApiController]
    public class TournamentGroupController : Controller
    {
        private readonly TournamentService _tournamentService;

        public TournamentGroupController(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpDelete("{id}")]
        public void DeleteTournamentGroup(int id)
        {
            _tournamentService.DeleteTournamentGroup(id);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost]
        public void CreateTournamentGroup(TournamentGroup tournamentGroup)
        {
            _tournamentService.CreateTournamentGroup(tournamentGroup);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPut("{id}")]
        public void UpdateTournamentGroup(TournamentGroup tournamentGroup)
        {
            _tournamentService.UpdateTournamentGroup(tournamentGroup);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost("{id}/teams")]
        public void AddTournamentGroupTeam(TournamentGroupTeamDto tournamentGroupTeamDto)
        {
            _tournamentService.AddTournamentTeam(tournamentGroupTeamDto.TeamId, tournamentGroupTeamDto.TournamentGroupId);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost("{id}/teams/{teamId}")]
        public void RemoveTournamentGroupTeam(int id, int teamId)
        {
            _tournamentService.RemoveTournamentTeam(teamId, id);
        }
    }
}
