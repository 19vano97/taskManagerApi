using System;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Project;
using TaskManagerApi.Services.Interfaces;

namespace TaskManagerApi.Services.Implementations;

public class ProjectService(TaskManagerAPIDbContext context) : IProjectService
{
    public async Task<ProjectItemDto> ChangeOwnerAsync(Guid projectId, Guid newOwner)
    {
        var projectToEdit = await context.ProjectItems.FirstOrDefaultAsync(t => t.Id == projectId);

        if (projectToEdit is null || !Guid.TryParse(newOwner.ToString(), out var checkedAssignee))
            return null;

        projectToEdit.OwnerId = newOwner;
        projectToEdit.ModifyDate = DateTime.UtcNow;

        context.ProjectItems.Update(projectToEdit);
        await context.SaveChangesAsync();

        return ConvertProjectToOutputAsync(await context.ProjectItems.FirstOrDefaultAsync(t => t.Id == projectId));
    }

    public async Task<ProjectItemDto> CreateProjectAsync(ProjectItemDto newProject)
    {
        if (newProject.Title is null)
            return null;

        var projectToAdd = new ProjectItem{
                Id = Guid.NewGuid(),
                Title = newProject.Title,
                Description = newProject.Description
            };
        
        context.ProjectItems.Add(projectToAdd);
        await context.SaveChangesAsync();

        var result = await context.ProjectItems.FirstOrDefaultAsync(id => id.Id == projectToAdd.Id);

        return ConvertProjectToOutputAsync(result!);
    }

    public async Task<ProjectItemDto> DeleteProjectAsync(Guid Id)
    {
         var projectToDelete = await context.ProjectItems.FirstOrDefaultAsync(t => t.Id == Id);

        if (projectToDelete is null)
            return null;

        context.ProjectItems.Remove(projectToDelete);
        await context.SaveChangesAsync();

        return ConvertProjectToOutputAsync(projectToDelete);
    }

    public async Task<ProjectItemDto> EditProjectAsync(Guid Id, ProjectItemDto newProject)
    {
        var projectToEdit = await context.ProjectItems.FirstOrDefaultAsync(t => t.Id == Id);

        if (projectToEdit is null)
            return null;
        
        if (newProject.Title is not null)
            projectToEdit.Title = newProject.Title;
        
        if (newProject.Description is not null)
            projectToEdit.Description = newProject.Description;

        if (newProject.OwnerId is not null)
            projectToEdit.OwnerId = newProject.OwnerId;

        projectToEdit.ModifyDate = DateTime.UtcNow;

        context.ProjectItems.Update(projectToEdit);
        await context.SaveChangesAsync();

        return ConvertProjectToOutputAsync(await context.ProjectItems.FirstOrDefaultAsync(t => t.Id == Id));
    }

    public async Task<List<ProjectItemDto>> GetProjectsAsync()
    {
        return await context.ProjectItems.Select(p => ConvertProjectToOutputAsync(p)).ToListAsync();
    }

    public async Task<ProjectItemDto> GetProjectById(Guid projectId)
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
