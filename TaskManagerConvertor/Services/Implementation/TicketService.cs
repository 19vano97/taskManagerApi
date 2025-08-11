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
    private readonly HttpClient _httpClient;
    private readonly IAccountHelperService _accountHelperService;
    private readonly IHelperService _helperService;
    private readonly ILogger<TicketService> _logger;

    public TicketService(IHttpClientFactory httpClientFactory,
                         IAccountHelperService accountHelperService,
                         IHelperService helperService,
                         ILogger<TicketService> logger)
    {
        _httpClient = httpClientFactory.CreateClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT);
        _accountHelperService = accountHelperService;
        _helperService = helperService;
        _logger = logger;
    }

    public async Task<RequestResult<TicketDto>> CreateTicketAsync(TicketDto ticketDto,
                                                             CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsync(Constants.TaskManagerApi.Ticket.POST_CREATE_TICKET,
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

            ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);

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

    public async Task<RequestResult<TicketDto>> GetTicketById(Guid Id, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/api/task/{Id}/details", cancellationToken);

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

            ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);

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

    public async Task<RequestResult<List<TicketDto>>> GetTasksAsync(Guid id, bool isProject, CancellationToken cancellationToken)
    {
        HttpResponseMessage? response;
         
        if (!isProject)
        {
            response = await _httpClient.GetAsync($"/api/task/all/{id}/organization"
                                                        , cancellationToken);
        }
        else
        {
            response = await _httpClient.GetAsync($"/api/task/all/{id}/project"
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
                tickets = await _accountHelperService.AddAccountDetails(tickets, cancellationToken);

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

    public async Task<RequestResult<List<TicketDto>>> CreateTaskForAiAsync(TicketForAiDto[] newTasks, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsync(Constants.TaskManagerApi.Ticket.POST_CREATE_TICKETS_FOR_AI,
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
                tickets = await _accountHelperService.AddAccountDetails(tickets, cancellationToken);

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

    public async Task<RequestResult<TicketDto>> EditTaskByIdAsync(TicketDto ticketDto, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsync($"/api/task/{ticketDto.Id}/edit",
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

            ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);

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

    public async Task<RequestResult<List<TicketHistoryDto>>> GetTicketHistory(Guid ticketId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/api/task/{ticketId}/history", cancellationToken);

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
                history = await _accountHelperService.AddAccountDetails(history, cancellationToken);
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

    public async Task<RequestResult<bool>> DeleteTaskByIdAsync(Guid ticketId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.DeleteAsync($"/api/task/{ticketId}/delete", cancellationToken);
        
        if (response.IsSuccessStatusCode)
        {
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

    public async Task<RequestResult<bool>> PostNewComment(Guid ticketId, TicketCommentDto comment, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsync($"api/task/{ticketId}/comment/new",
                                                        new StringContent(JsonConvert.SerializeObject(comment),
                                                        Encoding.UTF8,
                                                        "application/json"));

        if (response.IsSuccessStatusCode)
        {
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

    public async Task<RequestResult<bool>> EditComment(Guid ticketId, Guid commentId, TicketCommentDto comment, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsync($"api/task/{ticketId}/comment/{commentId}",
                                                        new StringContent(JsonConvert.SerializeObject(comment),
                                                        Encoding.UTF8,
                                                        "application/json"));

        if (response.IsSuccessStatusCode)
        {
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

    public async Task<RequestResult<bool>> DeleteComment(Guid ticketId, Guid commentId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.DeleteAsync($"api/task/{ticketId}/comment/{commentId}", cancellationToken);

        if (response.IsSuccessStatusCode)
        {
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

    public async Task<RequestResult<List<TicketCommentDto>>> GetCommentsByTicketId(Guid ticketId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/api/task/{ticketId}/comment/all", cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(await response.Content.ReadAsStringAsync());
            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!_helperService.TryParseJsonToDto(data, out List<TicketCommentDto>? ticket))
            {
                return new RequestResult<List<TicketCommentDto>>
                {
                    IsSuccess = false,
                    ErrorMessage = "Issue with parcing"
                };
            }

            ticket = await _accountHelperService.AddAccountDetails(ticket!, cancellationToken);

            return new RequestResult<List<TicketCommentDto>>
            {
                IsSuccess = true,
                Data = ticket
            };
        }

        _logger.LogWarning("{0} {1}", response.StatusCode, response.ReasonPhrase);

        return new RequestResult<List<TicketCommentDto>>
        {
            IsSuccess = false,
            ErrorMessage = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)
        };
    }
}
