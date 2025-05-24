using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Enitities.Task;

public class TaskItemStatusType
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
}
