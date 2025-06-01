using System;
using TaskHistoryService.Models;

namespace TaskHistoryService.Services.Interfaces;

public interface IHistoryService
{
    Task Write(TaskHistoryDto history);
    void WriteNoSaveToDb(TaskHistoryDto history);
    Task<List<TaskHistoryDto>> GetHistoryByTaskId(Guid taskId);
}
