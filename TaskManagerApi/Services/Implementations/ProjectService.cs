// Full fixed version of ProjectService.cs with safe status editing/deleting

using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities.Project;
using TaskManagerApi.Enitities.Task;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Project;
using TaskManagerApi.Models.TicketItemStatuses;
using TaskManagerApi.Services.Interfaces;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Services.Implementations;

public class ProjectService : IProjectService
{
    private readonly TicketManagerAPIDbContext _context;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(TicketManagerAPIDbContext context, ILogger<ProjectService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<ProjectItemDto>> CreateProjectAsync(ProjectItemDto newProject, CancellationToken cancellationToken)
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
        await _context.SaveChangesAsync(cancellationToken);

        await AddDefaultStatuses(projectToAdd.Id, cancellationToken);

        return await GetProjectByIdAsync(projectToAdd.Id, cancellationToken);
    }

    public async Task<ServiceResult<ProjectItemDto>> DeleteProjectAsync(Guid projectId, CancellationToken cancellationToken)
    {
        var projectToDelete = await _context.ProjectItems.FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
        if (projectToDelete is null)
            return new ServiceResult<ProjectItemDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        var unassign = await _context.ProjectAccounts.Where(p => p.ProjectId == projectToDelete.Id).ToListAsync(cancellationToken);

        _context.ProjectItems.Remove(projectToDelete);
        _context.ProjectAccounts.RemoveRange(unassign);
        await _context.SaveChangesAsync(cancellationToken);

        var check = await GetProjectByIdAsync(projectToDelete.Id, cancellationToken);
        if (!check.Success)
            return new ServiceResult<ProjectItemDto>
            {
                Success = true
            };

        return new ServiceResult<ProjectItemDto>
        {
            Success = false,
            ErrorMessage = string.Format(LogPhrases.ServiceResult.Error.DELETETION_ISSUE, projectId)
        };
    }

    public async Task<ServiceResult<ProjectItemDto>> EditProjectAsync(ProjectItemDto newProject, CancellationToken cancellationToken)
    {
        var projectToEdit = await _context.ProjectItems.FirstOrDefaultAsync(p => p.Id == newProject.Id);
        if (projectToEdit is null)
            return new ServiceResult<ProjectItemDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        if (projectToEdit.Title != newProject.Title && newProject.Title != null)
                projectToEdit.Title = newProject.Title;
        if (projectToEdit.Description != newProject.Description && newProject.Description != null)
            projectToEdit.Description = newProject.Description;
        if (projectToEdit.OwnerId != newProject.OwnerId && newProject.OwnerId != Guid.Empty)
            projectToEdit.OwnerId = newProject.OwnerId;

        projectToEdit.ModifyDate = DateTime.UtcNow;
        _context.ProjectItems.Update(projectToEdit);
        await _context.SaveChangesAsync(cancellationToken);

        return await EditStatusesAsync(newProject, cancellationToken);
    }

    public async Task<ServiceResult<ProjectItemDto>> EditStatusesAsync(ProjectItemDto project, CancellationToken cancellationToken)
    {
        if (project.Statuses is null)
            return new ServiceResult<ProjectItemDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.STATUSES_EMPTY
            };

        var currentMappings = await _context.ProjectTaskStatusMapping
            .Where(s => s.ProjectId == project.Id)
            .Include(s => s.TicketStatus)
            .ToListAsync(cancellationToken);

        var newStatuses = project.Statuses;
        var statusesInDb = await _context.TicketStatuses.ToListAsync(cancellationToken);

