using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities;
using TaskManagerApi.Enums;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Project;
using TaskManagerApi.Models.TaskItem;
using TaskManagerApi.Services.Interfaces;

namespace TaskManagerApi.Services.Implementations;

public class TaskItemService(TaskManagerAPIDbContext context) : ITaskItemService
{
    public async Task<TaskItemDto> AddParentTicket(Guid parentId, Guid childId)
    {
        var parentCheck = await GetTaskById(parentId);

        if (parentCheck is null)
        {
            return null!;
        }

        var childCheck = await GetTaskById(childId);

        if (childCheck is null)
        {
            return null!;
        }

        childCheck.ParentId = parentId;
        context.TaskItems.Update(childCheck);
        await context.SaveChangesAsync();

        return GeneralService.ConvertTaskToDtoAsync(await GetTaskById(childId));
    }

    public async Task<TaskItemDto> ChangeAssigneeAsync(Guid taskId, Guid newAssignee)
    {
        var taskToEdit = await GetTaskById(taskId);

        if (taskToEdit is null || !Guid.TryParse(newAssignee.ToString(), out var checkedAssignee))
            return null;

        taskToEdit.AssigneeId = newAssignee;
        taskToEdit.ModifyDate = DateTime.UtcNow;

        context.TaskItems.Update(taskToEdit);
        await context.SaveChangesAsync();

        return GeneralService.ConvertTaskToDtoAsync(await context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId));
    }

    public async Task<TaskItemDto> ChangeProjectAsync(Guid taskId, Guid newProject)
    {
        var taskToEdit = await GetTaskById(taskId);

        if (taskToEdit is null || !Guid.TryParse(newProject.ToString(), out var checkedAssignee))
            return null;

        taskToEdit.ProjectId = newProject;
        taskToEdit.ModifyDate = DateTime.UtcNow;

        context.TaskItems.Update(taskToEdit);
        await context.SaveChangesAsync();

        return GeneralService.ConvertTaskToDtoAsync(await context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId));
    }

    public async Task<TaskItemDto> ChangeTaskStatusAsync(Guid taskId, int newStatus)
    {
        var taskToEdit = await GetTaskById(taskId);

        if (taskToEdit is null || !Enum.IsDefined(typeof(TaskItemStatusTypesEnum), newStatus))
            return null;

        taskToEdit.StatusId = newStatus;
        taskToEdit.ModifyDate = DateTime.UtcNow;

        context.TaskItems.Update(taskToEdit);
        await context.SaveChangesAsync();

        return GeneralService.ConvertTaskToDtoAsync(await context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId));
    }

    public async Task<TaskItemDto> CreateTaskAsync(TaskItemDto newTask)
    {
        if (newTask.Title is null)
            return null!;

        var taskToAdd = new TaskItem{
                Id = Guid.NewGuid(),
                Title = newTask.Title,
                Description = newTask.Description,
                StatusId = newTask.StatusId,
                TypeId = newTask.Type,
                ReporterId = newTask.ReporterId,
                ProjectId = newTask.ProjectId,
                AssigneeId = newTask.AssigneeId
            };

        context.TaskItems.Add(taskToAdd);
        await context.SaveChangesAsync();

        var result = await GetTaskById(taskToAdd.Id);

        return GeneralService.ConvertTaskToDtoAsync(result!);
    }

    public async Task<TaskItemDto> DeleteTaskAsync(Guid taskId)
    {
        var taskToDelete = await GetTaskById(taskId);

        if (taskToDelete is null)
            return null;

        context.TaskItems.Remove(taskToDelete);
        await context.SaveChangesAsync();

        return GeneralService.ConvertTaskToDtoAsync(taskToDelete);
    }

    public async Task<TaskItemDto> EditTaskByIdAsync(Guid taskId, TaskItemDto newTask)
    {
        var taskToEdit = await GetTaskById(taskId);

        if (taskToEdit is null)
            return null;
        
        if (newTask.Title != string.Empty)
            taskToEdit.Title = newTask.Title;
        
        if (newTask.Description != string.Empty)
            taskToEdit.Description = newTask.Description;

        if (newTask.ReporterId != Guid.Empty)
            taskToEdit.ReporterId = newTask.ReporterId;

        if (newTask.AssigneeId != Guid.Empty)
            taskToEdit.AssigneeId = newTask.AssigneeId;

        if (newTask.Description != string.Empty)
            taskToEdit.ProjectId = newTask.ProjectId;

        taskToEdit.ModifyDate = DateTime.UtcNow;

        context.TaskItems.Update(taskToEdit);
        await context.SaveChangesAsync();

        return GeneralService.ConvertTaskToDtoAsync(await context.TaskItems.FirstAsync(t => t.Id == taskId));
    }

    public async Task<TaskItemDto> GetTaskByIdAsync(Guid taskId)
    {
        var findTask = await GetTaskById(taskId);

        if (findTask is null)
            return null!;
        
        return GeneralService.ConvertTaskToDtoAsync(findTask);
    }

    public async Task<List<TaskItemDto>> GetTasksByOrganizationAsync(Guid organizationId)
    {
        return await context.TaskItems.Where(t => t.ProjectItem.OrganizationId == organizationId).Select(t => GeneralService.ConvertTaskToDtoAsync(t)).ToListAsync();
    }

    public async Task<List<TaskItemDto>> GetTasksByProjectAsync(Guid projectId)
    {
        return await context.TaskItems.Where(t => t.ProjectId == projectId).Select(t => GeneralService.ConvertTaskToDtoAsync(t)).ToListAsync();
    }

    private async Task<TaskItem> GetTaskById(Guid taskId)
    {
        return await context.TaskItems.Include(t => t.TaskItemStatus)
                                              .Include(t => t.TaskType)
                                              .FirstOrDefaultAsync(t => t.Id == taskId);
    }
}
