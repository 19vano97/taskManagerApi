using System;

namespace TaskManagerApi.Models.TaskItemStatuses;

public class TaskItemStatusDto
{
    public required int TypeId { get; set; }
    public required string TypeName { get; set; }
    public required int StatusId { get; set; }
    public required string StatusName { get; set; }
    public required int Order { get; set; }
}
