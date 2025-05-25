using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Models.OrganizationModel;
using TaskManagerApi.Services.Interfaces;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/organization")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private IOrganizationService _organizationService;

        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<OrganizationDto>> CreateOrganizationAsync(OrganizationDto newOrganization)
        {
            var newOrgToAdd = await _organizationService.CreateAsync(User, newOrganization);

            if (newOrgToAdd is null)
                return BadRequest();

            return Ok(newOrgToAdd);
        }

        [HttpPost("edit")]
        public async Task<ActionResult<OrganizationDto>> EditOrganizationAsync(OrganizationDto editOrganization)
        {
            var editOrgToAdd = await _organizationService.EditAsync(User, editOrganization);

            if (editOrgToAdd is null)
                return BadRequest();

            return Ok(editOrgToAdd);
        }

        [HttpGet("account/default")]
        public async Task<ActionResult<List<OrganizationProjectDto>>> GetDefaultOrganizationAsync()
        {
            return Ok(await _organizationService.GetOrganizationsAsync(Guid.Parse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID).Value)));
        }
    }
}
