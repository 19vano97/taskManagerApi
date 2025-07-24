using System;

namespace TaskHistoryService.Models;

public class TaskHistoryDto
{
    public Guid Id { get; set; }
    public required Guid TaskId { get; set; }
    public string? EventName { get; set; }
    public string? PreviousState { get; set; }
    public string? NewState { get; set; }
    public required Guid Author { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
}
