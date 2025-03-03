using System;

namespace TaskManagerApi.Models.TaskItem;

public class TaskItemInputDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Guid? ReporterId { get; set; } = null;
    public Guid? AssigneeId { get; set; } = null;
    public Guid? ProjectId { get; set; } = null;
}
