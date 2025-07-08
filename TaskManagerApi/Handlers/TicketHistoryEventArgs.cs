using System;
using TaskManagerApi.Models.TaskHistory;

namespace TaskManagerApi.Handlers;

public class TicketHistoryEventArgs
{
    private TicketHistoryDto _history;

    public TicketHistoryEventArgs(TicketHistoryDto history)
    {
        _history = history;
    }

    public TicketHistoryDto History 
    { 
        get => _history;
    }
}
