using System;
using TaskManagerApi.Models.TaskHistory;

namespace TaskManagerApi.Handlers;

public class TaskHistoryEventArgs
{
    private TicketHistoryDto _history;

    public TaskHistoryEventArgs(TicketHistoryDto history)
    {
        _history = history;
    }

    public TicketHistoryDto History 
    { 
        get => _history;
    }
}
