using System;
using System.ComponentModel.DataAnnotations;
using TaskManagerConvertor.Models.Project;

namespace TaskManagerConvertor.Models.Organization;

public class OrganizationDto
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(10)]
    public string Abbreviation { get; set; } = string.Empty;

    [Required]
    public Guid OwnerId { get; set; }
    public AccountDto? Owner { get; set; }

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    public List<Guid>? AccountIds { get; set; }
    public List<AccountDto>? Accounts { get; set; }
    public List<ProjectItemDto>? Projects { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
}
