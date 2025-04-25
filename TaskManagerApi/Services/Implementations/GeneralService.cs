using System;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Services.Implementations;

public static class GeneralService
{
    private static readonly TaskManagerAPIDbContext _context;

    public static async Task<OrganizationAccount> VerifyAccountRelatesToOrganization(Guid accountId, Guid organizationId)
    {
        return await _context.OrganizationAccount.FirstOrDefaultAsync(o => o.AccountId == accountId && o.OrganizationId == organizationId);
    }

    public static async Task<ProjectItem> VerifyProjectInOrganization(Guid projectId, Guid organizationId)
    {
        var project = await _context.ProjectItems.FirstOrDefaultAsync(t => t.Id == projectId);

        if (project is null)
            return null;

        return project;
    }
}
