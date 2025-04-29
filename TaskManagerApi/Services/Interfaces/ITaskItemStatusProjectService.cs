using System;
using TaskManagerApi.Models.TaskItemStatuses;

namespace TaskManagerApi.Services.Interfaces;

public interface ITaskItemStatusProjectService
{
    Task<TaskItemStatusProjectSingleDto> AddAsync(TaskItemStatusProjectSingleDto status);
    Task<TaskItemAllStatusesProjectDto> SyncStatusesAsync(TaskItemAllStatusesProjectDto status);
    Task<TaskItemStatusProjectSingleDto> DeleteAsync(TaskItemStatusProjectSingleDto status);
}
