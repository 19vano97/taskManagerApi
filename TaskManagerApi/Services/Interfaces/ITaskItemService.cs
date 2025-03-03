using System;
using TaskManagerApi.Enums;
using TaskManagerApi.Models;
using TaskManagerApi.Models.TaskItem;

namespace TaskManagerApi.Services.Interfaces;

public interface ITaskItemService
{
    Task<List<TaskItemOutputDto>> GetTasksAsync();
    Task<TaskItemOutputDto> CreateTaskAsync(TaskItemInputDto newTask);
    Task<TaskItemOutputDto> EditTaskByIdAsync(Guid Id, TaskItemInputDto newTask);
    Task<TaskItemOutputDto> GetTaskByIdAsync(Guid taskId);
    Task<TaskItemOutputDto> DeleteTaskAsync(Guid Id);
    Task<TaskItemOutputDto> ChangeTaskStatusAsync(Guid taskId, int newStatus);
    Task<TaskItemOutputDto> ChangeAssigneeAsync(Guid taskId, Guid newAssignee);
    Task<TaskItemOutputDto> ChangeProjectAsync(Guid taskId, Guid newProject);
}
