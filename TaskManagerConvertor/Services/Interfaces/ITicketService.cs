using System;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Models.Ticket;
using TaskManagerConvertor.Models.TicketItem;

namespace TaskManagerConvertor.Services.Interfaces;

public interface ITicketService
{
    public Task<RequestResult<TicketDto>> GetTicketById(IHeaderDictionary headers, Guid Id, CancellationToken cancellationToken);
    public Task<RequestResult<TicketDto>> CreateTicketAsync(IHeaderDictionary headers, TicketDto ticketDto, CancellationToken cancellationToken);
    public Task<RequestResult<List<TicketDto>>> GetTasksAsync(IHeaderDictionary headers, Guid projectId, CancellationToken cancellationToken);
    public Task<RequestResult<List<TicketDto>>> CreateTaskForAiAsync(IHeaderDictionary headers, TicketForAiDto[] newTasks, CancellationToken cancellationToken);
    public Task<RequestResult<TicketDto>> EditTaskByIdAsync(IHeaderDictionary headers, TicketDto ticketDto, CancellationToken cancellationToken);
    public Task<RequestResult<List<TicketHistoryDto>>> GetTicketHistory(IHeaderDictionary headers, Guid ticketId, CancellationToken cancellationToken);
    public Task<RequestResult<bool>> PostNewComment(IHeaderDictionary headers, Guid ticketId, TicketCommentDto comment, CancellationToken cancellationToken);
    public Task<RequestResult<bool>> EditComment(IHeaderDictionary headers, Guid ticketId, Guid commentId, TicketCommentDto comment, CancellationToken cancellationToken);
    public Task<RequestResult<bool>> DeleteComment(IHeaderDictionary headers, Guid ticketId, Guid commentId, CancellationToken cancellationToken);
    public Task<RequestResult<List<TicketCommentDto>>> GetCommentsByTicketId(IHeaderDictionary headers, Guid ticketId, CancellationToken cancellationToken);
    public Task<RequestResult<bool>> DeleteTaskByIdAsync(IHeaderDictionary headers, Guid ticketId, CancellationToken cancellationToken);
}
