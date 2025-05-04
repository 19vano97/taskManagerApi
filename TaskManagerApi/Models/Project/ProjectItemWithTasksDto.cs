using System;
using TaskManagerApi.Models.TaskItem;

namespace TaskManagerApi.Models.Project;

public class ProjectItemWithTasksDto
{
    public ProjectItemDto Project { get; set; }
    public List<TaskItemDto> Tasks { get; set; }
}
