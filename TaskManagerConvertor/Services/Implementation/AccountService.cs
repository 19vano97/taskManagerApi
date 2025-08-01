using System;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Services.Interfaces;

namespace TaskManagerConvertor.Services.Implementation;

public class AccountService : IAccountService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHelperService _helperService;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IHttpClientFactory httpClientFactory,
                          IHelperService helperService,
                          ILogger<AccountService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _helperService = helperService;
        _logger = logger;
    }

    public async Task<RequestResult<List<AccountDto>>> GetAccountDetailsByIds(IHeaderDictionary headers,
                                                                              List<Guid> accountIds,
                                                                              CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.AUTH_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForIdentityServer(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<List<AccountDto>>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.PostAsync(Constants.IdentityServer.Api.GET_MULTIPLE_DETAILS,
                                                  new StringContent(JsonConvert.SerializeObject(accountIds.Distinct()),
                                                  Encoding.UTF8, "application/json"), cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());
            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out List<AccountDto>? account))
            {
                return new RequestResult<List<AccountDto>>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            return new RequestResult<List<AccountDto>>
            {
                IsSuccess = true,
                Data = account
            };
        }

        return new RequestResult<List<AccountDto>>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<AccountDto>> GetOwnAccountDetails(IHeaderDictionary headers, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.AUTH_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForIdentityServer(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<AccountDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.GetAsync(Constants.IdentityServer.Api.GET_OWN_DETAILS, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());
            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out AccountDto? account))
            {
                return new RequestResult<AccountDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            return new RequestResult<AccountDto>
            {
                IsSuccess = true,
                Data = account
            };
        }

        return new RequestResult<AccountDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<AccountDto>> PostAccountDetails(IHeaderDictionary headers, AccountDto account, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.AUTH_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForIdentityServer(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<AccountDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.PostAsync(Constants.IdentityServer.Api.POST_OWN_DETAILS,
                                                  new StringContent(JsonConvert.SerializeObject(account),
                                                    Encoding.UTF8, "application/json"),
                                                  cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());
            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out AccountDto? accountResponse))
            {
                return new RequestResult<AccountDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            return new RequestResult<AccountDto>
            {
                IsSuccess = true,
                Data = accountResponse
            };
        }

        return new RequestResult<AccountDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<AccountDto>> PrecreateInvitedAccount(IHeaderDictionary headers, AccountDto account, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.AUTH_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForIdentityServer(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<AccountDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.PostAsync(Constants.IdentityServer.Api.POST_INVITE_MEMBER,
                                                  new StringContent(JsonConvert.SerializeObject(account),
                                                    Encoding.UTF8, "application/json"),
                                                  cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());
            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out AccountDto? accountResponse))
            {
                return new RequestResult<AccountDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            return new RequestResult<AccountDto>
            {
                IsSuccess = true,
                Data = accountResponse
            };
        }

        return new RequestResult<AccountDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }
}
