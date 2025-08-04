using System;
using TaskManagerConvertor.Models;

namespace TaskManagerConvertor.Services.Interfaces;

public interface IAccountHelperService
{
    Task<T?> AddAccountDetails<T>(IHeaderDictionary headers, T? type, CancellationToken cancellationToken);
}
