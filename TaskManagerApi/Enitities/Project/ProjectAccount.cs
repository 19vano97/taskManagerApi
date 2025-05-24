using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerApi.Enitities.Project;

public class ProjectAccount
{
    [Key]
    public Guid Id { get; set; }
    public required Guid AccountId { get; set; }
    public required Guid ProjectId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime ModifyDate { get; set; }
    [ForeignKey("ProjectId")]
    public ProjectItem Project { get; set; }
}
