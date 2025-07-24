using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Models.TicketItemStatuses;

/// <summary>
/// Data Transfer Object representing a single status for a project.
/// </summary>
public class ProjectSingleStatusDto
{
    /// <summary>
    /// The unique identifier of the project to which the status belongs.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// The status details for the project (type, name, order, etc).
    /// </summary>
    [Required]
    public TicketStatusDto Status { get; set; } = new TicketStatusDto();
}
