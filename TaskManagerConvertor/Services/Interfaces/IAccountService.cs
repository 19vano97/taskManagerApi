using System;
using TaskManagerConvertor.Models;

namespace TaskManagerConvertor.Services.Interfaces;

public interface IAccountService
{
    public Task<RequestResult<AccountDto>> GetOwnAccountDetails(IHeaderDictionary headers, CancellationToken cancellationToken);
    public Task<RequestResult<List<AccountDto>>> GetAccountDetailsByIds(IHeaderDictionary headers, List<Guid> accountIds, CancellationToken cancellationToken);
}
