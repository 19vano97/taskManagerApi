using System;
using TaskManagerApi.Enitities;
using TaskManagerApi.Enums;
using TaskManagerApi.Models.Project;

namespace TaskManagerApi.Models.TaskItem;

public class TaskItemDto
{
    public Guid? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? StatusId { get; set; }
    public string? StatusName { get; set; }
    public int? Type { get; set; }
    public string? TypeName { get; set; }
    public Guid? ReporterId { get; set; }
    public Guid? AssigneeId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? ParentId { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
}
