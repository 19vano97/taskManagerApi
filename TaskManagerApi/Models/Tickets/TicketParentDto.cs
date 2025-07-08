using System;

namespace TaskManagerApi.Models.TicketItem;

public class TicketParentDto
{
    public required Guid ParentId { get; set; }
    public required Guid ChildId { get; set; }
}
