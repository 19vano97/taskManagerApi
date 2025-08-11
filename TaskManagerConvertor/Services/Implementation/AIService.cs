using System;
using System.Text;
using Newtonsoft.Json;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Models.AI;
using TaskManagerConvertor.Services.Interfaces;

namespace TaskManagerConvertor.Services.Implementation;

public class AIService : IAIService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAccountHelperService _accountHelperService;
    private readonly IHelperService _helperService;
    private readonly ILogger<AIService> _logger;

    public AIService(IHttpClientFactory httpClientFactory,
                     IAccountHelperService accountHelperService,
                     IHelperService helperService,
                     ILogger<AIService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _accountHelperService = accountHelperService;
        _helperService = helperService;
        _logger = logger;
    }

    public async Task<RequestResult<AiThreadDetailsDto>> CreateNewThreadAsync(IHeaderDictionary headers, AiThreadDetailsDto aiThread, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers,  httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<AiThreadDetailsDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.PostAsync($"/api/ai/thread/create",
                                                  new StringContent(JsonConvert.SerializeObject(aiThread),
                                                  Encoding.UTF8,
                                                  "application/json"));
        httpClient.Dispose();

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out AiThreadDetailsDto? ticket))
            {
                return new RequestResult<AiThreadDetailsDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            return new RequestResult<AiThreadDetailsDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }

        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<AiThreadDetailsDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<AiThreadDetailsDto>> DeleteThreadAsync(IHeaderDictionary headers, Guid threadId, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers,  httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<AiThreadDetailsDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.DeleteAsync($"/api/ai/thread/{threadId}/delete",
                                                 cancellationToken);
        httpClient.Dispose();

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out AiThreadDetailsDto? ticket))
            {
                return new RequestResult<AiThreadDetailsDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            return new RequestResult<AiThreadDetailsDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }

        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<AiThreadDetailsDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<List<AiThreadDetailsDto>>> GetAllThreadsAsync(IHeaderDictionary headers, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers,  httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<List<AiThreadDetailsDto>>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.GetAsync($"/api/ai/thread/all",
                                                 cancellationToken);
        httpClient.Dispose();

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out List<AiThreadDetailsDto>? ticket))
            {
                return new RequestResult<List<AiThreadDetailsDto>>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            return new RequestResult<List<AiThreadDetailsDto>>
            {
                IsSuccess = true,
                Data = ticket
            };
        }

        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<List<AiThreadDetailsDto>>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<List<ChatMessageDto>>> GetChatHistoryByThreadIdAsync(IHeaderDictionary headers, Guid aiThread, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers,  httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<List<ChatMessageDto>>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.GetAsync($"/api/ai/chat/{aiThread}/history",
                                                 cancellationToken);
        httpClient.Dispose();

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out List<ChatMessageDto>? ticket))
            {
                return new RequestResult<List<ChatMessageDto>>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            return new RequestResult<List<ChatMessageDto>>
            {
                IsSuccess = true,
                Data = ticket
            };
        }

        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<List<ChatMessageDto>>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<ChatMessageDto>> ChatWithUserAsync(IHeaderDictionary headers, ChatMessageDto message, Guid threadId, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers,  httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<ChatMessageDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.PostAsync($"/api/ai/chat/{threadId}/message",
                                                  new StringContent(JsonConvert.SerializeObject(message),
                                                  Encoding.UTF8,
                                                  "application/json"));
        httpClient.Dispose();

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out ChatMessageDto? ticket))
            {
                return new RequestResult<ChatMessageDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            return new RequestResult<ChatMessageDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }

        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<ChatMessageDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<AiThreadDetailsDto>> GetThreadInfoAsync(IHeaderDictionary headers, Guid aiThread, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers,  httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<AiThreadDetailsDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.GetAsync($"/api/ai/thread/{aiThread}/info",
                                                 cancellationToken);
        httpClient.Dispose();
        
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out AiThreadDetailsDto? ticket))
            {
                return new RequestResult<AiThreadDetailsDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            return new RequestResult<AiThreadDetailsDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }

        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<AiThreadDetailsDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }
}
