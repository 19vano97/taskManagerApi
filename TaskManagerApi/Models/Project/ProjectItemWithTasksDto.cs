using System;
using TaskManagerApi.Models.TicketItem;

namespace TaskManagerApi.Models.Project;

public class ProjectItemWithTasksDto
{
    public ProjectItemDto Project { get; set; }
    public List<TicketDto> Tasks { get; set; }
}
