using System;

namespace TaskManagerApi.Models.TaskHistory;

public class TaskHistoryObjectHistory
{
    public required string Object { get; set; }
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
}
