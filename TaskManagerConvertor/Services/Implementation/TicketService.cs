using System;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Models.Ticket;
using TaskManagerConvertor.Models.TicketItem;
using TaskManagerConvertor.Services.Interfaces;

namespace TaskManagerConvertor.Services.Implementation;

public class TicketService : ITicketService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHelperService _helperService;
    private readonly ILogger<TicketService> _logger;

    public TicketService(IHttpClientFactory httpClientFactory,
                         IHelperService helperService,
                         ILogger<TicketService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _helperService = helperService;
        _logger = logger;
    }

    public async Task<RequestResult<TicketDto>> CreateTicketAsync(IHeaderDictionary headers,
                                                             TicketDto ticketDto,
                                                             CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<TicketDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.PostAsync(Constants.TaskManagerApi.Ticket.POST_CREATE_TICKET,
                                                        new StringContent(JsonConvert.SerializeObject(ticketDto),
                                                        Encoding.UTF8,
                                                        "application/json"));
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out TicketDto? ticket))
            {
                return new RequestResult<TicketDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _helperService.AddAccountDetails(headers, ticket!, cancellationToken);

            return new RequestResult<TicketDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }

        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<TicketDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<TicketDto>> GetTicketById(IHeaderDictionary headers,
                                                              Guid Id,
                                                              CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<TicketDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.GetAsync($"/api/task/{Id}/details", cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());
            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out TicketDto? ticket))
            {
                return new RequestResult<TicketDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _helperService.AddAccountDetails(headers, ticket!, cancellationToken);

            return new RequestResult<TicketDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }

        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<TicketDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<List<TicketDto>>> GetTasksAsync(IHeaderDictionary headers, Guid projectId, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<List<TicketDto>>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = new HttpResponseMessage();

        if (projectId == Guid.Empty)
        {
            await httpClient.GetAsync(string.Format($"/api/task/{0}/organization",
                                                                     httpClient.DefaultRequestHeaders
                                                                        .First(o => o.Key == Constants.Settings.Header.ORGANIZATION).Value)
                                                        , cancellationToken);
        }
        else
        {
            await httpClient.GetAsync(string.Format($"/api/task/{0}/project",
                                                                     httpClient.DefaultRequestHeaders
                                                                        .First(o => o.Key == Constants.Settings.Header.ORGANIZATION).Value)
                                                        , cancellationToken);
        }

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());
            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out List<TicketDto>? tickets))
            {
                return new RequestResult<List<TicketDto>>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            if (tickets is not null && tickets.Count > 0)
                tickets = await _helperService.AddAccountDetails(headers, tickets, cancellationToken);

            return new RequestResult<List<TicketDto>>
            {
                IsSuccess = true,
                Data = tickets
            };
        }

        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<List<TicketDto>>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<List<TicketDto>>> CreateTaskForAiAsync(IHeaderDictionary headers, TicketForAiDto[] newTasks, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<List<TicketDto>>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.PostAsync(Constants.TaskManagerApi.Ticket.POST_CREATE_TICKETS_FOR_AI,
                                                        new StringContent(JsonConvert.SerializeObject(newTasks),
                                                        Encoding.UTF8,
                                                        "application/json"));

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());
            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out List<TicketDto>? tickets))
            {
                return new RequestResult<List<TicketDto>>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            if (tickets is not null && tickets.Count > 0)
                tickets = await _helperService.AddAccountDetails(headers, tickets, cancellationToken);

            return new RequestResult<List<TicketDto>>
            {
                IsSuccess = true,
                Data = tickets
            };
        }

        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<List<TicketDto>>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<TicketDto>> EditTaskByIdAsync(IHeaderDictionary headers, TicketDto ticketDto, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<TicketDto>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.PostAsync($"/api/ticket/{ticketDto.Id}/edit",
                                                  new StringContent(JsonConvert.SerializeObject(ticketDto),
                                                  Encoding.UTF8,
                                                  "application/json"));
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out TicketDto? ticket))
            {
                return new RequestResult<TicketDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _helperService.AddAccountDetails(headers, ticket!, cancellationToken);

            return new RequestResult<TicketDto>
            {
                IsSuccess = true,
                Data = ticket
            };
        }

        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<TicketDto>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<List<TicketHistoryDto>>> GetTicketHistory(IHeaderDictionary headers, Guid ticketId, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<List<TicketHistoryDto>>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.GetAsync($"/api/task/{ticketId}/details", cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());
            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out List<TicketHistoryDto>? history))
            {
                return new RequestResult<List<TicketHistoryDto>>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            if (history is not null && history.Count > 0)
            {
                history = await _helperService.AddAccountDetails(headers, history, cancellationToken);
            }

            return new RequestResult<List<TicketHistoryDto>>
            {
                IsSuccess = true,
                Data = history
            };
        }

        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<List<TicketHistoryDto>>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }

    public async Task<RequestResult<bool>> DeleteTaskByIdAsync(IHeaderDictionary headers, Guid ticketId, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        var httpClientCheck = _helperService.SetupHttpClientForTaskManager(headers, ref httpClient);
        if (!httpClientCheck.IsSuccess)
        {
            return new RequestResult<bool>
            {
                IsSuccess = false,
                ErrorMessage = httpClientCheck.ErrorMessage
            };
        }

        var response = await httpClient.DeleteAsync($"/api/task/{ticketId}/delete", cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            return new RequestResult<bool>
            {
                IsSuccess = true,
                Data = true
            };
        }

        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<bool>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }
}
