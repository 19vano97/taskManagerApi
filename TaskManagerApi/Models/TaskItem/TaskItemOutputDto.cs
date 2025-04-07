using System;
using TaskManagerApi.Enitities;
using TaskManagerApi.Enums;
using TaskManagerApi.Models.Project;

namespace TaskManagerApi.Models.TaskItem;

public class TaskItemOutputDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public TaskItemStatus? Status { get; set; }
    public Guid? ReporterId { get; set; } = null;
    public Guid? AssigneerId { get; set; } = null;
    public ProjectItemDto? Project { get; set; } = null;
    public DateTime CreateDate { get; set; }
    public DateTime ModifyDate { get; set; }
}
