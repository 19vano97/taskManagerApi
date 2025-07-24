using System;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Tickets;
using TaskManagerApi.Services.Interfaces;

namespace TaskManagerApi.Services.Implementations;

public class TicketCommentService : ITicketCommentService
{
    public Task<ServiceResult<bool>> DeleteComment(TicketCommentDto comment)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<TicketCommentDto>> EditComment(TicketCommentDto comment)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<List<TicketCommentDto>>> GetCommentsByTicketId(Guid ticketId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<TicketCommentDto>> PostNewComment(TicketCommentDto comment)
    {
        throw new NotImplementedException();
    }
}
