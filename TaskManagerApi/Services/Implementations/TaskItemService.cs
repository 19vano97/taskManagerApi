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
    public async Task<TaskItemOutputDto> ChangeAssigneeAsync(Guid taskId, Guid newAssignee)
    {
        var taskToEdit = await context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId);

        if (taskToEdit is null || !Guid.TryParse(newAssignee.ToString(), out var checkedAssignee))
            return null;

        taskToEdit.AssigneeId = newAssignee;
        taskToEdit.ModifyDate = DateTime.UtcNow;

        context.TaskItems.Update(taskToEdit);
        await context.SaveChangesAsync();

        return await ConvertTaskToOutputAsync(await context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId), context);
    }

    public async Task<TaskItemOutputDto> ChangeProjectAsync(Guid taskId, Guid newProject)
    {
        var taskToEdit = await context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId);

        if (taskToEdit is null || !Guid.TryParse(newProject.ToString(), out var checkedAssignee))
            return null;

        taskToEdit.ProjectId = newProject;
        taskToEdit.ModifyDate = DateTime.UtcNow;

        context.TaskItems.Update(taskToEdit);
        await context.SaveChangesAsync();

        return await ConvertTaskToOutputAsync(await context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId), context);
    }

    public async Task<TaskItemOutputDto> ChangeTaskStatusAsync(Guid taskId, int newStatus)
    {
        var taskToEdit = await context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId);

        if (taskToEdit is null || !Enum.IsDefined(typeof(TaskItemStatusEnum), newStatus))
            return null;

        taskToEdit.StatusId = newStatus;
        taskToEdit.ModifyDate = DateTime.UtcNow;

        context.TaskItems.Update(taskToEdit);
        await context.SaveChangesAsync();

        return await ConvertTaskToOutputAsync(await context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId), context);
    }

    public async Task<TaskItemOutputDto> CreateTaskAsync(TaskItemInputDto newTask)
    {
        if (newTask.Title is null)
            return null;

        var taskToAdd = new TaskItem{
                Id = Guid.NewGuid(),
                Title = newTask.Title,
                Description = newTask.Description,
                ReporterId = newTask.ReporterId,
                ProjectId = newTask.ProjectId,
                AssigneeId = newTask.AssigneeId
            };

        context.TaskItems.Add(taskToAdd);
        await context.SaveChangesAsync();

        var result = await context.TaskItems.FirstOrDefaultAsync(id => id.Id == taskToAdd.Id);

        return await ConvertTaskToOutputAsync(result!, context);
    }

    public async Task<TaskItemOutputDto> DeleteTaskAsync(Guid Id)
    {
        var taskToDelete = await context.TaskItems.FirstOrDefaultAsync(t => t.Id == Id);

        if (taskToDelete is null)
            return null;

        context.TaskItems.Remove(taskToDelete);
        await context.SaveChangesAsync();

        return await ConvertTaskToOutputAsync(taskToDelete, context);
    }

    public async Task<TaskItemOutputDto> EditTaskByIdAsync(Guid taskId, TaskItemInputDto newTask)
    {
        var taskToEdit = await context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId);

        if (taskToEdit is null)
            return null;
        
        if (newTask.Title is not null)
            taskToEdit.Title = newTask.Title;
        
        if (newTask.Description is not null)
            taskToEdit.Description = newTask.Description;

        if (newTask.ReporterId is not null)
            taskToEdit.ReporterId = newTask.ReporterId;

        if (newTask.AssigneeId is not null)
            taskToEdit.AssigneeId = newTask.AssigneeId;

        if (newTask.Description is not null)
            taskToEdit.ProjectId = newTask.ProjectId;

        taskToEdit.ModifyDate = DateTime.UtcNow;

        context.TaskItems.Update(taskToEdit);
        await context.SaveChangesAsync();

        return await ConvertTaskToOutputAsync(await context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId), context);
    }

    public async Task<TaskItemOutputDto> GetTaskByIdAsync(Guid taskId)
    {
        var findTask = await context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId);

        if (findTask is null)
            return null;
        
        return await ConvertTaskToOutputAsync(findTask, context);
    }

    public async Task<List<TaskItemOutputDto>> GetTasksAsync()
    {
        return await context.TaskItems.Select(t => new TaskItemOutputDto{
            Id = t.Id,
            Title = t!.Title,
            Description = t.Description,
            Status = (TaskItemStatusEnum)t.StatusId,
            Project = context.ProjectItems.Select((p) => (new ProjectItemDto{
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                CreateDate = p.CreateDate
            })).FirstOrDefault(p => p.Id == t.ProjectId),
            ReporterId = t.ReporterId,
            AssigneerId = t.AssigneeId,
            CreateDate = t.CreateDate,
            ModifyDate = t.ModifyDate
        }).ToListAsync();
    }

    private static async Task<TaskItemOutputDto> ConvertTaskToOutputAsync(TaskItem task, TaskManagerAPIDbContext context)
    {
        return new TaskItemOutputDto{
            Id = task.Id,
            Title = task!.Title,
            Description = task.Description,
            Status = (TaskItemStatusEnum)task.StatusId,
            Project = await context.ProjectItems.Select((p) => (new ProjectItemDto{
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                CreateDate = p.CreateDate
            })).FirstOrDefaultAsync(p => p.Id == task.ProjectId),
            ReporterId = task.ReporterId,
            AssigneerId = task.AssigneeId,
            CreateDate = task.CreateDate,
            ModifyDate = task.ModifyDate
        };
    }
}
