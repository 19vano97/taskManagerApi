using System;
using TaskManagerConvertor.Models;

namespace TaskManagerConvertor.Services.Interfaces;

public interface IHelperService
{
    RequestResult<HttpClient> SetupHttpClientForTaskManager(IHeaderDictionary headers, ref HttpClient httpClient);
    RequestResult<HttpClient> SetupHttpClientForIdentityServer(IHeaderDictionary headers, ref HttpClient httpClient);
    Task<T?> AddAccountDetails<T>(IHeaderDictionary headers, T? type, CancellationToken cancellationToken);
    bool TryParseJsonToDto<T>(string data, out T? accountDto);
}
