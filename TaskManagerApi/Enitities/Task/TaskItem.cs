using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagerApi.Enitities.Project;

namespace TaskManagerApi.Enitities.Task;

public class TaskItem
{
    [Key]
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? TypeId { get; set; } = 1;
    public int? StatusId { get; set; } = 2;
    public Guid ReporterId { get; set; } = Guid.Empty;
    public Guid AssigneeId { get; set; } = Guid.Empty;
    public Guid ParentId { get; set; } = Guid.Empty;
    public required Guid ProjectId { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
    [ForeignKey("ProjectId")]
    public ProjectItem? ProjectItem { get; set; }
    [ForeignKey("StatusId")]
    public TaskItemStatus? TaskItemStatus { get; set; }
    [ForeignKey("TypeId")]
    public TaskType? TaskType { get; set; }
}
