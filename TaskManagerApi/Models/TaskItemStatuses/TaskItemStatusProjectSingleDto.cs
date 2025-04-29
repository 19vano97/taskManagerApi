using System;

namespace TaskManagerApi.Models.TaskItemStatuses;

public class TaskItemStatusProjectSingleDto
{
    public required Guid ProjectId { get; set; }
    public required TaskItemStatusDto Status { get; set; }
}
