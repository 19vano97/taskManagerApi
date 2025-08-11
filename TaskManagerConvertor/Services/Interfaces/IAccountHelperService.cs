using System;
using TaskManagerConvertor.Models;

namespace TaskManagerConvertor.Services.Interfaces;

public interface IAccountHelperService
{
    Task<T?> AddAccountDetails<T>(T? type, CancellationToken cancellationToken);
}