        foreach (var newStatus in newStatuses)
        {
            if (!newStatus.StatusId.HasValue && !string.IsNullOrWhiteSpace(newStatus.StatusName))
            {
                var existing = await _context.TicketStatuses.FirstOrDefaultAsync(s => s.Name == newStatus.StatusName
                                                                                    && s.StatusTypeId == newStatus.TypeId
                                                                                , cancellationToken);
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
                    await _context.SaveChangesAsync(cancellationToken);

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
                }, cancellationToken);
            }
            else if (exists.Order != newStatus.Order)
            {
                exists.Order = newStatus.Order;
                _context.ProjectTaskStatusMapping.Update(exists);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return await GetProjectByIdAsync(project.Id, cancellationToken);
    }

    public async Task<ServiceResult<ProjectSingleStatusDto>> AddStatusAsync(ProjectSingleStatusDto status, CancellationToken cancellationToken)
    {
        var currentStatuses = await GetAllStatusesFromProject(status.ProjectId, cancellationToken);
        if (currentStatuses.Data!.Count > DefaultParametersForUsers.ProjectLimitations.MAX_STATUSES_VALUE)
            return new ServiceResult<ProjectSingleStatusDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.MAX_STATUSES_VALUE
            };

        var checkStatus = await CheckStatusExisted(status.Status.StatusName, status.Status.TypeId, cancellationToken);
        var statusToAdd = checkStatus ?? await AddNewStatus(status.Status.StatusName, status.Status.TypeId, cancellationToken);

        var toShift = currentStatuses.Data.Where(s => s.Order >= status.Status.Order).ToList();
        foreach (var item in toShift)
        {
            item.Order++;
            _context.ProjectTaskStatusMapping.Update(item);
        }

        _context.ProjectTaskStatusMapping.Add(new ProjectTaskStatusMapping
        {
            ProjectId = status.ProjectId,
            StatusId = statusToAdd.Data.Id,
            Order = status.Status.Order
        });

        var result = await _context.ProjectTaskStatusMapping
            .Include(s => s.TicketStatus.TicketStatusType)
            .FirstOrDefaultAsync(s => s.ProjectId == status.ProjectId && s.Order == status.Status.Order, cancellationToken);

        return new ServiceResult<ProjectSingleStatusDto>
        {
            Success = true,
            Data = CovertFromProjectStatusesMapping(result!)
        };
    }

    public async Task<ServiceResult<ProjectSingleStatusDto>> DeleteStatusAsync(ProjectSingleStatusDto status, CancellationToken cancellationToken)
    {
        var currentStatuses = await GetAllStatusesFromProject(status.ProjectId, cancellationToken);
        if (!currentStatuses.Success)
            return new ServiceResult<ProjectSingleStatusDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        var target = currentStatuses.Data.FirstOrDefault(s => s.StatusId == status.Status.StatusId);
        if (target == null)
            return new ServiceResult<ProjectSingleStatusDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        bool isUsed = await _context.Tickets.AnyAsync(t => t.StatusId == status.Status.StatusId);
        if (isUsed)
            return new ServiceResult<ProjectSingleStatusDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        _context.ProjectTaskStatusMapping.Remove(target);
        foreach (var item in currentStatuses.Data.Where(s => s.Order > target.Order))
        {
            item.Order--;
            _context.ProjectTaskStatusMapping.Update(item);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return new ServiceResult<ProjectSingleStatusDto>
        {
            Success = true,
            Data = status
        };
    }

    public async Task<ServiceResult<ProjectItemDto>> GetProjectByIdAsync(Guid projectId, CancellationToken cancellationToken)
    {
        var statuses = await GetStatuses(projectId, cancellationToken);
        if (statuses is null)
            return new ServiceResult<ProjectItemDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        var project = await _context.ProjectItems.FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
        if (project is null)
            return new ServiceResult<ProjectItemDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };


        return new ServiceResult<ProjectItemDto>
        {
            Success = true,
            Data = ConvertProjectToOutput(project, ConvertProjectStatusToDto(statuses.Data))
        };
    }

    public async Task<ServiceResult<List<ProjectItemDto>>> GetProjectsByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var projects = await _context.ProjectItems.Where(p => p.OrganizationId == organizationId).ToListAsync(cancellationToken);
        var result = new List<ProjectItemDto>();

        foreach (var project in projects)
        {
            var statuses = await GetStatuses(project.Id, cancellationToken);
            if (!statuses.Success) result.Add(ConvertProjectToOutput(project, ConvertProjectStatusToDto(statuses.Data)));
        }

        return new ServiceResult<List<ProjectItemDto>>
        {
            Success = true,
            Data = result
        };
    }

    public async Task<ServiceResult<ProjectAccountsDto>> GetAccountsByProjectId(Guid projectId, CancellationToken cancellationToken)
    {
        var project = await GetProjectByIdAsync(projectId, cancellationToken);
        if (project == null)
            return new ServiceResult<ProjectAccountsDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        return new ServiceResult<ProjectAccountsDto>
        {
            Success = true,
            Data = new ProjectAccountsDto
            {
                Project = project.Data,
                Accounts = await _context.ProjectAccounts.Where(p => p.ProjectId == project.Data.Id)
                    .Select(p => p.AccountId).ToListAsync(cancellationToken)
            }
        };
    }

    private async Task<ServiceResult<TicketStatus>> CheckStatusExisted(string status, int typeId, CancellationToken cancellationToken)
    {
        var res = await _context.TicketStatuses.FirstOrDefaultAsync(s => s.Name == status && s.StatusTypeId == typeId, cancellationToken);
        if (res is null)
            return new ServiceResult<TicketStatus>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        return new ServiceResult<TicketStatus>
        {
            Success = true,
            Data = res
        };
    }

    private async Task<ServiceResult<List<ProjectTaskStatusMapping>>> GetAllStatusesFromProject(Guid projectId, CancellationToken cancellationToken)
    {
        var res = await _context.ProjectTaskStatusMapping.Where(s => s.ProjectId == projectId).ToListAsync(cancellationToken);
        if (res is null)
            return new ServiceResult<List<ProjectTaskStatusMapping>>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        return new ServiceResult<List<ProjectTaskStatusMapping>>
        {
            Success = true,
            Data = res
        };
    }

    private async Task<ServiceResult<TicketStatus>> AddNewStatus(string status, int typeId, CancellationToken cancellationToken)
    {
        _context.TicketStatuses.Add(new TicketStatus { Name = status, StatusTypeId = typeId });
        await _context.SaveChangesAsync(cancellationToken);

        var res = await _context.TicketStatuses.FirstAsync(s => s.Name == status && s.StatusTypeId == typeId);
        if (res is null)
            return new ServiceResult<TicketStatus>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        return new ServiceResult<TicketStatus>
        {
            Success = true,
            Data = res
        };
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

    private async Task<ServiceResult<List<ProjectTaskStatusMapping>>> AddDefaultStatuses(Guid projectId, CancellationToken cancellationToken)
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

        await _context.SaveChangesAsync(cancellationToken);
        return await GetStatuses(projectId, cancellationToken);
    }

    private async Task<ServiceResult<List<ProjectTaskStatusMapping>>> GetStatuses(Guid projectId, CancellationToken cancellationToken)
    {
        var res = await _context.ProjectTaskStatusMapping.Include(s => s.TicketStatus.TicketStatusType)
                .Where(s => s.ProjectId == projectId).ToListAsync(cancellationToken);
        if (res is null)
            return new ServiceResult<List<ProjectTaskStatusMapping>>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        return new ServiceResult<List<ProjectTaskStatusMapping>>
        {
            Success = true,
            Data = res
        };
    }

    private static ProjectItemDto ConvertProjectToOutput(ProjectItem project, List<TicketStatusDto> statuses)
    {
        return new ProjectItemDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            Statuses = statuses,
            OwnerId = project.OwnerId,
            OrganizationId = project.OrganizationId,
            CreateDate = project.CreateDate
        };
    }

    private static List<TicketStatusDto> ConvertProjectStatusToDto(List<ProjectTaskStatusMapping> list)
    {
        var newList = new List<TicketStatusDto>();

        foreach (var item in list)
        {
            newList.Add(new TicketStatusDto
            {
                TypeId = item.TicketStatus.StatusTypeId,
                TypeName = item.TicketStatus.TicketStatusType.Name,
                StatusId = item.StatusId,
                StatusName = item.TicketStatus.Name,
                Order = item.Order
            });
        }

        return newList.OrderBy(t => t.Order).ToList();
    }
}