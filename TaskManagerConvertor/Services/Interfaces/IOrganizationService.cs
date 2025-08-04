using System;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Models.Organization;

namespace TaskManagerConvertor.Services.Interfaces;

public interface IOrganizationService
{
    public Task<RequestResult<OrganizationDto>> CreateOrganizationAsync(IHeaderDictionary headers, OrganizationDto organization, CancellationToken cancellationToken);
    public Task<RequestResult<OrganizationDto>> EditOrganizationAsync(IHeaderDictionary headers, Guid organizationId, OrganizationDto organization, CancellationToken cancellationToken);
    public Task<RequestResult<List<OrganizationDto>>> GetSelfOrganizationsAsync(IHeaderDictionary headers, CancellationToken cancellationToken);
    public Task<RequestResult<OrganizationDto>> GetOrganizationByIdAsync(IHeaderDictionary headers, Guid organizationId, CancellationToken cancellationToken);
    public Task<RequestResult<OrganizationDto>> GetOrganizationAccountsAsync(IHeaderDictionary headers, Guid organizationId, CancellationToken cancellationToken);
    public Task<RequestResult<OrganizationDto>> AddNewAccountToOrganization(IHeaderDictionary headers, Guid organizationId, Guid accountId, CancellationToken cancellationToken);
}
