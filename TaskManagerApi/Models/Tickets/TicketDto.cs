using System;
using TaskManagerApi.Enitities;
using TaskManagerApi.Enums;
using TaskManagerApi.Models.Project;

namespace TaskManagerApi.Models.TaskItem;

public class TicketDto
{
    public Guid? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? StatusId { get; set; }
    public string? StatusName { get; set; }
    public int? TypeId { get; set; }
    public string? TypeName { get; set; }
    public bool isCreatedByAi { get; set; } = false;
    public Guid? ReporterId { get; set; }
    public Guid? AssigneeId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? OrganizationId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public TimeOnly? SpentTime { get; set; }
    public TimeOnly? Estimate { get; set; }
    public List<TicketDto>? ChildIssues { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}
