using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Services.Interfaces;

namespace TaskManagerApi.Services.Implementations;

public class AccountVerification : IAccountVerification
{
    private readonly ILogger<AccountVerification> _logger;
    private readonly TaskManagerAPIDbContext _context;

    public AccountVerification(ILogger<AccountVerification> logger, TaskManagerAPIDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> VerifyAccountInOrganization(Guid accountId, Guid organizationId)
    {
        var mappedAccountOrganization = await _context.OrganizationAccount.FirstOrDefaultAsync(a => a.AccountId == accountId && a.OrganizationId == organizationId);
        return mappedAccountOrganization != null ? true : false;
    }

    public async Task<bool> VerifyAccountInOrganizationByProject(Guid accountId, Guid projectId)
    {
        var project = await _context.ProjectItems.FirstOrDefaultAsync(p => p.Id == projectId);
        var accountOrganization = await _context.OrganizationAccount.FirstOrDefaultAsync(a => a.AccountId == accountId && a.OrganizationId == project.OrganizationId);

        return accountOrganization != null ? true : false;
    }
}
