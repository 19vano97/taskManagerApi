using System;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Project;

namespace TaskManagerApi.Services.Interfaces;

public interface IProjectService
{
    Task<List<ProjectItemDto>> GetProjectsAsync();
    Task<ProjectItemDto> GetProjectById(Guid projectId);
    Task<ProjectItemDto> CreateProjectAsync(ProjectItemDto newProject);
    Task<ProjectItemDto> EditProjectAsync(Guid Id, ProjectItemDto newProject);
    Task<ProjectItemDto> DeleteProjectAsync(Guid Id);
    Task<ProjectItemDto> ChangeOwnerAsync(Guid projectId, Guid newOwner);
}
