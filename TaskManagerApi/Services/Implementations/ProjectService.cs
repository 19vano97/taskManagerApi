using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Project;
using TaskManagerApi.Models.TaskItemStatuses;
using TaskManagerApi.Services.Interfaces;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Services.Implementations;

public class ProjectService(TaskManagerAPIDbContext context) : IProjectService
{
    public async Task<ProjectItemDto> ChangeOwnerAsync(Guid projectId, Guid newOwner)
    {
        var projectToEdit =  await context.ProjectItems.FirstAsync(p => p.Id == projectId);

        if (projectToEdit is null)
            return null;

        projectToEdit.OwnerId = newOwner;
        projectToEdit.ModifyDate = DateTime.UtcNow;

        context.ProjectItems.Update(projectToEdit);
        await context.SaveChangesAsync();

        return ConvertProjectToOutput(await context.ProjectItems.FirstOrDefaultAsync(t => t.Id == projectId), await GetStatuses(projectId));
    }

    public async Task<ProjectItemDto> CreateProjectAsync(ProjectItemDto newProject)
    {
        var projectToAdd = new ProjectItem{
                Id = Guid.NewGuid(),
                Title = newProject.Title,
                Description = newProject.Description,
                OrganizationId = newProject.OrganizationId,
                OwnerId = newProject.OwnerId
            };
        
        context.ProjectItems.Add(projectToAdd);
        await context.SaveChangesAsync();

        var resultFromDb = await context.ProjectItems.FirstOrDefaultAsync(id => id.Id == projectToAdd.Id);
        var result = ConvertProjectToOutputAsync(resultFromDb!);
        result.Statuses = await AddStatuses(result.Id);

        return result;
    }

    public async Task<ProjectItemDto> DeleteProjectAsync(Guid projectId)
    {
        var projectToDelete = await context.ProjectItems.FirstAsync(p => p.Id == projectId);

        if (projectToDelete is null)
            return null;

        context.ProjectItems.Remove(projectToDelete);
        await context.SaveChangesAsync();

        return ConvertProjectToOutput(projectToDelete, await GetStatuses(projectId));
    }

    public async Task<ProjectItemDto> EditProjectAsync(ProjectItemDto newProject)
    {
        var projectToEdit =  await context.ProjectItems.FirstAsync(p => p.Id == newProject.Id);

        if (projectToEdit is null)
            return null;

        if (projectToEdit is null)
            return null;
        
        if (newProject.Title is not null && projectToEdit.Title != newProject.Title)
            projectToEdit.Title = newProject.Title;
        
        if (newProject.Description is not null && projectToEdit.Description != newProject.Description)
            projectToEdit.Description = newProject.Description;

        if (newProject.OwnerId != Guid.Empty && projectToEdit.OwnerId != newProject.OwnerId)
            projectToEdit.OwnerId = newProject.OwnerId;

        projectToEdit.ModifyDate = DateTime.UtcNow;

        context.ProjectItems.Update(projectToEdit);
        await context.SaveChangesAsync();

        return ConvertProjectToOutput(await context.ProjectItems.FirstOrDefaultAsync(t => t.Id == projectToEdit.Id), await GetStatuses(projectToEdit.Id));
    }

    public async Task<List<ProjectItemDto>> GetProjectsAsync(Guid organizationId)
    {
        var projects = await context.ProjectItems.Where(p => p.OrganizationId == organizationId).ToListAsync();
        var projectsDto = new List<ProjectItemDto>();

        foreach (var project in projects)
        {
            projectsDto.Add(ConvertProjectToOutput(project, await GetStatuses(project.Id)));
        }

        return projectsDto;
    }

    public async Task<ProjectItemDto> GetProjectByIdAsync(Guid projectId)
    {
        return await context.ProjectItems.Where(p => p.Id == projectId)
            .Select(p => ConvertProjectToOutputAsync(p)).FirstOrDefaultAsync();
    }

    private static ProjectItemDto ConvertProjectToOutputAsync(ProjectItem project)
    {
        return new ProjectItemDto{
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            OwnerId = project.OwnerId,
            OrganizationId = project.OrganizationId,
            CreateDate = project.CreateDate
        };
    }

    private static ProjectItemDto ConvertProjectToOutput(ProjectItem project, List<TaskItemStatusDto> statuses)
    {
        return new ProjectItemDto{
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            Statuses = statuses,
            OwnerId = project.OwnerId,
            OrganizationId = project.OrganizationId,
            CreateDate = project.CreateDate
        };
    }

    private async Task<List<TaskItemStatusDto>> AddStatuses(Guid projectId)
    {
        foreach (var status in StatusesConstants.DEFAULT_LIST)
        {
            context.ProjectTaskStatusMapping.Add(new ProjectTaskStatusMapping {
                ProjectId = projectId,
                StatusId = status.StatusId,
                Order = status.Order
            });
        }

        await context.SaveChangesAsync();

        return await GetStatuses(projectId);
    }

    private async Task<List<TaskItemStatusDto>> GetStatuses(Guid projectId)
    {
        return GeneralService.ConvertProejctStatusToDto(await context.ProjectTaskStatusMapping.Include(s => s.TaskItemStatus.taskItemStatusType).Where(s => s.ProjectId == projectId).ToListAsync());
    }
}
