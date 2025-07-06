using System;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities.Task;
using TaskManagerApi.Enums;
using TaskManagerApi.Handlers;
using TaskManagerApi.Models.TaskHistory;
using TaskManagerApi.Models.TaskItem;
using TaskManagerApi.Models.Tickets;
using TaskManagerApi.Services.Interfaces;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Services.Implementations;

public class TicketService : ITicketService
{
    private readonly TaskManagerAPIDbContext _context;
    private readonly ILogger<TicketService> _logger;
    private EventHandler<TaskHistoryEventArgs> _taskHistoryEvent;
    private TaskHistoryHandler _taskHistoryHandler;
    private ITicketHistoryService _taskHistoryService;

    public TicketService(TaskManagerAPIDbContext context,
                           ILogger<TicketService> logger,
                           ITicketHistoryService taskHistoryService)
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

    public async Task<TicketDto> CreateTaskAsync(TicketDto newTask)
    {
        if (newTask.Title is null)
            return null!;

        var taskToAdd = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = newTask.Title,
            Description = newTask.Description,
            StatusId = (int)TaskStatusEnum.ToDo,
            TypeId = newTask.TypeId == null ? (int)TaskTypesEnum.Task : newTask.TypeId,
            ReporterId = newTask.ReporterId,
            ProjectId = (Guid)newTask.ProjectId,
            ParentId = newTask.ParentId,
            AssigneeId = newTask.AssigneeId,
            StartDate = newTask.StartDate,
            DueDate = newTask.DueDate,
            Estimate = newTask.Estimate,
            SpentTime = newTask.SpentTime
        };

        _context.Tickets.Add(taskToAdd);
        await _context.SaveChangesAsync();

