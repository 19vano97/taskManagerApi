using System;
using TaskManagerApi.Enums;
using TaskManagerApi.Handlers;
using TaskManagerApi.Models;
using TaskManagerApi.Models.TaskItem;

namespace TaskManagerApi.Services.Interfaces;

public interface ITaskItemService
{
    Task<List<TaskItemDto>> GetTasksByOrganizationAsync(Guid organizationId);
    Task<List<TaskItemDto>> GetTasksByProjectAsync(Guid projectId);
    Task<TaskItemDto> CreateTaskAsync(TaskItemDto newTask);
    Task<TaskItemDto> EditTaskByIdAsync(Guid Id, TaskItemDto newTask);
    Task<TaskItemDto> GetTaskByIdAsync(Guid taskId);
    Task<TaskItemDto> DeleteTaskAsync(Guid Id);
    public event EventHandler<TaskHistoryEventArgs> TaskHistoryEventArgs;
}
