using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Project;
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

        return ConvertProjectToOutputAsync(await context.ProjectItems.FirstOrDefaultAsync(t => t.Id == projectId));
    }

    public async Task<ProjectItemDto> CreateProjectAsync(ProjectItemDto newProject, Guid ownerID)
    {
        var projectToAdd = new ProjectItem{
                Id = Guid.NewGuid(),
                Title = newProject.Title,
                Description = newProject.Description,
                OrganizationId = newProject.OrganizationId,
                OwnerId = ownerID
            };
        
        context.ProjectItems.Add(projectToAdd);
        await context.SaveChangesAsync();

        var result = await context.ProjectItems.FirstOrDefaultAsync(id => id.Id == projectToAdd.Id);

        return ConvertProjectToOutputAsync(result!);
    }

    public async Task<ProjectItemDto> DeleteProjectAsync(Guid projectId)
    {
        var projectToDelete = await context.ProjectItems.FirstAsync(p => p.Id == projectId);

        if (projectToDelete is null)
            return null;

        context.ProjectItems.Remove(projectToDelete);
        await context.SaveChangesAsync();

        return ConvertProjectToOutputAsync(projectToDelete);
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

        return ConvertProjectToOutputAsync(await context.ProjectItems.FirstOrDefaultAsync(t => t.Id == projectToEdit.Id));
    }

    public async Task<List<ProjectItemDto>> GetProjectsAsync(Guid organizationId)
    {
        return await context.ProjectItems.Where(p => p.OrganizationId == organizationId).Select(p => ConvertProjectToOutputAsync(p)).ToListAsync();
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
            CreateDate = project.CreateDate
        };
    }
}
