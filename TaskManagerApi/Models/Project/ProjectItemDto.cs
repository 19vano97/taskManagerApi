using System;
using TaskManagerApi.Models.TaskItemStatuses;

namespace TaskManagerApi.Models.Project;

public class ProjectItemDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid OwnerId { get; set; } = Guid.Empty;
    public Guid OrganizationId { get; set; } = Guid.Empty;
    public List<TicketStatusDto> Statuses { get; set; } = new List<TicketStatusDto>();
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModidyDate { get; set; } = DateTime.UtcNow;
}
