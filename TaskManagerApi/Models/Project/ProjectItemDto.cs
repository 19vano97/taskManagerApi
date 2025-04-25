using System;

namespace TaskManagerApi.Models.Project;

public class ProjectItemDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid OwnerId { get; set; } = Guid.Empty;
    public Guid OrganizationId { get; set; } = Guid.Empty;
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModidyDate { get; set; } = DateTime.UtcNow;
}
