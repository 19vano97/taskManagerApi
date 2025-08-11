using System.Net.Http;
using TaskManagerConvertor.Models;

namespace TaskManagerConvertor.Services.Interfaces;

public interface IHelperService
{
    RequestResult<HttpClient> SetupHttpClientForIdentityServer(IHeaderDictionary headers, ref HttpClient httpClient);
    RequestResult<HttpClient> SetupHttpClientForTaskManager(IHeaderDictionary headers, HttpClient httpClient);
    bool TryParseJsonToDto<T>(string data, out T? type);
}
