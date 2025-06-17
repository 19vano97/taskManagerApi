using System;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagerApi.Services.Interfaces;

public interface IAccountVerification
{
    Task<bool> VerifyAccountInOrganization(Guid accountId, Guid organizationId);
    Task<bool> VerifyAccountInOrganizationByProject(Guid accountId, Guid projectId);
}
