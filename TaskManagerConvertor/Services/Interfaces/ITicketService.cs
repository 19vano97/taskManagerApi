using System;
using System.Runtime.CompilerServices;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Models.Ticket;
using TaskManagerConvertor.Models.TicketItem;

namespace TaskManagerConvertor.Services.Interfaces;

public interface ITicketService
{
    public Task<RequestResult<TicketDto>> GetTicketById(Guid Id, CancellationToken cancellationToken);
    public Task<RequestResult<TicketDto>> CreateTicketAsync(TicketDto ticketDto, CancellationToken cancellationToken);
    public Task<RequestResult<List<TicketDto>>> GetTasksAsync(Guid id, bool isProject, CancellationToken cancellationToken);
    public Task<RequestResult<List<TicketDto>>> CreateTaskForAiAsync(TicketForAiDto[] newTasks, CancellationToken cancellationToken);
    public Task<RequestResult<TicketDto>> EditTaskByIdAsync(TicketDto ticketDto, CancellationToken cancellationToken);
    public Task<RequestResult<List<TicketHistoryDto>>> GetTicketHistory(Guid ticketId, CancellationToken cancellationToken);
    public Task<RequestResult<bool>> PostNewComment(Guid ticketId, TicketCommentDto comment, CancellationToken cancellationToken);
    public Task<RequestResult<bool>> EditComment(Guid ticketId, Guid commentId, TicketCommentDto comment, CancellationToken cancellationToken);
    public Task<RequestResult<bool>> DeleteComment(Guid ticketId, Guid commentId, CancellationToken cancellationToken);
    public Task<RequestResult<List<TicketCommentDto>>> GetCommentsByTicketId(Guid ticketId, CancellationToken cancellationToken);
    public Task<RequestResult<bool>> DeleteTaskByIdAsync(Guid ticketId, CancellationToken cancellationToken);
}
