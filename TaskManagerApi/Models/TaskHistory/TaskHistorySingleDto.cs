using System;

namespace TaskManagerApi.Models.TaskHistory;

public class TaskHistorySingleDto
{
    public required TaskHistoryObjectHistory Object { get; set; }
    public DateTime CreateDate { get; set; }
}
