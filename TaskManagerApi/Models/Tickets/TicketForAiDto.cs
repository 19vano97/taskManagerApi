using System;

namespace TaskManagerApi.Models.Tickets;

public class TicketForAiDto
{
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? TypeId { get; set; } = 1;
    public bool isCreatedByAi { get; set; } = true;
    public Guid? AssigneeId { get; set; }
    public Guid? ReporterId { get; set; }
    public string? ParentName { get; set; }
    public required Guid ProjectId { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}
