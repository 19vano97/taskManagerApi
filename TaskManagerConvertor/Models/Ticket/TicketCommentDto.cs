using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerConvertor.Models.Ticket;

public class TicketCommentDto
{
    public Guid? Id { get; set; }
    public required Guid TicketId { get; set; }
    public required Guid AccountId { get; set; }
    public AccountDto? Account { get; set; }
    [StringLength(2000)]
    public required string Message { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}
