using System;
using TaskManagerApi.Models.TaskItemStatuses;

namespace TaskManagerApi.Services.Interfaces;

public interface IProjectStatusesService
{
    Task<ProjectSingleStatusDto> AddAsync(ProjectSingleStatusDto status);
    // Task<TaskItemAllStatusesProjectDto> SyncStatusesAsync(TaskItemAllStatusesProjectDto status);
    Task<ProjectSingleStatusDto> DeleteAsync(ProjectSingleStatusDto status);
}
