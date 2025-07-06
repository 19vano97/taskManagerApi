using System;

namespace TaskManagerApi.Models.TaskItemStatuses;

public class TicketStatusDto
{
    public required int TypeId { get; set; }
    public string? TypeName { get; set; }
    public int? StatusId { get; set; }
    public string StatusName { get; set; }
    public required int Order { get; set; }
}
