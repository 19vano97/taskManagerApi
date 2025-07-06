using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerApi.Enitities.Task;

public class TicketStatus
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required int StatusTypeId { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
    [ForeignKey("StatusTypeId")]
    public TicketStatusType TicketStatusType { get; set; }
}
