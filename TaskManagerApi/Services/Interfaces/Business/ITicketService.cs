using System;
using TaskManagerApi.Enums;
using TaskManagerApi.Handlers;
using TaskManagerApi.Models;
using TaskManagerApi.Models.TicketItem;
using TaskManagerApi.Models.Tickets;

namespace TaskManagerApi.Services.Interfaces.Business;

public interface ITicketService
{
    Task<ServiceResult<List<TicketDto>>> GetTasksByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken);
    Task<ServiceResult<List<TicketDto>>> GetTasksByProjectAsync(Guid projectId, CancellationToken cancellationToken);
    Task<ServiceResult<TicketDto>> CreateTaskAsync(TicketDto newTask, CancellationToken cancellationToken);
    Task<ServiceResult<List<TicketDto>>> CreateTicketsForAiAsync(TicketForAiDto[] newTasks, CancellationToken cancellationToken);
    Task<ServiceResult<TicketDto>> EditTaskByIdAsync(Guid Id, TicketDto newTask, CancellationToken cancellationToken, bool saveToDb = true);
    Task<ServiceResult<TicketDto>> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken);
    Task<ServiceResult<TicketDto>> DeleteTaskAsync(Guid Id, CancellationToken cancellationToken);
    Task<ServiceResult<bool>> PostNewComment(TicketCommentDto comment, CancellationToken cancellationToken);
    Task<ServiceResult<bool>> EditComment(TicketCommentDto comment, CancellationToken cancellationToken);
    Task<ServiceResult<bool>> DeleteComment(Guid commentId, CancellationToken cancellationToken);
    Task<ServiceResult<List<TicketCommentDto>>> GetCommentsByTicketId(Guid ticketId, CancellationToken cancellationToken);
    public event EventHandler<TicketHistoryEventArgs> TaskHistoryEventArgs;
}
