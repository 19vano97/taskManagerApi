using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities.Project;
using TaskManagerApi.Models.Project;
using TaskManagerApi.Models.TaskItemStatuses;
using TaskManagerApi.Services.Interfaces;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Services.Implementations;

public class ProjectService : IProjectService
{
    private readonly TaskManagerAPIDbContext _context;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(TaskManagerAPIDbContext context, ILogger<ProjectService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProjectItemDto> ChangeOwnerAsync(Guid projectId, Guid newOwner)
    {
        var projectToEdit =  await _context.ProjectItems.FirstAsync(p => p.Id == projectId);

        if (projectToEdit is null)
            return null;

        projectToEdit.OwnerId = newOwner;
        projectToEdit.ModifyDate = DateTime.UtcNow;

        _context.ProjectItems.Update(projectToEdit);
        await _context.SaveChangesAsync();

        return GeneralService.ConvertProjectToOutput(await _context.ProjectItems.FirstOrDefaultAsync(t => t.Id == projectId), 
                                      await GetStatuses(projectId));
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
        
        _context.ProjectItems.Add(projectToAdd);
        _context.ProjectAccounts.Add(new ProjectAccount 
        {
            AccountId = newProject.OwnerId,
            ProjectId = projectToAdd.Id
        });
        await _context.SaveChangesAsync();

        var resultFromDb = await _context.ProjectItems.FirstOrDefaultAsync(id => id.Id == projectToAdd.Id);
        var result = GeneralService.ConvertProjectToOutput(resultFromDb!);
        result.Statuses = await AddDefaultStatuses(result.Id);

        return result;
    }

    public async Task<ProjectItemDto> DeleteProjectAsync(Guid projectId)
    {
        var projectToDelete = await _context.ProjectItems.FirstAsync(p => p.Id == projectId);

        if (projectToDelete is null)
            return null;

        var unassignPeopleFromProject = await _context.ProjectAccounts.Where(p => p.ProjectId == projectToDelete.Id)
                                                                     .ToListAsync();

        _context.ProjectItems.Remove(projectToDelete);
        _context.ProjectAccounts.RemoveRange(unassignPeopleFromProject);
        await _context.SaveChangesAsync();

        return GeneralService.ConvertProjectToOutput(projectToDelete, await GetStatuses(projectId));
    }

    public async Task<ProjectItemDto> EditProjectAsync(ProjectItemDto newProject)
    {
        var projectToEdit =  await _context.ProjectItems.FirstAsync(p => p.Id == newProject.Id);

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

        _context.ProjectItems.Update(projectToEdit);
        await _context.SaveChangesAsync();

        return GeneralService.ConvertProjectToOutput(await _context.ProjectItems.FirstOrDefaultAsync(t => t.Id == projectToEdit.Id), 
                                      await GetStatuses(projectToEdit.Id));
    }

    public async Task<List<ProjectItemDto>> GetProjectsAsync(Guid organizationId)
    {
        var projects = await _context.ProjectItems.Where(p => p.OrganizationId == organizationId).ToListAsync();
        var projectsDto = new List<ProjectItemDto>();

        foreach (var project in projects)
        {
            projectsDto.Add(GeneralService.ConvertProjectToOutput(project, await GetStatuses(project.Id)));
        }

        return projectsDto;
    }

    public async Task<ProjectItemDto> GetProjectByIdAsync(Guid projectId)
    {
        var statuses = await GetStatuses(projectId);
        return await _context.ProjectItems.Where(p => p.Id == projectId)
                                         .Select(p => GeneralService.ConvertProjectToOutput(p, statuses))
                                         .FirstOrDefaultAsync();
    }

    public async Task<ProjectAccountsDto> GetAccountsByProjectId(Guid projectId)
    {
        var project = await GetProjectByIdAsync(projectId);

        if (project is null)
        {
            return null!;
        }
        
        return new ProjectAccountsDto
        {
            Project = project,
            Accounts = await _context.ProjectAccounts.Where(p => p.ProjectId == project.Id)
                                                    .Select(p => p.AccountId).ToListAsync()
        };
    }

    private async Task<List<TaskItemStatusDto>> AddDefaultStatuses(Guid projectId)
    {
        foreach (var status in StatusesConstants.DEFAULT_LIST)
        {
            _context.ProjectTaskStatusMapping.Add(new ProjectTaskStatusMapping {
                ProjectId = projectId,
                StatusId = status.StatusId,
                Order = status.Order
            });
        }

        await _context.SaveChangesAsync();

        return await GetStatuses(projectId);
    }

    private async Task<List<TaskItemStatusDto>> GetStatuses(Guid projectId)
    {
        return GeneralService.ConvertProejctStatusToDto(await _context.ProjectTaskStatusMapping.Include(s => s.TaskItemStatus.taskItemStatusType).Where(s => s.ProjectId == projectId).ToListAsync());
    }
}
