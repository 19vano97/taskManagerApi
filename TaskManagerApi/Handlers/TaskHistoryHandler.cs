using System;
using TaskManagerApi.Models.TaskHistory;
using TaskManagerApi.Services.Interfaces;

namespace TaskManagerApi.Handlers;

public class TaskHistoryHandler
{
    private ITicketHistoryService _history;

    public TaskHistoryHandler(ITicketHistoryService history)
    {
        _history = history;
    }

    public void Subscribe(ITicketService taskItemService)
    {
        taskItemService.TaskHistoryEventArgs += HandlerLoggerEvent;
    }

    public void Unsubscribe(ITicketService taskItemService)
    {
        taskItemService.TaskHistoryEventArgs -= HandlerLoggerEvent;
    }

    private void HandlerLoggerEvent(object? sender, TaskHistoryEventArgs e)
    {
        _ = _history.Write(e.History)
            .ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    // Log or handle the error
                    Console.Error.WriteLine(task.Exception);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
    }
}
