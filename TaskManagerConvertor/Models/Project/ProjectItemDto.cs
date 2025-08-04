using System;
using System.ComponentModel.DataAnnotations;
using TaskManagerConvertor.Models.Ticket;
using TaskManagerConvertor.Models.TicketItem;

namespace TaskManagerConvertor.Models.Project;

public class ProjectItemDto
{
    /// <summary>
    /// The unique identifier of the project.
    /// </summary>
    public Guid Id { get; set; } = Guid.Empty;

    /// <summary>
    /// The title of the project.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The description of the project.
    /// </summary>
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier of the owner of the project.
    /// </summary>
    [Required]
    public Guid OwnerId { get; set; } = Guid.Empty;
    public AccountDto? Owner { get; set; }

    /// <summary>
    /// The unique identifier of the organization to which the project belongs.
    /// </summary>
    [Required]
    public Guid OrganizationId { get; set; } = Guid.Empty;

    /// <summary>
    /// The list of statuses associated with the project.
    /// </summary>
    public List<TicketStatusDto>? Statuses { get; set; }

    public List<TicketDto>? Tickets { get; set; }

    /// <summary>
    /// The creation date of the project.
    /// </summary>
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The last modification date of the project.
    /// </summary>
    public DateTime ModidyDate { get; set; } = DateTime.UtcNow;
}
