using System;
using TaskManagerConvertor.Models;

namespace TaskManagerConvertor.Services.Interfaces;

public interface IAccountService
{
    public Task<RequestResult<AccountDto>> GetOwnAccountDetails(CancellationToken cancellationToken);
    public Task<RequestResult<AccountDto>> PostAccountDetails(AccountDto account, CancellationToken cancellationToken);
    public Task<RequestResult<AccountDto>> PrecreateInvitedAccount(AccountDto account, CancellationToken cancellationToken);
    public Task<RequestResult<List<AccountDto>>> GetAccountDetailsByIds(List<Guid> accountIds, CancellationToken cancellationToken);
}
