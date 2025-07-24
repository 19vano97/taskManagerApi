using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Models.Tickets;

public class TicketForAiDto
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    public int? Type { get; set; }
    public bool isCreatedByAi { get; set; } = true;
    public Guid? AssigneeId { get; set; }
    public Guid? ReporterId { get; set; }
    public string? ParentName { get; set; }

    [Required]
    public Guid ProjectId { get; set; }

    public DateTime? CreateDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}
