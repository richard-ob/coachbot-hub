using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/organisations")]
    [ApiController]
    public class OrganisationController : Controller
    {
        private readonly TournamentService _tournamentService;

        public OrganisationController(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HttpGet("{id}")]
        public Organisation Get(int id)
        {
            return _tournamentService.GetOrganisation(id);
        }

        [HttpGet]
        public IEnumerable<Organisation> GetAll()
        {
            return _tournamentService.GetOrganisations();
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _tournamentService.RemoveOrganisation(id);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPost]
        public void Create(Organisation organisation)
        {
            _tournamentService.CreateOrganisation(organisation);
        }

        [HubRolePermission(HubRole = PlayerHubRole.Administrator)]
        [HttpPut]
        public void Update(Organisation organisation)
        {
            _tournamentService.UpdateOrganisation(organisation);
        }
    }
}