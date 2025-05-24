using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagerApi.Enitities.Organization;
using TaskManagerApi.Enitities.Task;

namespace TaskManagerApi.Enitities.Project;

public class ProjectItem
{
    [Key]
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid OwnerId { get; set; } = Guid.Empty;
    public Guid OrganizationId { get; set; } = Guid.Empty;
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
    public TaskItem? TaskItem { get; set; }
    [ForeignKey("OrganizationId")]
    public OrganizationItem Organization { get; set; }
    public virtual ICollection<ProjectTaskStatusMapping> ProjectItems { get; set; } = new List<ProjectTaskStatusMapping>();

}
