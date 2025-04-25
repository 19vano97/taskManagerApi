using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerApi.Enitities;

public class TaskItem
{
    [Key]
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public int StatusId { get; set; } = 1;
    public Guid ReporterId { get; set; } = Guid.Empty;
    public Guid AssigneeId { get; set; } = Guid.Empty;
    public Guid ProjectId { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
    [ForeignKey("ProjectId")]
    public ProjectItem? ProjectItem { get; set; }
}
