using System;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Tickets;

namespace TaskManagerApi.Services.Interfaces;

public interface ITicketCommentService
{
    Task<ServiceResult<TicketCommentDto>> PostNewComment(TicketCommentDto comment);
    Task<ServiceResult<TicketCommentDto>> EditComment(TicketCommentDto comment);
    Task<ServiceResult<bool>> DeleteComment(TicketCommentDto comment);
    Task<ServiceResult<List<TicketCommentDto>>> GetCommentsByTicketId(Guid ticketId);
}
