using System;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Services.Implementations;

public class GeneralService
{
    public static async Task<OrganizationAccount> VerifyAccountRelatesToOrganization(TaskManagerAPIDbContext context, Guid accountId, Guid organizationId)
    {
        return await context.OrganizationAccount.FirstOrDefaultAsync(o => o.AccountId == accountId && o.OrganizationId == organizationId);
    }

    public static async Task<ProjectItem> VerifyProjectInOrganization(TaskManagerAPIDbContext context, Guid projectId, Guid organizationId)
    {
        var project = await context.ProjectItems.FirstOrDefaultAsync(t => t.Id == projectId);

        if (project is null)
            return null;

        return project;
    }
}
