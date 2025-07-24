using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagerApi.Enitities.Task;

namespace TaskManagerApi.Enitities.Tickets;

public class TicketComment
{
    [Key]
    public Guid Id { get; set; }
    public required Guid TicketId { get; set; }
    public required Guid AccountId { get; set; }
    public required string Message { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime ModifyDate { get; set; }
    [ForeignKey("TicketId")]
    public Ticket Ticket { get; set; }
}
