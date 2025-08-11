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
    private readonly HttpClient _httpClient;
    private readonly IAccountHelperService _accountHelperService;
    private readonly IHelperService _helperService;
    private readonly ILogger<OrganizationService> _logger;

    public OrganizationService(IHttpClientFactory httpClientFactory,
                               IAccountHelperService accountHelperService,
                               IHelperService helperService,
                               ILogger<OrganizationService> logger)
    {
        _httpClient = httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        _accountHelperService = accountHelperService;
        _helperService = helperService;
        _logger = logger;
    }

    public async Task<RequestResult<OrganizationDto>> AddNewAccountToOrganization(Guid organizationId, Guid accountId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsync($"api/organization/details/{organizationId}/new-member/{accountId}", null, cancellationToken);

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

            ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);
            ticket!.AccountIds = null;

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

    public async Task<RequestResult<OrganizationDto>> CreateOrganizationAsync(OrganizationDto organization, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsync(Constants.TaskManagerApi.Organization.POST_CREATE_ORGANIZATION,
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

            ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);
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

    public async Task<RequestResult<OrganizationDto>> EditOrganizationAsync(Guid organizationId, OrganizationDto organization, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsync($"/api/organization/{organizationId}/edit",
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

            ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);
            ticket!.AccountIds = null;

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

    public async Task<RequestResult<OrganizationDto>> GetOrganizationAccountsAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/api/organization/{organizationId}/accounts", cancellationToken);

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

            ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);
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

    public async Task<RequestResult<OrganizationDto>> GetOrganizationByIdAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/api/organization/{organizationId}/details", cancellationToken);

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

            ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);
            ticket!.AccountIds = null;

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

    public async Task<RequestResult<List<OrganizationDto>>> GetSelfOrganizationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync(Constants.TaskManagerApi.Organization.GET_MY_ORGANIZATION, cancellationToken);
            
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

                ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);

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
        }
        catch (System.Exception ex)
        {
            _logger.LogWarning(ex.ToString());

            return new RequestResult<List<OrganizationDto>>
            {
                IsSuccess = false,
                ErrorMessage = ex.ToString()
            };
        }
        
         return new RequestResult<List<OrganizationDto>>
        {
            IsSuccess = false,
            ErrorMessage = "issue"
        };
    }
}
