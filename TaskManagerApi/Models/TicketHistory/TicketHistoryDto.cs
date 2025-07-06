using System;

namespace TaskManagerApi.Models.TaskHistory;

public class TicketHistoryDto
{
    public Guid Id { get; set; }
    public required Guid TaskId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string PreviousState { get; set; } = string.Empty;
    public string NewState { get; set; } = string.Empty;
    public required Guid Author { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
}
