using System;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Models.Organization;

namespace TaskManagerConvertor.Services.Interfaces;

public interface IOrganizationService
{
    public Task<RequestResult<OrganizationDto>> CreateOrganizationAsync(OrganizationDto organization, CancellationToken cancellationToken);
    public Task<RequestResult<OrganizationDto>> EditOrganizationAsync(Guid organizationId, OrganizationDto organization, CancellationToken cancellationToken);
    public Task<RequestResult<List<OrganizationDto>>> GetSelfOrganizationsAsync(CancellationToken cancellationToken);
    public Task<RequestResult<OrganizationDto>> GetOrganizationByIdAsync(Guid organizationId, CancellationToken cancellationToken);
    public Task<RequestResult<OrganizationDto>> GetOrganizationAccountsAsync(Guid organizationId, CancellationToken cancellationToken);
    public Task<RequestResult<OrganizationDto>> AddNewAccountToOrganization(Guid organizationId, Guid accountId, CancellationToken cancellationToken);
}
