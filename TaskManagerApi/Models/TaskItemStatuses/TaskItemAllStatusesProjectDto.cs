using System;

namespace TaskManagerApi.Models.TaskItemStatuses;

public class TaskItemAllStatusesProjectDto
{
    public required Guid ProjectId { get; set; }
    public required List<TaskItemStatusDto> Statuses { get; set; }
}
