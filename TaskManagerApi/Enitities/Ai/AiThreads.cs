using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagerApi.Enitities.Organization;

namespace TaskManagerApi.Enitities.Ai;

public class AiThreads
{
    [Key]
    public Guid Id { get; set; }
    public Guid OrganizationAccountId { get; set; }
    public string Name { get; set; }
    public string Thread { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime ModifyDate { get; set; }
    [ForeignKey("OrganizationAccountId")]
    public OrganizationAccount OrganizationAccount { get; set; }
}
