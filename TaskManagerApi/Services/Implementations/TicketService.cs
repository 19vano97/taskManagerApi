using System;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities.Task;
using TaskManagerApi.Enums;
using TaskManagerApi.Handlers;
using TaskManagerApi.Models;
using TaskManagerApi.Models.TaskHistory;
using TaskManagerApi.Models.TicketItem;
using TaskManagerApi.Models.Tickets;
using TaskManagerApi.Services.Interfaces;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Services.Implementations;

public class TicketService : ITicketService
{
    private readonly TicketManagerAPIDbContext _context;
    private readonly ILogger<TicketService> _logger;
    private EventHandler<TicketHistoryEventArgs> _taskHistoryEvent;
    private TicketHistoryHandler _taskHistoryHandler;
    private ITicketHistoryService _taskHistoryService;

    public TicketService(TicketManagerAPIDbContext context,
                           ILogger<TicketService> logger,
                           ITicketHistoryService taskHistoryService)
    {
        _context = context;
        _logger = logger;
        _taskHistoryService = taskHistoryService;
        _taskHistoryHandler = new TicketHistoryHandler(_taskHistoryService);
        _taskHistoryHandler.Subscribe(this);
    }

    public event EventHandler<TicketHistoryEventArgs> TaskHistoryEventArgs
    {
        add => _taskHistoryEvent += value;
        remove => _taskHistoryEvent -= value;
    }

    public async Task<ServiceResult<TicketDto>> CreateTaskAsync(TicketDto newTask, CancellationToken cancellationToken)
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
        await _context.SaveChangesAsync(cancellationToken);