        var result = await GetTaskByIdDto(taskToAdd.Id);
        _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TicketHistoryDto
        {
            TaskId = (Guid)result.Id!,
            Author = (Guid)result.ReporterId!,
            EventName = TaskHistoryTypes.TaskCreate.TASK_CREATED
        }));

        _logger.LogInformation($"Task has been created => {result}");

        return result;
    }

    public async Task<TicketDto> DeleteTaskAsync(Guid taskId)
    {
        var taskToDelete = await GetTaskById(taskId);

        if (taskToDelete is null)
            return null!;

        _context.Tickets.Remove(taskToDelete);
        await _context.SaveChangesAsync();

        return GeneralService.ConvertTaskToDto(taskToDelete);
    }

    public async Task<TicketDto> EditTaskByIdAsync(Guid taskId, TicketDto newTask)
    {
        var taskToEdit = await GetTaskById(taskId);

        if (taskToEdit is null)
            return null!;

        try
        {
            if (taskToEdit.Title != newTask.Title && newTask.Title != null)
            {
                var oldTitle = taskToEdit.Title;
                taskToEdit.Title = newTask.Title!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = (Guid)taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_TITLE,
                    PreviousState = oldTitle,
                    NewState = taskToEdit.Title
                }));
            }

            if (taskToEdit.Description != newTask.Description && newTask.Description != null)
            {
                var oldDescription = taskToEdit.Description;
                taskToEdit.Description = newTask.Description!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = (Guid)taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_DESCRIPTION,
                    PreviousState = oldDescription,
                    NewState = taskToEdit.Description
                }));
            }

            if (taskToEdit.ReporterId != newTask.ReporterId && newTask.ReporterId != null)
            {
                var oldReporter = taskToEdit.ReporterId;
                taskToEdit.ReporterId = (Guid)newTask.ReporterId!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = (Guid)taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_REPORTEDID,
                    PreviousState = oldReporter.ToString(),
                    NewState = taskToEdit.ReporterId.ToString()
                }));
            }

            if (taskToEdit.AssigneeId != newTask.AssigneeId && newTask.AssigneeId != null)
            {
                var oldAssignee = taskToEdit.AssigneeId;
                taskToEdit.AssigneeId = (Guid)newTask.AssigneeId!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = (Guid)taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_ASSIGNEEID,
                    PreviousState = oldAssignee.ToString(),
                    NewState = taskToEdit.AssigneeId.ToString()
                }));
            }

            if (taskToEdit.ProjectId != newTask.ProjectId && newTask.ProjectId != null)
            {
                var oldProject = taskToEdit.ProjectId;
                taskToEdit.ProjectId = (Guid)newTask.ProjectId!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = (Guid)taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_PROJECT,
                    PreviousState = oldProject.ToString(),
                    NewState = taskToEdit.ProjectId.ToString()
                }));
            }

            if (taskToEdit.ParentId != newTask.ParentId && newTask.ParentId != null)
            {
                var oldParent = taskToEdit.ParentId;
                taskToEdit.ParentId = (Guid)newTask.ParentId!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = (Guid)taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_PARENTTASK,
                    PreviousState = oldParent.ToString(),
                    NewState = taskToEdit.ParentId.ToString()
                }));
            }

            if (taskToEdit.StatusId != newTask.StatusId && newTask.StatusId != 0)
            {
                var oldstatus = taskToEdit.StatusId;
                taskToEdit.StatusId = newTask.StatusId!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = (Guid)taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_STATUS,
                    PreviousState = oldstatus.ToString(),
                    NewState = taskToEdit.StatusId.ToString()
                }));
            }

            if (taskToEdit.TypeId != newTask.TypeId && newTask.TypeId != 0 && newTask.TypeId != null)
            {
                var oldType = taskToEdit.TypeId;
                taskToEdit.TypeId = newTask.TypeId!;
                _taskHistoryEvent?.Invoke(this, new TaskHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Id,
                    Author = (Guid)taskToEdit.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_TASKTYPE,
                    PreviousState = oldType.ToString(),
                    NewState = taskToEdit.TypeId.ToString()
                }));
            }

            taskToEdit.ModifyDate = DateTime.UtcNow;

            _context.Tickets.Update(taskToEdit);
            await _context.SaveChangesAsync();
        }
        catch (System.Exception ex)
        {
            _logger.LogCritical($"Edit task => {ex}");
        }

        var result = await GetTaskByIdDto(taskToEdit.Id);

        return result!;
    }

    public async Task<TicketDto> GetTaskByIdAsync(Guid taskId)
    {
        var findTask = await GetTaskByIdDto(taskId);

        if (findTask is null)
            return null!;

        return findTask;
    }

    public async Task<List<TicketDto>> GetTasksByOrganizationAsync(Guid organizationId)
    {
        return await _context.Tickets.Include(t => t.TaskItemStatus)
                                     .Include(t => t.TaskType)
                                     .Where(t => t.ProjectItem.OrganizationId == organizationId)
                                     .Select(t => GeneralService.ConvertTaskToDto(t))
                                     .ToListAsync();
    }

    public async Task<List<TicketDto>> GetTasksByProjectAsync(Guid projectId)
    {
        return await _context.Tickets.Include(t => t.TaskItemStatus)
                                     .Include(t => t.TaskType)
                                     .Where(t => t.ProjectId == projectId)
                                     .Select(t => GeneralService.ConvertTaskToDto(t))
                                     .ToListAsync();
    }

    public async Task<List<TicketDto>> CreateTicketsForAiAsync(TicketForAiDto[] newTasks)
    {
        var createdTasks = new List<(TicketDto ticket, string parentName)>();

        foreach (var task in newTasks)
        {
            var newTask = await CreateTaskAsync(new TicketDto
            {
                Title = task.Title,
                Description = task.Description,
                TypeId = task.TypeId,
                isCreatedByAi = true,
                AssigneeId = task.AssigneeId,
                ReporterId = task.ReporterId,
                ProjectId = task.ProjectId
            });
            createdTasks.Add((newTask, task.ParentName));
        }

        var edit = new List<TicketDto>();

        foreach (var task in createdTasks)
        {
            if (!string.IsNullOrWhiteSpace(task.parentName))
            {
                var parentTask = createdTasks.FirstOrDefault(t =>
                    !string.IsNullOrEmpty(t.ticket.Title) &&
                    t.ticket.Title.Trim().Equals(task.parentName.Trim(), StringComparison.OrdinalIgnoreCase));
                if (parentTask.ticket?.Id != null && task.ticket.Id != parentTask.ticket.Id)
                {
                    task.ticket.ParentId = parentTask.ticket.Id;
                    var updated = await EditTaskByIdAsync((Guid)task.ticket.Id!, task.ticket);

                    if (updated != null)
                        edit.Add(updated);
                }
            }
        }

        var finalTickets = new List<TicketDto>();
        foreach (var item in createdTasks)
        {
            var res = await GetTaskByIdAsync((Guid)item.ticket.Id!);
            finalTickets.Add(res);
        }

        return finalTickets;
    }

    private async Task<Ticket> GetTaskById(Guid taskId)
    {
        return await _context.Tickets.Include(t => t.TaskItemStatus)
                                     .Include(t => t.TaskType)
                                     .Include(p => p.ProjectItem)
                                     .FirstOrDefaultAsync(t => t.Id == taskId);
    }

    private async Task<TicketDto> GetTaskByIdDto(Guid taskId)
    {
        var task = await GetTaskById(taskId);
        var childIssues = await GetChildIssues(taskId);
        var res = GeneralService.ConvertTaskToDto(task);
        if (res == null)
            return null;

        res.ChildIssues = childIssues.Count == 0 ? null : childIssues;

        if (childIssues.Count == 0)
            res.ChildIssues = null;

        return res;
    }

    private async Task<List<TicketDto>> GetChildIssues(Guid taskId)
    {
        return await _context.Tickets.Include(t => t.TaskItemStatus)
                                     .Include(t => t.TaskType)
                                     .Where(t => t.ParentId == taskId)
                                     .Select(t => GeneralService.ConvertTaskToDto(t))
                                     .ToListAsync();
    }
}
