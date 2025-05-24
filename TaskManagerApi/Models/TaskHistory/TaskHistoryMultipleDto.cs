using System;

namespace TaskManagerApi.Models.TaskHistory;

public class TaskHistoryMultipleDto
{
    public required List<TaskHistoryObjectHistory> Objects { get; set; }
    public DateTime CreateDate { get; set; }
}
