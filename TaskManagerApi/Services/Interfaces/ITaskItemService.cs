using System;
using TaskManagerApi.Enums;
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
    Task<TaskItemDto> ChangeTaskStatusAsync(Guid taskId, int newStatus);
    Task<TaskItemDto> ChangeAssigneeAsync(Guid taskId, Guid newAssignee);
    Task<TaskItemDto> ChangeProjectAsync(Guid taskId, Guid newProject);
    Task<TaskItemDto> AddParentTicket(Guid parentId, Guid childId);
}
