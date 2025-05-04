using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerApi.Enitities;

public class TaskItemStatus
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required int StatusTypeId { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifyDate { get; set; } = DateTime.UtcNow;
    [ForeignKey("StatusTypeId")]
    public TaskItemStatusType taskItemStatusType { get; set; }
}
