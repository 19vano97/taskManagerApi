using System;
using TaskManagerApi.Models.Project;

namespace TaskManagerApi.Models.OrganizationModel;

public class OrganizationProjectDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public required string Name { get; set; }
    public string Abbreviation { get; set; } = string.Empty;
    public Guid Owner { get; set; } = Guid.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
    public List<ProjectItemDto>? Projects { get; set; }
    public List<Guid>? Accounts { get; set; }
}
