using System;
using TaskManagerApi.Models.TaskHistory;

namespace TaskManagerApi.Services.Interfaces;

public interface ITicketHistoryService
{
    Task Write(TicketHistoryDto history);
    Task<List<TicketHistoryDto>> GetHistoryByTaskId(Guid taskId);
}
