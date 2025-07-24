using System;

namespace TaskManagerApi.Models.Tickets;

public class TicketCommentDto
{
    public Guid? Id { get; set; }
    public required Guid TaskId { get; set; }
    public required Guid AccountId { get; set; }
    public required string Message { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}
