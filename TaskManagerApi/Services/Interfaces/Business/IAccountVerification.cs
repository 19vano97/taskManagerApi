using System;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagerApi.Services.Interfaces.Business;

public interface IAccountVerification
{
    Task<bool> VerifyAccountInOrganization(Guid accountId, Guid organizationId, CancellationToken cancellationToken);
    Task<bool> VerifyAccountInOrganizationByProject(Guid accountId, Guid projectId, CancellationToken cancellationToken);
}
