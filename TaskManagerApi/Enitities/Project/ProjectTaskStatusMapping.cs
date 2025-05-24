using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagerApi.Enitities.Task;

namespace TaskManagerApi.Enitities.Project;

public class ProjectTaskStatusMapping
{
    [Key]
    public int Id { get; set; }
    public required Guid ProjectId { get; set; }
    public required int StatusId { get; set; }
    public required int Order { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
    [ForeignKey("ProjectId")]
    public ProjectItem ProjectItem { get; set; }
    [ForeignKey("StatusId")]
    public TaskItemStatus TaskItemStatus { get; set; }
}
