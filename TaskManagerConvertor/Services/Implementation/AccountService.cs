using System;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Services.Interfaces;

namespace TaskManagerConvertor.Services.Implementation;

public class AccountService : IAccountService
{
    private readonly HttpClient _httpClient;
    private readonly IHelperService _httpHelperService;
    private readonly ILogger<AccountService> _logger;

    public AccountService(HttpClient httpClient,
                          IHelperService httpHelperService,
                          ILogger<AccountService> logger)
    {
        _httpClient = httpClient;
        _httpHelperService = httpHelperService;
        _logger = logger;
    }

    public async Task<RequestResult<List<AccountDto>>> GetAccountDetailsByIds(List<Guid> accountIds,
                                                                              CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsync(Constants.IdentityServer.Api.GET_MULTIPLE_DETAILS,
                                                  new StringContent(JsonConvert.SerializeObject(accountIds.Distinct()),
                                                  Encoding.UTF8, "application/json"), cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());
            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_httpHelperService.TryParseJsonToDto(data, out List<AccountDto>? account))
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

    public async Task<RequestResult<AccountDto>> GetOwnAccountDetails(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(Constants.IdentityServer.Api.GET_OWN_DETAILS, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());
            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_httpHelperService.TryParseJsonToDto(data, out AccountDto? account))
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

    public async Task<RequestResult<AccountDto>> PostAccountDetails(AccountDto account, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsync(Constants.IdentityServer.Api.POST_OWN_DETAILS,
                                                  new StringContent(JsonConvert.SerializeObject(account),
                                                    Encoding.UTF8, "application/json"),
                                                  cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());
            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_httpHelperService.TryParseJsonToDto(data, out AccountDto? accountResponse))
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

    public async Task<RequestResult<AccountDto>> PrecreateInvitedAccount(AccountDto account, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsync(Constants.IdentityServer.Api.POST_INVITE_MEMBER,
                                                  new StringContent(JsonConvert.SerializeObject(account),
                                                    Encoding.UTF8, "application/json"),
                                                  cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());
            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_httpHelperService.TryParseJsonToDto(data, out AccountDto? accountResponse))
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
