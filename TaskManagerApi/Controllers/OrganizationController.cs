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
        public async Task<ActionResult<OrganizationDto>> CreateOrganizationAsync(OrganizationDto newOrganization,
                                                                                 CancellationToken cancellationToken)
        {
            var newOrgToAdd = await _organizationService.CreateAsync(User, newOrganization, cancellationToken);

            if (!newOrgToAdd.Success)
                return BadRequest(newOrgToAdd.ErrorMessage);

            return Ok(newOrgToAdd.Data);
        }

        [HttpPost("{organizationId}/edit")]
        public async Task<ActionResult<OrganizationDto>> EditOrganizationAsync(OrganizationDto editOrganization,
                                                                               Guid organizationId,
                                                                               CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                    || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId, cancellationToken)
                    || organizationId != editOrganization.Id)
                return BadRequest();

            var editOrgToAdd = await _organizationService.EditAsync(User, editOrganization, cancellationToken);

            if (!editOrgToAdd.Success)
                return BadRequest(editOrgToAdd.ErrorMessage);

            return Ok(editOrgToAdd.Data);
        }

        [HttpGet("details/me")]
        public async Task<ActionResult<List<OrganizationProjectDto>>> GetSelfOrganizationsAsync(CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId))
                return BadRequest();

            var res = await _organizationService.GetOrganizationsByAccountAsync(accountId, cancellationToken);

            if (!res.Success)
            {
                if (res.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(res.ErrorMessage);
            }

            return Ok(res.Data);
        }

        [HttpGet("{organizationId}/details")]
        public async Task<ActionResult<OrganizationProjectDto>> GetOrganizationByIdAsync(Guid organizationId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId, cancellationToken))
                return BadRequest();

            var res = await _organizationService.GetOrganizationProjectsAsync(organizationId, cancellationToken);
            if (!res.Success)
            {
                if (res.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(res.ErrorMessage);
            }

            return Ok(res.Data);
        }

        [HttpGet("{organizationId}/accounts")]
        public async Task<ActionResult<OrganizationAccountsDto>> GetOrganizationInfoAsync(Guid organizationId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId, cancellationToken))
                return BadRequest();

            var organization = await _organizationService.GetOrganizationProjectsAsync(organizationId, cancellationToken);
            if (!organization.Success)
            {
                if (organization.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(organization.ErrorMessage);
            }

            return Ok(organization.Data);
        }

        [HttpPost("details/{organizationId}/new-member/{accountId}")]
        public async Task<ActionResult<OrganizationProjectDto>> AddNewAccountToOrganization(Guid organizationId, Guid accountId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountIdInitiator)
                || !await _accountVerification.VerifyAccountInOrganization(accountIdInitiator, organizationId, cancellationToken))
                return BadRequest();

            var organization = await _organizationService.AddNewMemberToOrganization(organizationId, accountId, cancellationToken);
            if (!organization.Success)
            {
                if (organization.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(organization.ErrorMessage);
            }

            return Ok(organization.Data);
        }
    }
}
