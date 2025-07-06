using System;
using System.Security.Claims;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Project;
using TaskManagerApi.Models.TaskItemStatuses;

namespace TaskManagerApi.Services.Interfaces;

public interface IProjectService
{
    Task<List<ProjectItemDto>> GetProjectsByOrganizationIdAsync(Guid organizationId);
    Task<ProjectItemDto> GetProjectByIdAsync(Guid projectId);
    Task<ProjectAccountsDto> GetAccountsByProjectId(Guid projectId);
    Task<ProjectItemDto> CreateProjectAsync(ProjectItemDto newProject);
    Task<ProjectItemDto> EditProjectAsync(ProjectItemDto newProject);
    Task<ProjectItemDto> DeleteProjectAsync(Guid Id);
    Task<ProjectItemDto> ChangeOwnerAsync(Guid projectId, Guid newOwner);
    Task<ProjectSingleStatusDto> AddStatusAsync(ProjectSingleStatusDto status, bool ShouldBeSavedOnDb = true);
    Task<ProjectItemDto> EditStatusesAsync(ProjectItemDto status);
    Task<ProjectSingleStatusDto> DeleteStatusAsync(ProjectSingleStatusDto status);
}
