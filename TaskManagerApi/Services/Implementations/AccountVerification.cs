using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Services.Interfaces;

namespace TaskManagerApi.Services.Implementations;

public class AccountVerification : IAccountVerification
{
    private readonly ILogger<AccountVerification> _logger;
    private readonly TicketManagerAPIDbContext _context;

    public AccountVerification(ILogger<AccountVerification> logger, TicketManagerAPIDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> VerifyAccountInOrganization(Guid accountId, Guid organizationId, CancellationToken cancellationToken)
    {
        var mappedAccountOrganization = await _context.OrganizationAccount
            .FirstOrDefaultAsync(a => a.AccountId == accountId && a.OrganizationId == organizationId, cancellationToken);
        return mappedAccountOrganization != null ? true : false;
    }

    public async Task<bool> VerifyAccountInOrganizationByProject(Guid accountId, Guid projectId, CancellationToken cancellationToken)
    {
        var project = await _context.ProjectItems.FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
        var accountOrganization = await _context.OrganizationAccount
            .FirstOrDefaultAsync(a => a.AccountId == accountId && a.OrganizationId == project.OrganizationId, cancellationToken);

        return accountOrganization != null ? true : false;
    }
}
