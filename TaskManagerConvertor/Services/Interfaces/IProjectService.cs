using System;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Models.Project;

namespace TaskManagerConvertor.Services.Interfaces;

public interface IProjectService
{
    public Task<RequestResult<List<ProjectItemDto>>> GetAllProjectsWithTasksListAsync(IHeaderDictionary headers, Guid organizationId, CancellationToken cancellationToken);
    public Task<RequestResult<ProjectItemDto>> GetProjectByIdAsync(IHeaderDictionary headers, Guid projectId, CancellationToken cancellationToken);
    public Task<RequestResult<ProjectItemDto>> GetProjectWithTasksByIdAsync(IHeaderDictionary headers, Guid projectId, CancellationToken cancellationToken);
    public Task<RequestResult<ProjectItemDto>> CreateProjectAsync(IHeaderDictionary headers, ProjectItemDto project, CancellationToken cancellationToken);
    public Task<RequestResult<ProjectItemDto>> EditProjectByIdAsync(IHeaderDictionary headers, ProjectItemDto editProject,
                                                                    Guid projectId, CancellationToken cancellationToken);
    public Task<RequestResult<ProjectItemDto>> DeleteProjectAsync(IHeaderDictionary headers, Guid projectId, CancellationToken cancellationToken);
}
