using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Services.Interfaces;

namespace TaskManagerConvertor.Services.Implementation;

public class HelperService : IHelperService
{
    private readonly ILogger<HelperService> _logger;

    public HelperService(ILogger<HelperService> logger)
    {
        _logger = logger;
    }

    public RequestResult<HttpClient> SetupHttpClientForIdentityServer(IHeaderDictionary headers, ref HttpClient httpClient)
    {
        if (headers.TryGetValue(Constants.Settings.Header.AUTHORIZATION, out var authHeader))
        {
            var authHeaderValue = authHeader.ToString();
            var parts = authHeaderValue.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(parts[0], parts[1]);
            }
            else
            {
                return new RequestResult<HttpClient>
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid Authorization header format"
                };
            }
        }
        else
        {
            return new RequestResult<HttpClient>
            {
                IsSuccess = false,
                ErrorMessage = "Issue with organzation or token"
            };
        }

        return new RequestResult<HttpClient>
        {
            IsSuccess = true,
            Data = httpClient
        };
    }

    public RequestResult<HttpClient> SetupHttpClientForTaskManager(IHeaderDictionary headers, HttpClient httpClient)
    {
        if (headers.TryGetValue(Constants.Settings.Header.AUTHORIZATION, out var authHeader))
        {
            var authHeaderValue = authHeader.ToString();
            var parts = authHeaderValue.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(parts[0], parts[1]);
            }
            else
            {
                return new RequestResult<HttpClient>
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid Authorization header format"
                };
            }

            if (headers.TryGetValue(Constants.Settings.Header.ORGANIZATION, out var organizationIdString))
            {
                httpClient.DefaultRequestHeaders.Add(Constants.Settings.Header.ORGANIZATION, organizationIdString.ToString());
            }
        }
        else
        {
            return new RequestResult<HttpClient>
            {
                IsSuccess = false,
                ErrorMessage = "Issue with organzation or token"
            };
        }

        return new RequestResult<HttpClient>
        {
            IsSuccess = true,
            Data = httpClient
        };
    }

    public bool TryParseJsonToDto<T>(string data, out T? type)
    {
        try
        {
            _logger.LogInformation(typeof(T).ToString());
            type = JsonConvert.DeserializeObject<T>(data)!;
            return true;
        }
        catch (System.Exception ex)
        {
            _logger.LogError($"Error with parcing json to {typeof(T)}: {ex.ToString()}");
            type = default;

            return false;
        }
    }
}
