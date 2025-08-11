using System;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Models.Project;

namespace TaskManagerConvertor.Services.Interfaces;

public interface IProjectService
{
    public Task<RequestResult<List<ProjectItemDto>>> GetAllProjectsWithTasksListAsync(Guid organizationId, CancellationToken cancellationToken);
    public Task<RequestResult<ProjectItemDto>> GetProjectByIdAsync(Guid projectId, CancellationToken cancellationToken);
    public Task<RequestResult<ProjectItemDto>> GetProjectWithTasksByIdAsync(Guid projectId, CancellationToken cancellationToken);
    public Task<RequestResult<ProjectItemDto>> CreateProjectAsync(ProjectItemDto project, CancellationToken cancellationToken);
    public Task<RequestResult<ProjectItemDto>> EditProjectByIdAsync(ProjectItemDto editProject,
                                                                    Guid projectId, CancellationToken cancellationToken);
    public Task<RequestResult<ProjectItemDto>> DeleteProjectAsync(Guid projectId, CancellationToken cancellationToken);
}
