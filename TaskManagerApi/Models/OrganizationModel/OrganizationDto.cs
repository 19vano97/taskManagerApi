using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Models.OrganizationModel;

public class OrganizationDto
{
    public Guid Id { get; set; } = Guid.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(10)]
    public string Abbreviation { get; set; } = string.Empty;

    [Required]
    public Guid Owner { get; set; } = Guid.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
}
