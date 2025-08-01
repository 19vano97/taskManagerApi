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
        private ILogger<OrganizationController> _logger;

        public OrganizationController(IOrganizationService organizationService,
                                      IAccountVerification accountVerification,
                                      ILogger<OrganizationController> logger)
        {
            _organizationService = organizationService;
            _accountVerification = accountVerification;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<ActionResult<OrganizationDto>> CreateOrganizationAsync(OrganizationDto newOrganization,
                                                                                 CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateOrganizationAsync called");
            var newOrgToAdd = await _organizationService.CreateAsync(User, newOrganization, cancellationToken);

            if (!newOrgToAdd.IsSuccess)
            {
                _logger.LogWarning("Failed to create organization. Error: {Error}", newOrgToAdd.ErrorMessage);
                return BadRequest(newOrgToAdd.ErrorMessage);
            }

            _logger.LogInformation("Organization created successfully. Id: {OrgId}", newOrgToAdd.Data?.Id);
            return Ok(newOrgToAdd.Data);
        }

        [HttpPost("{organizationId}/edit")]
        public async Task<ActionResult<OrganizationDto>> EditOrganizationAsync(OrganizationDto editOrganization,
                                                                               Guid organizationId,
                                                                               CancellationToken cancellationToken)
        {
            _logger.LogInformation("EditOrganizationAsync called for organizationId: {OrgId}", organizationId);
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                    || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId, cancellationToken)
                    || organizationId != editOrganization.Id)
            {
                _logger.LogWarning("Account verification failed or organizationId mismatch for EditOrganizationAsync. organizationId: {OrgId}", organizationId);
                return BadRequest();
            }

            var editOrgToAdd = await _organizationService.EditAsync(User, editOrganization, cancellationToken);

            if (!editOrgToAdd.IsSuccess)
            {
                _logger.LogWarning("Failed to edit organization. Error: {Error}", editOrgToAdd.ErrorMessage);
                return BadRequest(editOrgToAdd.ErrorMessage);
            }

            _logger.LogInformation("Organization edited successfully. Id: {OrgId}", editOrgToAdd.Data?.Id);
            return Ok(editOrgToAdd.Data);
        }

        [HttpGet("details/me")]
        public async Task<ActionResult<List<OrganizationProjectDto>>> GetSelfOrganizationsAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetSelfOrganizationsAsync called");
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId))
            {
                _logger.LogWarning("AccountId not found in token for GetSelfOrganizationsAsync");
                return BadRequest();
            }

            var res = await _organizationService.GetOrganizationsByAccountAsync(accountId, cancellationToken);

            if (!res.IsSuccess)
            {
                _logger.LogWarning("Failed to get organizations for accountId: {AccountId}. Error: {Error}", accountId, res.ErrorMessage);
                if (res.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(res.ErrorMessage);
            }

            _logger.LogInformation("Organizations fetched for accountId: {AccountId}", accountId);
            return Ok(res.Data);
        }

        [HttpGet("{organizationId}/details")]
        public async Task<ActionResult<OrganizationProjectDto>> GetOrganizationByIdAsync(Guid organizationId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetOrganizationByIdAsync called for organizationId: {OrgId}", organizationId);
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId, cancellationToken))
            {
                _logger.LogWarning("Account verification failed for GetOrganizationByIdAsync. organizationId: {OrgId}", organizationId);
                return BadRequest();
            }

            var res = await _organizationService.GetOrganizationProjectsAsync(organizationId, cancellationToken);
            if (!res.IsSuccess)
            {
                _logger.LogWarning("Failed to get organization by id: {OrgId}. Error: {Error}", organizationId, res.ErrorMessage);
                if (res.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(res.ErrorMessage);
            }

            _logger.LogInformation("Organization fetched successfully. Id: {OrgId}", organizationId);
            return Ok(res.Data);
        }

        [HttpGet("{organizationId}/accounts")]
        public async Task<ActionResult<OrganizationAccountsDto>> GetOrganizationInfoAsync(Guid organizationId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetOrganizationInfoAsync called for organizationId: {OrgId}", organizationId);
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId, cancellationToken))
            {
                _logger.LogWarning("Account verification failed for GetOrganizationInfoAsync. organizationId: {OrgId}", organizationId);
                return BadRequest();
            }

            var organization = await _organizationService.GetOrganizationProjectsAsync(organizationId, cancellationToken);
            if (!organization.IsSuccess)
            {
                _logger.LogWarning("Failed to get organization info. organizationId: {OrgId}. Error: {Error}", organizationId, organization.ErrorMessage);
                if (organization.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(organization.ErrorMessage);
            }

            _logger.LogInformation("Organization info fetched successfully. Id: {OrgId}", organizationId);
            return Ok(organization.Data);
        }

        [HttpPost("details/{organizationId}/new-member/{accountId}")]
        public async Task<ActionResult<OrganizationProjectDto>> AddNewAccountToOrganization(Guid organizationId, Guid accountId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddNewAccountToOrganization called for organizationId: {OrgId}, accountId: {AccountId}", organizationId, accountId);
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountIdInitiator)
                || !await _accountVerification.VerifyAccountInOrganization(accountIdInitiator, organizationId, cancellationToken))
            {
                _logger.LogWarning("Account verification failed for AddNewAccountToOrganization. organizationId: {OrgId}, accountId: {AccountId}", organizationId, accountId);
                return BadRequest();
            }

            var organization = await _organizationService.AddNewMemberToOrganization(organizationId, accountId, cancellationToken);
            if (!organization.IsSuccess)
            {
                _logger.LogWarning("Failed to add new member to organization. organizationId: {OrgId}, accountId: {AccountId}. Error: {Error}", organizationId, accountId, organization.ErrorMessage);
                if (organization.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(organization.ErrorMessage);
            }

            _logger.LogInformation("New member added to organization successfully. organizationId: {OrgId}, accountId: {AccountId}", organizationId, accountId);
            return Ok(organization.Data);
        }
    }
}
