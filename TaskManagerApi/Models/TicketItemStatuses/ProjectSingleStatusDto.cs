using System;

namespace TaskManagerApi.Models.TicketItemStatuses;

public class ProjectSingleStatusDto
{
    public required Guid ProjectId { get; set; }
    public required TicketStatusDto Status { get; set; }
}
