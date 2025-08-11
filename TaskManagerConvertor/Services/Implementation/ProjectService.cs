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
    private readonly HttpClient _httpClient;
    private readonly IHelperService _helperService;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(IHttpClientFactory httpClientFactory,
                               IAccountHelperService accountHelperService,
                               IHelperService helperService,
                               ILogger<ProjectService> logger)
    {
        _httpClient = httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        _accountHelperService = accountHelperService;
        _helperService = helperService;
        _logger = logger;
    }

    public async Task<RequestResult<ProjectItemDto>> CreateProjectAsync(ProjectItemDto project, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsync(Constants.TaskManagerApi.Project.POST_CREATE_PROJECT,
                                                        new StringContent(JsonConvert.SerializeObject(project),
                                                        Encoding.UTF8,
                                                        "application/json"));
        
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

            ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);

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

    public async Task<RequestResult<ProjectItemDto>> DeleteProjectAsync(Guid projectId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.DeleteAsync($"/api/project/{projectId}/delete", cancellationToken);

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

            ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);

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

    public async Task<RequestResult<ProjectItemDto>> EditProjectByIdAsync(ProjectItemDto editProject, Guid projectId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsync($"api/project/{projectId}/edit",
                                                        new StringContent(JsonConvert.SerializeObject(editProject),
                                                        Encoding.UTF8,
                                                        "application/json"));

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

            ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);

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

    public async Task<RequestResult<List<ProjectItemDto>>> GetAllProjectsWithTasksListAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/api/project/all/{organizationId}", cancellationToken);

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

            ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);

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

    public async Task<RequestResult<ProjectItemDto>> GetProjectByIdAsync(Guid projectId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/api/project/{projectId}", cancellationToken);

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

            ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);

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

    public async Task<RequestResult<ProjectItemDto>> GetProjectWithTasksByIdAsync(Guid projectId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/api/project/{projectId}/tasks", cancellationToken);

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

            ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);

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
