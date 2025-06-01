using System;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities.Task;
using TaskManagerApi.Enums;
using TaskManagerApi.Handlers;
using TaskManagerApi.Models.TaskHistory;
using TaskManagerApi.Models.TaskItem;
using TaskManagerApi.Services.Interfaces;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Services.Implementations;

public class TaskItemService : ITaskItemService
{
    private readonly TaskManagerAPIDbContext _context;
    private readonly ILogger<TaskItemService> _logger;
    private EventHandler<TaskHistoryEventArgs> _taskHistoryEvent;
    private TaskHistoryHandler _taskHistoryHandler;
    private ITaskHistoryService _taskHistoryService;

    public TaskItemService(TaskManagerAPIDbContext context, 
                           ILogger<TaskItemService> logger, 
                           ITaskHistoryService taskHistoryService)
    {
        _context = context;
        _logger = logger;
        _taskHistoryService = taskHistoryService;
        _taskHistoryHandler = new TaskHistoryHandler(_taskHistoryService);
        _taskHistoryHandler.Subscribe(this);
    }

    public event EventHandler<TaskHistoryEventArgs> TaskHistoryEventArgs
    {
        add => _taskHistoryEvent += value;
        remove => _taskHistoryEvent -= value;
    }
    
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
        _context.TaskItems.Update(childCheck);
        await _context.SaveChangesAsync();

