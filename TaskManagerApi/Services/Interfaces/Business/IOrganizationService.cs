using System;
using System.Security.Claims;
using TaskManagerApi.Models;
using TaskManagerApi.Models.OrganizationModel;

namespace TaskManagerApi.Services.Interfaces.Business;

public interface IOrganizationService
{
    Task<ServiceResult<OrganizationDto>> CreateAsync(ClaimsPrincipal user, OrganizationDto newOgranization, CancellationToken cancellationToken);
    Task<ServiceResult<OrganizationDto>> EditAsync(ClaimsPrincipal user, OrganizationDto newOgranization, CancellationToken cancellationToken);
    Task<ServiceResult<OrganizationDto>> DeleteAsync(OrganizationDto newOgranization, CancellationToken cancellationToken);
    Task<ServiceResult<OrganizationDto>> GetOrganizationByIdAsync(Guid organizationId, CancellationToken cancellationToken);
    Task<ServiceResult<OrganizationDto>> GetOrganizationAccountAsync(Guid organizationId, CancellationToken cancellationToken);
    Task<ServiceResult<OrganizationDto>> AddNewMemberToOrganization(Guid organizationId, Guid accountId, CancellationToken cancellationToken);
    Task<ServiceResult<OrganizationDto>> GetOrganizationAsync(Guid organizationId, CancellationToken cancellationToken);
    Task<ServiceResult<List<OrganizationDto>>> GetOrganizationsByAccountAsync(Guid accountId, CancellationToken cancellationToken);
}
