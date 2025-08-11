using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TaskManagerApi.Models;
using TaskManagerApi.Models.TaskHistory;
using TaskManagerApi.Services.Interfaces;
using TaskManagerApi.Services.Interfaces.Business;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Services.Implementations.Business;

public class TicketHistoryService : ITicketHistoryService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TicketHistoryService> _logger;

    public TicketHistoryService(IHttpClientFactory httpClientFactory, ILogger<TicketHistoryService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task Write(TicketHistoryDto history)
    {
        var httpClient = _httpClientFactory.CreateClient("taskHistory");
        var data = JsonConvert.SerializeObject(history);
        var response = await httpClient.PostAsync("api/thistory/add", new StringContent(data, Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Unsuccess " + data);
            return;
        }

        _logger.LogInformation(data);
    }

    public async Task<ServiceResult<List<TicketHistoryDto>>> GetHistoryByTaskId(Guid taskId, CancellationToken cancellationToken)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("taskHistory");
            var response = await httpClient.GetFromJsonAsync<List<TicketHistoryDto>>($"api/thistory/info/{taskId}", cancellationToken);

            if (response is null)
            {
                _logger.LogWarning("no reponce");
                return new ServiceResult<List<TicketHistoryDto>>
                {
                    IsSuccess = false,
                    ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
                };
            }

            return new ServiceResult<List<TicketHistoryDto>>
            {
                IsSuccess = true,
                Data = response
            };
        }
        catch (System.Exception er)
        {
            _logger.LogError(er.ToString());
            return new ServiceResult<List<TicketHistoryDto>>
            {
                IsSuccess = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };
        }
    }
}