        return GeneralService.ConvertTaskToDtoAsync(await GetTaskById(childId));
    }

    public async Task<TaskItemDto> ChangeTaskStatusAsync(Guid taskId, int newStatus)
    {
        var taskToEdit = await GetTaskById(taskId);

        if (taskToEdit is null || !Enum.IsDefined(typeof(TaskStatusEnum), newStatus))
            return null!;

        taskToEdit.StatusId = newStatus;
        taskToEdit.ModifyDate = DateTime.UtcNow;

        _context.TaskItems.Update(taskToEdit);
        await _context.SaveChangesAsync();

        return GeneralService.ConvertTaskToDtoAsync(await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId));
    }

    public async Task<TaskItemDto> CreateTaskAsync(TaskItemDto newTask)
    {
        if (newTask.Title is null)
            return null!;

        var taskToAdd = new TaskItem{
                Id = Guid.NewGuid(),
                Title = newTask.Title,
                Description = newTask.Description,
                StatusId = (int)TaskStatusEnum.ToDo,
                TypeId = (int)TaskTypesEnum.Task,
                ReporterId = (Guid)newTask.ReporterId!,
                ProjectId = (Guid)newTask.ProjectId!,
                ParentId = (Guid)newTask.ParentId,
                AssigneeId = (Guid)newTask.AssigneeId!
        };

        _context.TaskItems.Add(taskToAdd);
        await _context.SaveChangesAsync();

        var result = await GetTaskById(taskToAdd.Id);
        _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TaskHistoryDto
        {
            TaskId = result.Id,
            Author = result.ReporterId,
            EventName = TaskHistoryTypes.TaskCreate.TASK_CREATED
        }));

        _logger.LogInformation($"Task has been created => {result}");

        return GeneralService.ConvertTaskToDtoAsync(result!);
    }

    public async Task<TaskItemDto> DeleteTaskAsync(Guid taskId)
    {
        var taskToDelete = await GetTaskById(taskId);

        if (taskToDelete is null)
            return null!;

        _context.TaskItems.Remove(taskToDelete);
        await _context.SaveChangesAsync();

        return GeneralService.ConvertTaskToDtoAsync(taskToDelete);
    }

    public async Task<TaskItemDto> EditTaskByIdAsync(Guid taskId, TaskItemDto newTask)
    {
        var taskToEdit = await GetTaskById(taskId);

        if (taskToEdit is null)
            return null!;
        
        try
        {
            if (taskToEdit.Title != newTask.Title)
            {
                var oldTitle = taskToEdit.Title;
                taskToEdit.Title = newTask.Title!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TaskHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_TITLE,
                    PreviousState = oldTitle,
                    NewState = taskToEdit.Title
                }));
            }
            
            if (taskToEdit.Description != newTask.Description)
            {
                var oldDescription = taskToEdit.Description;
                taskToEdit.Description = newTask.Description!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TaskHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_DESCRIPTION,
                    PreviousState = oldDescription,
                    NewState = taskToEdit.Description
                }));
            }
    
            if (taskToEdit.ReporterId != newTask.ReporterId)
            {
                var oldReporter = taskToEdit.ReporterId;
                taskToEdit.ReporterId = (Guid)newTask.ReporterId!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TaskHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_REPORTEDID,
                    PreviousState = oldReporter.ToString(),
                    NewState = taskToEdit.ReporterId.ToString()
                }));
            }
    
            if (taskToEdit.AssigneeId != newTask.AssigneeId)
            {
                var oldAssignee= taskToEdit.AssigneeId;
                taskToEdit.AssigneeId = (Guid)newTask.AssigneeId!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TaskHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_ASSIGNEEID,
                    PreviousState = oldAssignee.ToString(),
                    NewState = taskToEdit.AssigneeId.ToString()
                }));
            }  

            if (taskToEdit.ProjectId != newTask.ProjectId)
            {
                var oldProject = taskToEdit.ProjectId;
                taskToEdit.ProjectId = (Guid)newTask.ProjectId!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TaskHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_PROJECT,
                    PreviousState = oldProject.ToString(),
                    NewState = taskToEdit.ProjectId.ToString()
                }));
            }

            if (taskToEdit.ParentId != newTask.ParentId)
            {
                var oldParent = taskToEdit.ParentId;
                taskToEdit.ParentId = (Guid)newTask.ParentId!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TaskHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_PARENTTASK,
                    PreviousState = oldParent.ToString(),
                    NewState = taskToEdit.ParentId.ToString()
                }));
            }

            if (taskToEdit.StatusId != newTask.StatusId)
            {
                var oldstatus = taskToEdit.StatusId;
                taskToEdit.StatusId = newTask.StatusId!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TaskHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_STATUS,
                    PreviousState = oldstatus.ToString(),
                    NewState = taskToEdit.ProjectId.ToString()
                }));
            }

            if (taskToEdit.TypeId != newTask.Type)
            {
                var oldType = taskToEdit.TypeId;
                taskToEdit.TypeId = newTask.Type!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TaskHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_TASKTYPE,
                    PreviousState = oldType.ToString(),
                    NewState = taskToEdit.ProjectId.ToString()
                }));
            }
    
            taskToEdit.ModifyDate = DateTime.UtcNow;
    
            _context.TaskItems.Update(taskToEdit);
            await _context.SaveChangesAsync();
        }
        catch (System.Exception ex)
        {
            _logger.LogCritical($"Edit task => {ex}");
        }

        var result = await GetTaskById(taskToEdit.Id);

        return GeneralService.ConvertTaskToDtoAsync(result!);
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
        return await _context.TaskItems.Include(t => t.TaskItemStatus)
                                      .Include(t => t.TaskType)
                                      .Where(t => t.ProjectItem.OrganizationId == organizationId)
                                      .Select(t => GeneralService.ConvertTaskToDtoAsync(t))
                                      .ToListAsync();
    }

    public async Task<List<TaskItemDto>> GetTasksByProjectAsync(Guid projectId)
    {
        return await _context.TaskItems.Include(t => t.TaskItemStatus)
                                      .Include(t => t.TaskType)
                                      .Where(t => t.ProjectId == projectId)
                                      .Select(t => GeneralService.ConvertTaskToDtoAsync(t))
                                      .ToListAsync();
    }

    private async Task<TaskItem> GetTaskById(Guid taskId)
    {
        return await _context.TaskItems.Include(t => t.TaskItemStatus)
                                      .Include(t => t.TaskType)
                                      .FirstOrDefaultAsync(t => t.Id == taskId);
    }
}
