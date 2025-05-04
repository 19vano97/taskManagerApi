using System;

namespace TaskManagerApi.Models.TaskItemStatuses;

public class ProjectSingleStatusDto
{
    public required Guid ProjectId { get; set; }
    public required TaskItemStatusDto Status { get; set; }
}
