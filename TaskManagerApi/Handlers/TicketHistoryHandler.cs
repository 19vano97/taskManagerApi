using System;
using TaskManagerApi.Models.TaskHistory;
using TaskManagerApi.Services.Interfaces;
using TaskManagerApi.Services.Interfaces.Business;

namespace TaskManagerApi.Handlers;

public class TicketHistoryHandler
{
    private ITicketHistoryService _history;

    public TicketHistoryHandler(ITicketHistoryService history)
    {
        _history = history;
    }

    public void Subscribe(ITicketService ticketItemService)
    {
        ticketItemService.TaskHistoryEventArgs += HandlerLoggerEvent;
    }

    public void Unsubscribe(ITicketService tickettemService)
    {
        tickettemService.TaskHistoryEventArgs -= HandlerLoggerEvent;
    }

    private void HandlerLoggerEvent(object? sender, TicketHistoryEventArgs e)
    {
        _ = _history.Write(e.History)
            .ContinueWith(task =>
            { }, TaskContinuationOptions.OnlyOnFaulted);
    }
}
