using System;
using System.Text;
using Newtonsoft.Json;
using TaskManagerApi.Models.TaskHistory;
using TaskManagerApi.Services.Interfaces;

namespace TaskManagerApi.Services.Implementations;

public class TaskHistoryService : ITaskHistoryService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TaskHistoryService> _logger;

    public TaskHistoryService(IHttpClientFactory httpClientFactory, ILogger<TaskHistoryService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task Write(TaskHistoryDto history)
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
}
