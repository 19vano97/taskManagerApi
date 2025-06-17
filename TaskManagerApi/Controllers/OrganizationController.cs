using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
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
        private IAccountVerification _accountVerification;

        public OrganizationController(IOrganizationService organizationService,
                                      IAccountVerification accountVerification)
        {
            _organizationService = organizationService;
            _accountVerification = accountVerification;
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
            if (this.Request.Headers.TryGetValue("organizationId", out var organizationIdString)
                || !Guid.TryParse(organizationIdString, out Guid organizationId)
                || !Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId))
                return BadRequest();

            var editOrgToAdd = await _organizationService.EditAsync(User, editOrganization);

            if (editOrgToAdd is null)
                return BadRequest();

            return Ok(editOrgToAdd);
        }

        [HttpGet("account/default")]
        public async Task<ActionResult<List<OrganizationProjectDto>>> GetDefaultOrganizationAsync(Guid organizationId1)
        {
            if (!this.Request.Headers.TryGetValue("organizationId", out var organizationIdString)
                || !Guid.TryParse(organizationIdString, out Guid organizationId)
                || !Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId))
                return BadRequest();

            return Ok(await _organizationService.GetOrganizationsByAccountAsync(Guid.Parse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID).Value)));
        }

        [HttpGet("info/accounts")]
        public async Task<ActionResult<OrganizationAccountsDto>> GetOrganizationInfoAsync(Guid organizationId1)
        {
            if (!this.Request.Headers.TryGetValue("organizationId", out var organizationIdString)
                || !Guid.TryParse(organizationIdString, out Guid organizationId)
                || !Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId))
                return BadRequest();

            var organization = await _organizationService.GetOrganizationProjectsAsync(organizationId);

            if (organization is null)
                return NotFound();

            return Ok(organization);
        }
    }
}
