using System;
using TaskManagerApi.Models.TaskHistory;

namespace TaskManagerApi.Services.Interfaces;

public interface ITaskHistoryService
{
    Task Write(TaskHistoryDto history);
}
