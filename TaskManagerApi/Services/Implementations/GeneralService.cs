using System;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities.Organization;
using TaskManagerApi.Enitities.Project;
using TaskManagerApi.Enitities.Task;
using TaskManagerApi.Models.Project;
using TaskManagerApi.Models.TaskItem;
using TaskManagerApi.Models.TaskItemStatuses;

namespace TaskManagerApi.Services.Implementations;

public class GeneralService
{
    public static async Task<OrganizationAccount> VerifyAccountRelatesToOrganization(TaskManagerAPIDbContext context, Guid accountId, Guid organizationId)
    {
        return await context.OrganizationAccount.FirstOrDefaultAsync(o => o.AccountId == accountId && o.OrganizationId == organizationId);
    }

    public static async Task<ProjectItem> VerifyProjectInOrganization(TaskManagerAPIDbContext context, Guid projectId, Guid organizationId)
    {
        var project = await context.ProjectItems.FirstOrDefaultAsync(t => t.Id == projectId);

        if (project is null)
            return null!;

        return project;
    }

    public static List<TaskItemStatusDto> CopyListFromImmutableList(ImmutableList<TaskItemStatusDto> immutableList)
    {
        var newList = new List<TaskItemStatusDto>();

        foreach (var item in immutableList)
        {
            newList.Add(item);
        }

        return newList;
    }

    public static List<TaskItemStatusDto> ConvertProejctStatusToDto(List<ProjectTaskStatusMapping> list)
    {
        var newList = new List<TaskItemStatusDto>();

        foreach (var item in list)
        {
            newList.Add(new TaskItemStatusDto
            {
                TypeId = item.TaskItemStatus.StatusTypeId,
                TypeName = item.TaskItemStatus.taskItemStatusType.Name,
                StatusId = item.StatusId,
                StatusName = item.TaskItemStatus.Name,
                Order = item.Order
            });
        }

        return newList.OrderBy(t => t.Order).ToList();
    }

    public static TaskItemDto ConvertTaskToDto(TaskItem task)
    {
        return new TaskItemDto
        {
            Id = task.Id,
            Title = task!.Title,
            Description = task.Description,
            StatusId = task.StatusId,
            StatusName = task.TaskItemStatus?.Name ?? null,
            Type = task.TypeId,
            TypeName = task.TaskType?.Name ?? null,
            ProjectId = task.ProjectId,
            ReporterId = task.ReporterId,
            AssigneeId = task.AssigneeId,
            ParentId = task.ParentId,
            CreateDate = task.CreateDate,
            ModifyDate = task.ModifyDate
        };
    }

    public static ProjectItemDto ConvertProjectToOutput(ProjectItem project)
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

    public static ProjectItemDto ConvertProjectToOutput(ProjectItem project, List<TaskItemStatusDto> statuses)
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
}