        var result = await GetTaskByIdDto(taskToAdd.Id, cancellationToken);
        if (!result.Success)
            return new ServiceResult<TicketDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.NegativeActions.TASK_CREATION_FAILED_LOG
            };
        _taskHistoryEvent?.Invoke(this, new TicketHistoryEventArgs(new TicketHistoryDto
        {
            TaskId = (Guid)result.Data!.Id!,
            Author = (Guid)result.Data!.ReporterId!,
            EventName = TaskHistoryTypes.TaskCreate.TASK_CREATED
        }));

        _logger.LogInformation($"{LogPhrases.PositiveActions.TASK_CREATED_LOG} {result}");

        return result;
    }

    public async Task<ServiceResult<TicketDto>> DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var taskToDelete = await GetTaskById(taskId, cancellationToken);

        if (!taskToDelete.Success)
            return new ServiceResult<TicketDto>
            {
                Success = taskToDelete.Success,
                ErrorMessage = taskToDelete.ErrorMessage
            };

        _context.Tickets.Remove(taskToDelete.Data!);
        await _context.SaveChangesAsync(cancellationToken);

        return new ServiceResult<TicketDto>
        {
            Success = true,
            Data = ConvertTaskToDto(taskToDelete.Data)
        };
    }

    public async Task<ServiceResult<TicketDto>> EditTaskByIdAsync(Guid taskId, TicketDto newTask, CancellationToken cancellationToken)
    {
        var taskToEdit = await GetTaskById(taskId, cancellationToken);

        if (!taskToEdit.Success)
            return new ServiceResult<TicketDto>
            {
                Success = taskToEdit.Success,
                ErrorMessage = taskToEdit.ErrorMessage
            };

        try
        {
            if (taskToEdit.Data!.Title != newTask.Title && newTask.Title != null)
            {
                var oldTitle = taskToEdit.Data!.Title;
                taskToEdit.Data!.Title = newTask.Title!;
                _taskHistoryEvent?.Invoke(this, new TicketHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Data!.Id,
                    Author = (Guid)taskToEdit.Data!.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_TITLE,
                    PreviousState = oldTitle,
                    NewState = taskToEdit.Data!.Title
                }));
            }

            if (taskToEdit.Data!.Description != newTask.Description && newTask.Description != null)
            {
                var oldDescription = taskToEdit.Data!.Description;
                taskToEdit.Data!.Description = newTask.Description!;
                _taskHistoryEvent?.Invoke(this, new TicketHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Data!.Id,
                    Author = (Guid)taskToEdit.Data!.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_DESCRIPTION,
                    PreviousState = oldDescription,
                    NewState = taskToEdit.Data!.Description
                }));
            }

            if (taskToEdit.Data!.ReporterId != newTask.ReporterId && newTask.ReporterId != null)
            {
                var oldReporter = taskToEdit.Data!.ReporterId;
                taskToEdit.Data!.ReporterId = (Guid)newTask.ReporterId!;
                _taskHistoryEvent?.Invoke(this, new TicketHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Data!.Id,
                    Author = (Guid)taskToEdit.Data!.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_REPORTEDID,
                    PreviousState = oldReporter.ToString(),
                    NewState = taskToEdit.Data!.ReporterId.ToString()
                }));
            }

            if (taskToEdit.Data!.AssigneeId != newTask.AssigneeId && newTask.AssigneeId != null)
            {
                var oldAssignee = taskToEdit.Data!.AssigneeId;
                taskToEdit.Data!.AssigneeId = (Guid)newTask.AssigneeId!;
                _taskHistoryEvent?.Invoke(this, new TicketHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Data!.Id,
                    Author = (Guid)taskToEdit.Data!.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_ASSIGNEEID,
                    PreviousState = oldAssignee.ToString(),
                    NewState = taskToEdit.Data!.AssigneeId.ToString()
                }));
            }

            if (taskToEdit.Data!.ProjectId != newTask.ProjectId && newTask.ProjectId != null)
            {
                var oldProject = taskToEdit.Data!.ProjectId;
                taskToEdit.Data!.ProjectId = (Guid)newTask.ProjectId!;
                _taskHistoryEvent?.Invoke(this, new TicketHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Data!.Id,
                    Author = (Guid)taskToEdit.Data!.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_PROJECT,
                    PreviousState = oldProject.ToString(),
                    NewState = taskToEdit.Data!.ProjectId.ToString()
                }));
            }

            if (taskToEdit.Data!.ParentId != newTask.ParentId && newTask.ParentId != null)
            {
                var oldParent = taskToEdit.Data!.ParentId;
                taskToEdit.Data!.ParentId = (Guid)newTask.ParentId!;
                _taskHistoryEvent?.Invoke(this, new TicketHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Data!.Id,
                    Author = (Guid)taskToEdit.Data!.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_PARENTTASK,
                    PreviousState = oldParent.ToString(),
                    NewState = taskToEdit.Data!.ParentId.ToString()
                }));
            }

            if (taskToEdit.Data!.StatusId != newTask.StatusId && newTask.StatusId != 0)
            {
                var oldstatus = taskToEdit.Data!.StatusId;
                taskToEdit.Data!.StatusId = newTask.StatusId!;
                _taskHistoryEvent?.Invoke(this, new TicketHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Data!.Id,
                    Author = (Guid)taskToEdit.Data!.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_STATUS,
                    PreviousState = oldstatus.ToString(),
                    NewState = taskToEdit.Data!.StatusId.ToString()
                }));
            }

            if (taskToEdit.Data!.TypeId != newTask.TypeId && newTask.TypeId != 0 && newTask.TypeId != null)
            {
                var oldType = taskToEdit.Data!.TypeId;
                taskToEdit.Data!.TypeId = newTask.TypeId!;
                _taskHistoryEvent?.Invoke(this, new TicketHistoryEventArgs(new TicketHistoryDto
                {
                    TaskId = taskToEdit.Data!.Id,
                    Author = (Guid)taskToEdit.Data!.ReporterId,
                    EventName = TaskHistoryTypes.TaskEdit.TASK_EDITED_TASKTYPE,
                    PreviousState = oldType.ToString(),
                    NewState = taskToEdit.Data!.TypeId.ToString()
                }));
            }

            taskToEdit.Data!.ModifyDate = DateTime.UtcNow;

            _context.Tickets.Update(taskToEdit.Data!);
            await _context.SaveChangesAsync(cancellationToken);

            return await GetTaskByIdDto(taskToEdit.Data!.Id, cancellationToken);
        }
        catch (System.Exception ex)
        {
            _logger.LogCritical($"Edit task => {ex}");
            return new ServiceResult<TicketDto>
            {
                Success = false,
                ErrorMessage = ex.ToString()
            };
        }
    }

    public async Task<ServiceResult<TicketDto>> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var findTask = await GetTaskByIdDto(taskId, cancellationToken);

        if (!findTask.Success)
            return new ServiceResult<TicketDto>
            {
                Success = findTask.Success,
                ErrorMessage = findTask.ErrorMessage
            };

        return findTask;
    }

    public async Task<ServiceResult<List<TicketDto>>> GetTasksByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var res = await _context.Tickets.Include(t => t.TaskItemStatus)
                                     .Include(t => t.TaskType)
                                     .Where(t => t.ProjectItem.OrganizationId == organizationId)
                                     .Select(t => ConvertTaskToDto(t))
                                     .ToListAsync(cancellationToken);
        if (res is null)
            return new ServiceResult<List<TicketDto>>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        return new ServiceResult<List<TicketDto>>
        {
            Success = true,
            Data = res!
        };
    }

    public async Task<ServiceResult<List<TicketDto>>> GetTasksByProjectAsync(Guid projectId, CancellationToken cancellationToken)
    {
        var res = await _context.Tickets.Include(t => t.TaskItemStatus)
                                     .Include(t => t.TaskType)
                                     .Where(t => t.ProjectId == projectId)
                                     .Select(t => ConvertTaskToDto(t))
                                     .ToListAsync(cancellationToken); 
        if (res is null)
            return new ServiceResult<List<TicketDto>>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        return new ServiceResult<List<TicketDto>>
        {
            Success = true,
            Data = res!
        };
    }

    public async Task<ServiceResult<List<TicketDto>>> CreateTicketsForAiAsync(TicketForAiDto[] newTasks, CancellationToken cancellationToken)
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
            }, cancellationToken);
            createdTasks.Add((newTask.Data, task.ParentName));
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
                    var updated = await EditTaskByIdAsync((Guid)task.ticket.Id!, task.ticket, cancellationToken);

                    if (updated.Success)
                        edit.Add(updated.Data);
                }
            }
        }

        var finalTickets = new List<TicketDto>();
        foreach (var item in createdTasks)
        {
            var res = await GetTaskByIdAsync((Guid)item.ticket.Id!, cancellationToken);
            finalTickets.Add(res.Data);
        }

        return new ServiceResult<List<TicketDto>>
        {
            Success = true,
            Data = finalTickets
        };
    }

    private async Task<ServiceResult<Ticket>> GetTaskById(Guid taskId, CancellationToken cancellationToken)
    {
        var res = await _context.Tickets.Include(t => t.TaskItemStatus)
                                     .Include(t => t.TaskType)
                                     .Include(p => p.ProjectItem)
                                     .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);
        if (res is null)
            return new ServiceResult<Ticket>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        return new ServiceResult<Ticket>
        {
            Success = true,
            Data = res!
        }; 
    }

    private async Task<ServiceResult<TicketDto>> GetTaskByIdDto(Guid taskId, CancellationToken cancellationToken)
    {
        var task = await GetTaskById(taskId, cancellationToken);
        var childIssues = await GetChildIssues(taskId, cancellationToken);
        var res = ConvertTaskToDto(task.Data);
        if (res == null)
            return new ServiceResult<TicketDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        res.ChildIssues = childIssues.Data!.Count == 0 ? null : childIssues.Data;

        if (childIssues.Data.Count == 0)
            res.ChildIssues = null;

        return new ServiceResult<TicketDto>
        {
            Success = true,
            Data = res
        };
    }

    private async Task<ServiceResult<List<TicketDto>>> GetChildIssues(Guid taskId, CancellationToken cancellationToken)
    {
        var res = await _context.Tickets.Include(t => t.TaskItemStatus)
                                     .Include(t => t.TaskType)
                                     .Where(t => t.ParentId == taskId)
                                     .Select(t => ConvertTaskToDto(t))
                                     .ToListAsync(cancellationToken);
        if (res is null)
            return new ServiceResult<List<TicketDto>>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        return new ServiceResult<List<TicketDto>>
        {
            Success = true,
            Data = res!
        };
    }

    public static TicketDto? ConvertTaskToDto(Ticket? task)
    {
        if (task == null)
            return null;

        return new TicketDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            StatusId = task.StatusId,
            StatusName = task.TaskItemStatus?.Name ?? null,
            TypeId = task.TypeId,
            TypeName = task.TaskType?.Name ?? null,
            ProjectId = task.ProjectId,
            ReporterId = task.ReporterId,
            AssigneeId = task.AssigneeId,
            ParentId = task.ParentId,
            OrganizationId = task.ProjectItem?.OrganizationId,
            CreateDate = task.CreateDate,
            ModifyDate = task.ModifyDate
        };
    }
}
