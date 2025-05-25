using System;
using System.Security.Claims;
using TaskManagerApi.Models.OrganizationModel;

namespace TaskManagerApi.Services.Interfaces;

public interface IOrganizationService
{
    Task<OrganizationDto> CreateAsync(ClaimsPrincipal user, OrganizationDto newOgranization);
    Task<OrganizationDto> EditAsync(ClaimsPrincipal user, OrganizationDto newOgranization);
    Task<OrganizationDto> DeleteAsync(OrganizationDto newOgranization);
    Task<OrganizationProjectDto> GetOrganizationProjectsAsync(Guid organizationId);
    Task<OrganizationDto> GetOrganizationAsync(Guid organizationId);
    Task<List<OrganizationProjectDto>> GetOrganizationsAsync(Guid accountId);
}
