using System;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Models.Organization;
using TaskManagerConvertor.Services.Interfaces;

namespace TaskManagerConvertor.Services.Implementation;

public class OrganizationService : IOrganizationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAccountHelperService _accountHelperService;
    private readonly IHelperService _helperService;
    private readonly ILogger<OrganizationService> _logger;

    public OrganizationService(IHttpClientFactory httpClientFactory,
                               IAccountHelperService accountHelperService,
                               IHelperService helperService,
                               ILogger<OrganizationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _accountHelperService = accountHelperService;
        _helperService = helperService;
        _logger = logger;
    }

    public async Task<RequestResult<OrganizationDto>> AddNewAccountToOrganization(IHeaderDictionary headers, Guid organizationId, Guid accountId, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<OrganizationDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.PostAsync($"api/organization/details/{organizationId}/new-member/{accountId}", null, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out OrganizationDto? ticket))
            {
                return new RequestResult<OrganizationDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _accountHelperService.AddAccountDetails(headers, ticket!, cancellationToken);
            ticket.AccountIds = null;

            return new RequestResult<OrganizationDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }

        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<OrganizationDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<OrganizationDto>> CreateOrganizationAsync(IHeaderDictionary headers, OrganizationDto organization, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<OrganizationDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.PostAsync(Constants.TaskManagerApi.Organization.POST_CREATE_ORGANIZATION,
                                                        new StringContent(JsonConvert.SerializeObject(organization),
                                                        Encoding.UTF8,
                                                        "application/json"));
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out OrganizationDto? ticket))
            {
                return new RequestResult<OrganizationDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _accountHelperService.AddAccountDetails(headers, ticket!, cancellationToken);
            ticket.AccountIds = null;

            return new RequestResult<OrganizationDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }
        
        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<OrganizationDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<OrganizationDto>> EditOrganizationAsync(IHeaderDictionary headers, Guid organizationId, OrganizationDto organization, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<OrganizationDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.PostAsync($"/api/organization/{organizationId}/edit",
                                                        new StringContent(JsonConvert.SerializeObject(organization),
                                                        Encoding.UTF8,
                                                        "application/json"));
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out OrganizationDto? ticket))
            {
                return new RequestResult<OrganizationDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _accountHelperService.AddAccountDetails(headers, ticket!, cancellationToken);
            ticket.AccountIds = null;

            return new RequestResult<OrganizationDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }
        
        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<OrganizationDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<OrganizationDto>> GetOrganizationAccountsAsync(IHeaderDictionary headers, Guid organizationId, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<OrganizationDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.GetAsync($"/api/organization/{organizationId}/accounts", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out OrganizationDto? ticket))
            {
                return new RequestResult<OrganizationDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _accountHelperService.AddAccountDetails(headers, ticket!, cancellationToken);
            ticket.AccountIds = null;

            return new RequestResult<OrganizationDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }
        
        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<OrganizationDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<OrganizationDto>> GetOrganizationByIdAsync(IHeaderDictionary headers, Guid organizationId, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<OrganizationDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.GetAsync($"/api/organization/{organizationId}/details", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out OrganizationDto? ticket))
            {
                return new RequestResult<OrganizationDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _accountHelperService.AddAccountDetails(headers, ticket!, cancellationToken);
            ticket.AccountIds = null;

            return new RequestResult<OrganizationDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }
        
        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<OrganizationDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<List<OrganizationDto>>> GetSelfOrganizationsAsync(IHeaderDictionary headers, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<List<OrganizationDto>>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.GetAsync(Constants.TaskManagerApi.Organization.GET_MY_ORGANIZATION, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out List<OrganizationDto>? ticket))
            {
                return new RequestResult<List<OrganizationDto>>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _accountHelperService.AddAccountDetails(headers, ticket!, cancellationToken);
            
            foreach (var item in ticket)
            {
                item.AccountIds = null;
            }

            return new RequestResult<List<OrganizationDto>>
            {
                IsSuccess = true,
                Data = ticket
            };
        }
        
        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<List<OrganizationDto>>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }
}
