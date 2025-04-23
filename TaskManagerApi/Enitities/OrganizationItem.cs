using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Enitities;

public class OrganizationItem
{
    [Key]
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string Abbreviation { get; set; }
    public required Guid Owner { get; set; }
    public string Description { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
    public virtual ICollection<ProjectItem> ProjectItems { get; set; } = new List<ProjectItem>();
    public virtual ICollection<OrganizationAccount> OrganizationAccounts { get; set; } = new List<OrganizationAccount>();
}
