using System;
using System.Security.Claims;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Project;

namespace TaskManagerApi.Services.Interfaces;

public interface IProjectService
{
    Task<List<ProjectItemDto>> GetProjectsAsync(Guid organizationId);
    Task<ProjectItemDto> GetProjectByIdAsync(Guid projectId);
    Task<ProjectItemDto> CreateProjectAsync(ProjectItemDto newProject);
    Task<ProjectItemDto> EditProjectAsync(ProjectItemDto newProject);
    Task<ProjectItemDto> DeleteProjectAsync(Guid Id);
    Task<ProjectItemDto> ChangeOwnerAsync(Guid projectId, Guid newOwner);
}
