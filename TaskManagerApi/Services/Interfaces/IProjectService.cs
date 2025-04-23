using System;
using System.Security.Claims;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Project;

namespace TaskManagerApi.Services.Interfaces;

public interface IProjectService
{
    Task<List<ProjectItemDto>> GetProjectsAsync(Guid organizationId, ClaimsPrincipal user);
    Task<ProjectItemDto> GetProjectById(Guid projectId, Guid organizationId, ClaimsPrincipal user);
    Task<ProjectItemDto> CreateProjectAsync(ProjectItemDto newProject, Guid organizationId, ClaimsPrincipal user);
    Task<ProjectItemDto> EditProjectAsync(Guid Id, ProjectItemDto newProject, Guid organizationId, ClaimsPrincipal user);
    Task<ProjectItemDto> DeleteProjectAsync(Guid Id, Guid organizationId, ClaimsPrincipal user);
    Task<ProjectItemDto> ChangeOwnerAsync(Guid projectId, Guid newOwner, Guid organizationId, ClaimsPrincipal user);
}
