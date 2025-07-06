using System;

namespace TaskManagerApi.Models.TaskItem;

public class TicketParentDto
{
    public required Guid ParentId { get; set; }
    public required Guid ChildId { get; set; }
}
