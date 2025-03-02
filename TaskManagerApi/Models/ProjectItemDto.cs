using System;

namespace TaskManagerApi.Models;

public class ProjectItemDto
{
    public Guid? Id { get; set; }
    public string? Title { get; set; } 
    public string? Description { get; set; }
    public Guid? OwnerId { get; set; } = null;
    public DateTime? CreateDate { get; set; }
}
