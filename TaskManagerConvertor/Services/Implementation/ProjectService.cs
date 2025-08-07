using System;
using System.Reflection.Metadata;
using System.Text;
using Newtonsoft.Json;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Models.Project;
using TaskManagerConvertor.Services.Interfaces;

namespace TaskManagerConvertor.Services.Implementation;

public class ProjectService : IProjectService
{
    private readonly IAccountHelperService _accountHelperService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHelperService _helperService;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(IHttpClientFactory httpClientFactory,
                               IAccountHelperService accountHelperService,
                               IHelperService helperService,
                               ILogger<ProjectService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _accountHelperService = accountHelperService;
        _helperService = helperService;
        _logger = logger;
    }

    public async Task<RequestResult<ProjectItemDto>> CreateProjectAsync(IHeaderDictionary headers, ProjectItemDto project, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<ProjectItemDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.PostAsync(Constants.TaskManagerApi.Project.POST_CREATE_PROJECT,
                                                        new StringContent(JsonConvert.SerializeObject(project),
                                                        Encoding.UTF8,
                                                        "application/json"));
        httpClient.Dispose();
        
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out ProjectItemDto? ticket))
            {
                return new RequestResult<ProjectItemDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _accountHelperService.AddAccountDetails(headers, ticket!, cancellationToken);

            return new RequestResult<ProjectItemDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }
        
        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<ProjectItemDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<ProjectItemDto>> DeleteProjectAsync(IHeaderDictionary headers, Guid projectId, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<ProjectItemDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.DeleteAsync($"/api/project/{projectId}/delete", cancellationToken);
        httpClient.Dispose();

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out ProjectItemDto? ticket))
            {
                return new RequestResult<ProjectItemDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _accountHelperService.AddAccountDetails(headers, ticket!, cancellationToken);

            return new RequestResult<ProjectItemDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }

        return new RequestResult<ProjectItemDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<ProjectItemDto>> EditProjectByIdAsync(IHeaderDictionary headers, ProjectItemDto editProject, Guid projectId, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<ProjectItemDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.PostAsync($"api/project/{projectId}/edit",
                                                        new StringContent(JsonConvert.SerializeObject(editProject),
                                                        Encoding.UTF8,
                                                        "application/json"));
        httpClient.Dispose();

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out ProjectItemDto? ticket))
            {
                return new RequestResult<ProjectItemDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _accountHelperService.AddAccountDetails(headers, ticket!, cancellationToken);

            return new RequestResult<ProjectItemDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }
        
        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<ProjectItemDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<List<ProjectItemDto>>> GetAllProjectsWithTasksListAsync(IHeaderDictionary headers, Guid organizationId, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<List<ProjectItemDto>>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.GetAsync($"/api/project/all/{organizationId}", cancellationToken);
        httpClient.Dispose();
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out List<ProjectItemDto>? ticket))
            {
                return new RequestResult<List<ProjectItemDto>>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _accountHelperService.AddAccountDetails(headers, ticket!, cancellationToken);

            return new RequestResult<List<ProjectItemDto>>
            {
                IsSuccess = true,
                Data = ticket
            };
        }

        return new RequestResult<List<ProjectItemDto>>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<ProjectItemDto>> GetProjectByIdAsync(IHeaderDictionary headers, Guid projectId, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<ProjectItemDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.GetAsync($"/api/project/{projectId}", cancellationToken);
        httpClient.Dispose();
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out ProjectItemDto? ticket))
            {
                return new RequestResult<ProjectItemDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _accountHelperService.AddAccountDetails(headers, ticket!, cancellationToken);

            return new RequestResult<ProjectItemDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }

        return new RequestResult<ProjectItemDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<ProjectItemDto>> GetProjectWithTasksByIdAsync(IHeaderDictionary headers, Guid projectId, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<ProjectItemDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.GetAsync($"/api/project/{projectId}/tasks", cancellationToken);
        httpClient.Dispose();
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out ProjectItemDto? ticket))
            {
                return new RequestResult<ProjectItemDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _accountHelperService.AddAccountDetails(headers, ticket!, cancellationToken);

            return new RequestResult<ProjectItemDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }

        return new RequestResult<ProjectItemDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }
}
