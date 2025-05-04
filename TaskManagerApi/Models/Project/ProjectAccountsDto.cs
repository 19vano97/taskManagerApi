using System;

namespace TaskManagerApi.Models.Project;

public class ProjectAccountsDto
{
    public required ProjectItemDto Project { get; set; }
    public List<Guid> Accounts { get; set; }
}
