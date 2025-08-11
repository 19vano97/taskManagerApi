using System;
using TaskManagerApi.Models;
using TaskManagerApi.Models.TaskHistory;

namespace TaskManagerApi.Services.Interfaces.Business;

public interface ITicketHistoryService
{
    Task Write(TicketHistoryDto history);
    Task<ServiceResult<List<TicketHistoryDto>>> GetHistoryByTaskId(Guid taskId, CancellationToken cancellationToken);
}
