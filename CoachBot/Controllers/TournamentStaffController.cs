using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/tournament-staff")]
    [ApiController]
    public class TournamentStaffController : Controller
    {
        private readonly TournamentService _tournamentService;

        public TournamentStaffController(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpDelete("{id}")]
        public void DeleteTournamentStaffMember(int id)
        {
            _tournamentService.DeleteTournamentStaff(id);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost]
        public void CreateTournamentStaffMember(TournamentStaff tournamentStaff)
        {
            // TODO: MUST BE ADMIN OF TOURNAMENT
            _tournamentService.CreateTournamentStaff(tournamentStaff);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPut("{id}")]
        public void UpdateTournamentStaffMember(TournamentStaff tournamentStaff)
        {
            // TODO: MUST BE ADMIN OF TOURNAMENT
            _tournamentService.UpdateTournamentStaff(tournamentStaff);
        }

    }
}
