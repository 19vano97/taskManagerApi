using System;
using TaskManagerApi.Data;
using TaskManagerApi.Models.Account;
using TaskManagerApi.Services.Interfaces;

namespace TaskManagerApi.Services.Implementations;

public class AccountService : IAccountService
{
    private readonly TaskManagerAPIDbContext _context;
    private readonly ILogger<AccountService> _logger;
    
    public AccountService(TaskManagerAPIDbContext context, ILogger<AccountService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Task<AccountDefaultInfo> GetAccountDefaultInfoAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}
