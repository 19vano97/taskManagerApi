using System;
using TaskManagerApi.Enums;
using TaskManagerApi.Handlers;
using TaskManagerApi.Models;
using TaskManagerApi.Models.TicketItem;
using TaskManagerApi.Models.Tickets;

namespace TaskManagerApi.Services.Interfaces;

public interface ITicketService
{
    Task<ServiceResult<List<TicketDto>>> GetTasksByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken);
    Task<ServiceResult<List<TicketDto>>> GetTasksByProjectAsync(Guid projectId, CancellationToken cancellationToken);
    Task<ServiceResult<TicketDto>> CreateTaskAsync(TicketDto newTask, CancellationToken cancellationToken);
    Task<ServiceResult<List<TicketDto>>> CreateTicketsForAiAsync(TicketForAiDto[] newTasks, CancellationToken cancellationToken);
    Task<ServiceResult<TicketDto>> EditTaskByIdAsync(Guid Id, TicketDto newTask, CancellationToken cancellationToken, bool saveToDb = true);
    Task<ServiceResult<TicketDto>> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken);
    Task<ServiceResult<TicketDto>> DeleteTaskAsync(Guid Id, CancellationToken cancellationToken);
    public event EventHandler<TicketHistoryEventArgs> TaskHistoryEventArgs;
}
