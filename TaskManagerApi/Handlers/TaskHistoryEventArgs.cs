using System;
using TaskManagerApi.Models.TaskHistory;

namespace TaskManagerApi.Handlers;

public class TaskHistoryEventArgs
{
    private TaskHistoryDto _history;

    public TaskHistoryEventArgs(TaskHistoryDto history)
    {
        _history = history;
    }

    public TaskHistoryDto History 
    { 
        get => _history;
    }
}
