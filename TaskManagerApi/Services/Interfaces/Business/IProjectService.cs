using System;
using System.Security.Claims;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Project;
using TaskManagerApi.Models.TicketItemStatuses;

namespace TaskManagerApi.Services.Interfaces.Business;

public interface IProjectService
{
    Task<ServiceResult<List<ProjectItemDto>>> GetProjectsByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken);
    Task<ServiceResult<ProjectItemDto>> GetProjectByIdAsync(Guid projectId, CancellationToken cancellationToken);
    Task<ServiceResult<ProjectAccountsDto>> GetAccountsByProjectId(Guid projectId, CancellationToken cancellationToken);
    Task<ServiceResult<ProjectItemDto>> CreateProjectAsync(ProjectItemDto newProject, CancellationToken cancellationToken);
    Task<ServiceResult<ProjectItemDto>> EditProjectAsync(ProjectItemDto newProject, CancellationToken cancellationToken);
    Task<ServiceResult<ProjectItemDto>> DeleteProjectAsync(Guid Id, CancellationToken cancellationToken);
    Task<ServiceResult<ProjectSingleStatusDto>> AddStatusAsync(ProjectSingleStatusDto status, CancellationToken cancellationToken);
    Task<ServiceResult<ProjectItemDto>> EditStatusesAsync(ProjectItemDto status, CancellationToken cancellationToken);
    Task<ServiceResult<ProjectSingleStatusDto>> DeleteStatusAsync(ProjectSingleStatusDto status, CancellationToken cancellationToken);
}
