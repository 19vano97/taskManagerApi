using System;

namespace TaskManagerApi.Models.AI;

public class AiThreadDetailsDto
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? AccountId { get; set; }
    public DateTime? CreateDate { get; set; }
}
