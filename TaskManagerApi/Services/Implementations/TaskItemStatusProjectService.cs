using System;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities;
using TaskManagerApi.Models.TaskItemStatuses;
using TaskManagerApi.Services.Interfaces;

namespace TaskManagerApi.Services.Implementations;

public class TaskItemStatusProjectService(TaskManagerAPIDbContext context) : ITaskItemStatusProjectService
{
    public Task<TaskItemStatusProjectSingleDto> AddAsync(TaskItemStatusProjectSingleDto status)
    {
        throw new NotImplementedException();
    }

    public async Task<TaskItemStatusProjectSingleDto> DeleteAsync(TaskItemStatusProjectSingleDto status)
    {
        throw new NotImplementedException();
    }

    public async Task<TaskItemAllStatusesProjectDto> SyncStatusesAsync(TaskItemAllStatusesProjectDto status)
    {
        throw new NotImplementedException();
    }

    private async Task<TaskItemStatus> CheckStatusExisted(string status, int typeId)
    {
        return await context.TaskItemStatuses.FirstOrDefaultAsync(s => s.Name == status && s.StatusTypeId == typeId);
    }
}
