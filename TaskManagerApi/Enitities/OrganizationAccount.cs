using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerApi.Enitities;

public class OrganizationAccount
{
    [Key]
    public Guid Id { get; set; }
    public required Guid AccountId { get; set; }
    public required Guid OrganizationId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime ModifyDate { get; set; }
    [ForeignKey("OrganizationId")]
    public OrganizationItem Organization { get; set; }
}
