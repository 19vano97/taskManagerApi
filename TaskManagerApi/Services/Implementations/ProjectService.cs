// Full fixed version of ProjectService.cs with safe status editing/deleting

using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities.Project;
using TaskManagerApi.Enitities.Task;
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
        var projectToEdit = await _context.ProjectItems.FirstOrDefaultAsync(p => p.Id == projectId);
        if (projectToEdit is null) return null;

        projectToEdit.OwnerId = newOwner;
        projectToEdit.ModifyDate = DateTime.UtcNow;
        _context.ProjectItems.Update(projectToEdit);
        await _context.SaveChangesAsync();

        return GeneralService.ConvertProjectToOutput(projectToEdit, await GetStatuses(projectId));
    }

    public async Task<ProjectItemDto> CreateProjectAsync(ProjectItemDto newProject)
    {
        var projectToAdd = new ProjectItem
        {
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

        var result = GeneralService.ConvertProjectToOutput(projectToAdd);
        result.Statuses = await AddDefaultStatuses(result.Id);

        return result;
    }

    public async Task<ProjectItemDto> DeleteProjectAsync(Guid projectId)
    {
        var projectToDelete = await _context.ProjectItems.FirstOrDefaultAsync(p => p.Id == projectId);
        if (projectToDelete is null) return null;

        var unassign = await _context.ProjectAccounts.Where(p => p.ProjectId == projectToDelete.Id).ToListAsync();

        _context.ProjectItems.Remove(projectToDelete);
        _context.ProjectAccounts.RemoveRange(unassign);
        await _context.SaveChangesAsync();

        return GeneralService.ConvertProjectToOutput(projectToDelete, await GetStatuses(projectId));
    }

    public async Task<ProjectItemDto> EditProjectAsync(ProjectItemDto newProject)
    {
        var projectToEdit = await _context.ProjectItems.FirstOrDefaultAsync(p => p.Id == newProject.Id);
        if (projectToEdit is null) return null;

        if (projectToEdit.Title != newProject.Title && newProject.Title != null)
            projectToEdit.Title = newProject.Title;
        if (projectToEdit.Description != newProject.Description && newProject.Description != null)
            projectToEdit.Description = newProject.Description;
        if (projectToEdit.OwnerId != newProject.OwnerId && newProject.OwnerId != Guid.Empty)
            projectToEdit.OwnerId = newProject.OwnerId;

        projectToEdit.ModifyDate = DateTime.UtcNow;
        _context.ProjectItems.Update(projectToEdit);
        await _context.SaveChangesAsync();

        return await EditStatusesAsync(newProject);
    }

    public async Task<ProjectItemDto> EditStatusesAsync(ProjectItemDto project)
    {
        if (project.Statuses is null) return project;

        var currentMappings = await _context.ProjectTaskStatusMapping
            .Where(s => s.ProjectId == project.Id)
            .Include(s => s.TicketStatus)
            .ToListAsync();

        var newStatuses = project.Statuses;
        var statusesInDb = await _context.TicketStatuses.ToListAsync();

        // Ensure all statuses have valid StatusIds (either pre-existing or newly created)
        foreach (var newStatus in newStatuses)
        {
            if (!newStatus.StatusId.HasValue && !string.IsNullOrWhiteSpace(newStatus.StatusName))
            {
                // Try to find existing status
                var existing = await _context.TicketStatuses.FirstOrDefaultAsync(s => s.Name == newStatus.StatusName && s.StatusTypeId == newStatus.TypeId);
                if (existing != null)
                {
                    newStatus.StatusId = existing.Id;
                }
                else
                {
                    var created = new TicketStatus
                    {
                        Name = newStatus.StatusName,
                        StatusTypeId = newStatus.TypeId
                    };

                    _context.TicketStatuses.Add(created);
                    await _context.SaveChangesAsync();

                    newStatus.StatusId = created.Id;
                }
            }
        }

        var validStatusIds = newStatuses.Where(s => s.StatusId.HasValue).Select(s => (s.StatusId.Value, s.StatusName)).ToHashSet();

        foreach (var current in currentMappings)
        {
            var statusInDb = statusesInDb.First(s => s.Id == current.StatusId).Name;
            if (!validStatusIds.Contains((current.StatusId, current.TicketStatus.Name)))
            {
                bool isUsed = await _context.Tickets.AnyAsync(t => t.StatusId == current.StatusId);
                if (!isUsed)
                {
                    _context.ProjectTaskStatusMapping.Remove(current);
                }
            }
        }

        foreach (var newStatus in newStatuses)
        {
            if (!newStatus.StatusId.HasValue) continue;

            var exists = currentMappings.FirstOrDefault(s => s.StatusId == newStatus.StatusId);
            if (exists == null)
            {
                var created = await AddStatusAsync(new ProjectSingleStatusDto
                    {
                        ProjectId = project.Id,
                        Status = new TicketStatusDto
                        {
                            StatusId = newStatus.StatusId,
                            StatusName = newStatus.StatusName,
                            TypeId = newStatus.TypeId,
                            TypeName = newStatus.TypeName,
                            Order = newStatus.Order
                        }
                    });
            }
            else if (exists.Order != newStatus.Order)
            {
                exists.Order = newStatus.Order;
                _context.ProjectTaskStatusMapping.Update(exists);
            }
        }

        await _context.SaveChangesAsync();
        return await GetProjectByIdAsync(project.Id);
    }

    public async Task<ProjectSingleStatusDto> AddStatusAsync(ProjectSingleStatusDto status, bool shouldSave = true)
    {
        var currentStatuses = await GetAllStatusesFromProject(status.ProjectId);
        if (currentStatuses.Count > DefaultParametersForUsers.ProjectLimitations.MAX_STATUSES_VALUE)
            return null!;

        var checkStatus = await CheckStatusExisted(status.Status.StatusName, status.Status.TypeId);
        var statusToAdd = checkStatus ?? await AddNewStatus(status.Status.StatusName, status.Status.TypeId);

        var toShift = currentStatuses.Where(s => s.Order >= status.Status.Order).ToList();
        foreach (var item in toShift)
        {
            item.Order++;
            _context.ProjectTaskStatusMapping.Update(item);
        }

        _context.ProjectTaskStatusMapping.Add(new ProjectTaskStatusMapping
        {
            ProjectId = status.ProjectId,
            StatusId = statusToAdd.Id,
            Order = status.Status.Order
        });

        if (shouldSave)
            await _context.SaveChangesAsync();

        var result = await _context.ProjectTaskStatusMapping
            .Include(s => s.TicketStatus.TicketStatusType)
            .FirstOrDefaultAsync(s => s.ProjectId == status.ProjectId && s.Order == status.Status.Order);

        return CovertFromProjectStatusesMapping(result!);
    }

    public async Task<ProjectSingleStatusDto> DeleteStatusAsync(ProjectSingleStatusDto status)
    {
        var currentStatuses = await GetAllStatusesFromProject(status.ProjectId);
        var target = currentStatuses.FirstOrDefault(s => s.StatusId == status.Status.StatusId);
        if (target == null) return null!;

        bool isUsed = await _context.Tickets.AnyAsync(t => t.StatusId == status.Status.StatusId);
        if (isUsed) return null!; // can't delete used status

        _context.ProjectTaskStatusMapping.Remove(target);
        foreach (var item in currentStatuses.Where(s => s.Order > target.Order))
        {
            item.Order--;
            _context.ProjectTaskStatusMapping.Update(item);
        }

        await _context.SaveChangesAsync();
        return status;
    }

    private async Task<TicketStatus> CheckStatusExisted(string status, int typeId) =>
        await _context.TicketStatuses.FirstOrDefaultAsync(s => s.Name == status && s.StatusTypeId == typeId);

    private async Task<List<ProjectTaskStatusMapping>> GetAllStatusesFromProject(Guid projectId) =>
        await _context.ProjectTaskStatusMapping.Where(s => s.ProjectId == projectId).ToListAsync();

    private async Task<TicketStatus> AddNewStatus(string status, int typeId)
    {
        _context.TicketStatuses.Add(new TicketStatus { Name = status, StatusTypeId = typeId });
        await _context.SaveChangesAsync();
        return await _context.TicketStatuses.FirstAsync(s => s.Name == status && s.StatusTypeId == typeId);
    }

    private static ProjectSingleStatusDto CovertFromProjectStatusesMapping(ProjectTaskStatusMapping statusMapping)
    {
        return new ProjectSingleStatusDto
        {
            ProjectId = statusMapping.ProjectId,
            Status = new TicketStatusDto
            {
                TypeId = statusMapping.TicketStatus.StatusTypeId,
                TypeName = statusMapping.TicketStatus.TicketStatusType.Name,
                StatusId = statusMapping.StatusId,
                StatusName = statusMapping.TicketStatus.Name,
                Order = statusMapping.Order
            }
        };
    }

    private async Task<List<TicketStatusDto>> AddDefaultStatuses(Guid projectId)
    {
        foreach (var status in StatusesConstants.DEFAULT_LIST)
        {
            _context.ProjectTaskStatusMapping.Add(new ProjectTaskStatusMapping
            {
                ProjectId = projectId,
                StatusId = (int)status.StatusId,
                Order = status.Order
            });
        }

        await _context.SaveChangesAsync();
        return await GetStatuses(projectId);
    }

    private async Task<List<TicketStatusDto>> GetStatuses(Guid projectId)
    {
        return GeneralService.ConvertProjectStatusToDto(
            await _context.ProjectTaskStatusMapping.Include(s => s.TicketStatus.TicketStatusType)
                .Where(s => s.ProjectId == projectId).ToListAsync()
        );
    }

    public async Task<ProjectItemDto> GetProjectByIdAsync(Guid projectId)
    {
        var statuses = await GetStatuses(projectId);
        var project = await _context.ProjectItems.FirstOrDefaultAsync(p => p.Id == projectId);
        return project == null ? null : GeneralService.ConvertProjectToOutput(project, statuses);
    }

    public async Task<List<ProjectItemDto>> GetProjectsByOrganizationIdAsync(Guid organizationId)
    {
        var projects = await _context.ProjectItems.Where(p => p.OrganizationId == organizationId).ToListAsync();
        var result = new List<ProjectItemDto>();

        foreach (var project in projects)
        {
            var statuses = await GetStatuses(project.Id);
            result.Add(GeneralService.ConvertProjectToOutput(project, statuses));
        }

        return result;
    }

    public async Task<ProjectAccountsDto> GetAccountsByProjectId(Guid projectId)
    {
        var project = await GetProjectByIdAsync(projectId);
        if (project == null) return null!;

        return new ProjectAccountsDto
        {
            Project = project,
            Accounts = await _context.ProjectAccounts.Where(p => p.ProjectId == project.Id)
                .Select(p => p.AccountId).ToListAsync()
        };
    }
}