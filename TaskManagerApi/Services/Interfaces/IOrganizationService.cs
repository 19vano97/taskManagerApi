using System;
using System.Security.Claims;
using TaskManagerApi.Models.OrganizationModel;

namespace TaskManagerApi.Services.Interfaces;

public interface IOrganizationService
{
    Task<OrganizationDto> Create(ClaimsPrincipal user, OrganizationDto newOgranization);
    Task<OrganizationDto> Edit(ClaimsPrincipal user, OrganizationDto newOgranization);
    Task<OrganizationDto> Delete(OrganizationDto newOgranization);
}
