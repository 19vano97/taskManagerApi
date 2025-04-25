using System;
using TaskManagerApi.Enitities;
using TaskManagerApi.Enums;
using TaskManagerApi.Models.Project;

namespace TaskManagerApi.Models.TaskItem;

public class TaskItemDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? Status { get; set; }
    public Guid ReporterId { get; set; } = Guid.Empty;
    public Guid AssigneeId { get; set; } = Guid.Empty;
    public Guid ProjectId { get; set; } = Guid.Empty;
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
}
