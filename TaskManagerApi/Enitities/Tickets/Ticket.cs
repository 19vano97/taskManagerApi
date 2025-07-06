using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagerApi.Enitities.Project;

namespace TaskManagerApi.Enitities.Task;

public class Ticket
{
    [Key]
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? TypeId { get; set; } = 1;
    public int? StatusId { get; set; } = 2;
    public bool isCreatedByAi { get; set; } = false;
    public Guid? ReporterId { get; set; }
    public Guid? AssigneeId { get; set; }
    public Guid? ParentId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public TimeOnly? SpentTime { get; set; }
    public TimeOnly? Estimate { get; set; }
    public required Guid ProjectId { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
    [ForeignKey("ProjectId")]
    public ProjectItem? ProjectItem { get; set; }
    [ForeignKey("StatusId")]
    public TicketStatus? TaskItemStatus { get; set; }
    [ForeignKey("TypeId")]
    public TicketType? TaskType { get; set; }
}
