using System;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities;
using TaskManagerApi.Models.TaskItemStatuses;
using TaskManagerApi.Services.Interfaces;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Services.Implementations;

public class ProjectStatusesService(TaskManagerAPIDbContext context) : IProjectStatusesService
{
    public async Task<ProjectSingleStatusDto> AddAsync(ProjectSingleStatusDto status)
    {
        var currentStatuses = await GetAllStatusesFromProject(status.ProjectId);
        
        if (currentStatuses.Count > DefaultParametersForUsers.ProjectLimitations.MAX_STATUSES_VALUE)
        {
            return null!;
        }

        var checkStatus = await CheckStatusExisted(status.Status.StatusName, status.Status.TypeId);
        var statusToAdd = checkStatus == null! 
            ? await AddNewStatus(status.Status.StatusName, status.Status.TypeId)
            : checkStatus;

        foreach (var item in currentStatuses)
        {
            if (item.Order < status.Status.Order)
            {
                currentStatuses.Remove(item);
            }
            else
            {
                item.Order++;
            }
        }

        context.ProjectTaskStatusMapping.Add(new ProjectTaskStatusMapping
        {
            ProjectId = status.ProjectId,
            StatusId = statusToAdd.Id,
            Order = status.Status.Order
        });

        foreach (var item in currentStatuses)
        {
            context.ProjectTaskStatusMapping.Update(item);
        }

        await context.SaveChangesAsync();
        var result = await context.ProjectTaskStatusMapping.Include(s => s.TaskItemStatus.taskItemStatusType).FirstOrDefaultAsync(s => s.ProjectId == status.ProjectId && s.Order == status.Status.Order);

        return CovertFromProjectStatusesMapping(result!);
    }

    public async Task<ProjectSingleStatusDto> DeleteAsync(ProjectSingleStatusDto status)
    {
        var checkStatus = await CheckStatusExisted(status.Status.StatusName, status.Status.StatusId);
        if (checkStatus is null)
        {
            return null!;
        }

        var currentStatuses = await GetAllStatusesFromProject(status.ProjectId);
        if (true)
        {
            
        }

        foreach (var item in currentStatuses)
        {
            if (item.Order >= status.Status.Order)
            {
                item.Order--;
            }
        }

        context.ProjectTaskStatusMapping.Remove(currentStatuses.First(s => s.Order == status.Status.Order));

        await context.SaveChangesAsync();

        return status;
    }

    private async Task<TaskItemStatus> CheckStatusExisted(string status, int typeId)
    {
        return await context.TaskItemStatuses.FirstOrDefaultAsync(s => s.Name == status && s.StatusTypeId == typeId);
    }

    private async Task<List<ProjectTaskStatusMapping>> GetAllStatusesFromProject(Guid projectId)
    {
        return await context.ProjectTaskStatusMapping.Where(s => s.ProjectId == projectId).ToListAsync();
    }

    private async Task<TaskItemStatus> AddNewStatus(string status, int typeId)
    {
        context.TaskItemStatuses.Add(new TaskItemStatus {
            Name = status,
            StatusTypeId = typeId
        });

        await context.SaveChangesAsync();

        return await context.TaskItemStatuses.FirstAsync(s => s.Name == status && s.StatusTypeId == typeId);
    }

    private static ProjectSingleStatusDto CovertFromProjectStatusesMapping(ProjectTaskStatusMapping statusMapping)
    {
        return new ProjectSingleStatusDto{
            ProjectId = statusMapping.ProjectId,
            Status = new TaskItemStatusDto{
                TypeId = statusMapping.TaskItemStatus.StatusTypeId,
                TypeName = statusMapping.TaskItemStatus.taskItemStatusType.Name,
                StatusId = statusMapping.StatusId,
                StatusName = statusMapping.TaskItemStatus.Name,
                Order = statusMapping.Order
            }
        };
    }
}
