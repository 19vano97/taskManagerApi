using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerConvertor.Models.Organization;
using TaskManagerConvertor.Services.Interfaces;

namespace TaskManagerConvertor.Controllers.TaskManager
{
    [Route("api/organization")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;
        private readonly ILogger<OrganizationController> _logger;

        public OrganizationController(IOrganizationService organizationService,
                                ILogger<OrganizationController> logger)
        {
            _organizationService = organizationService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<ActionResult<OrganizationDto>> CreateOrganizationAsync(OrganizationDto newOrganization,
                                                                                 CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateOrganizationAsync called");

            var response = await _organizationService.CreateOrganizationAsync(newOrganization, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpPost("{organizationId}/edit")]
        public async Task<ActionResult<OrganizationDto>> EditOrganizationAsync(OrganizationDto editOrganization,
                                                                               Guid organizationId,
                                                                               CancellationToken cancellationToken)
        {
            _logger.LogInformation("EditOrganizationAsync called for organizationId: {OrgId}", organizationId);

            var response = await _organizationService.EditOrganizationAsync(organizationId, editOrganization, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }


        [HttpGet("details/me")]
        public async Task<ActionResult<List<OrganizationDto>>> GetSelfOrganizationsAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetSelfOrganizationsAsync called");

            var response = await _organizationService.GetSelfOrganizationsAsync(cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpGet("{organizationId}/details")]
        public async Task<ActionResult<OrganizationDto>> GetOrganizationByIdAsync(Guid organizationId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetOrganizationByIdAsync called for organizationId: {OrgId}", organizationId);

            var response = await _organizationService.GetOrganizationByIdAsync(organizationId, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpGet("{organizationId}/accounts")]
        public async Task<ActionResult<OrganizationDto>> GetOrganizationAccountsAsync(Guid organizationId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetOrganizationInfoAsync called for organizationId: {OrgId}", organizationId);

            var response = await _organizationService.GetOrganizationAccountsAsync(organizationId, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpPost("details/{organizationId}/new-member/{accountId}")]
        public async Task<ActionResult<OrganizationDto>> AddNewAccountToOrganization(Guid organizationId, Guid accountId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddNewAccountToOrganization called for organizationId: {OrgId}, accountId: {AccountId}", organizationId, accountId);

            var response = await _organizationService.AddNewAccountToOrganization(organizationId, accountId, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }
    }
}
