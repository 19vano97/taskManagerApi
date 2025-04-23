using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerApi.Enitities;

public class ProjectItem
{
    [Key]
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? OwnerId { get; set; } = Guid.Empty;
    public Guid OrganizationId { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
    public TaskItem? TaskItem { get; set; }
    [ForeignKey("OrganizationId")]
    public OrganizationItem? Organization { get; set; }
    public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
}
