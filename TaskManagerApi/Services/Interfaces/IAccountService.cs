using System;
using TaskManagerApi.Models.Account;

namespace TaskManagerApi.Services.Interfaces;

public interface IAccountService
{
    Task<AccountDefaultInfo> GetAccountDefaultInfoAsync(Guid userId);
}
