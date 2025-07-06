using System;
using TaskManagerApi.Enums;
using TaskManagerApi.Handlers;
using TaskManagerApi.Models;
using TaskManagerApi.Models.TaskItem;
using TaskManagerApi.Models.Tickets;

namespace TaskManagerApi.Services.Interfaces;

public interface ITicketService
{
    Task<List<TicketDto>> GetTasksByOrganizationAsync(Guid organizationId);
    Task<List<TicketDto>> GetTasksByProjectAsync(Guid projectId);
    Task<TicketDto> CreateTaskAsync(TicketDto newTask);
    Task<List<TicketDto>> CreateTicketsForAiAsync(TicketForAiDto[] newTasks);
    Task<TicketDto> EditTaskByIdAsync(Guid Id, TicketDto newTask);
    Task<TicketDto> GetTaskByIdAsync(Guid taskId);
    Task<TicketDto> DeleteTaskAsync(Guid Id);
    public event EventHandler<TaskHistoryEventArgs> TaskHistoryEventArgs;
}
