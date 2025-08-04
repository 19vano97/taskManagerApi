using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerConvertor.Models.Ticket;

/// <summary>
/// Data Transfer Object for a ticket status.
/// </summary>
public class TicketStatusDto
{
    /// <summary>
    /// The type identifier of the status.
    /// </summary>
    [Required]
    public int TypeId { get; set; }

    /// <summary>
    /// The name of the status type.
    /// </summary>
    public string? TypeName { get; set; }

    /// <summary>
    /// The unique identifier of the status.
    /// </summary>
    public int? StatusId { get; set; }

    /// <summary>
    /// The name of the status.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// The order of the status in the workflow.
    /// </summary>
    [Required]
    public int Order { get; set; }
}
