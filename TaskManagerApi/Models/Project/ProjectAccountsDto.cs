using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Models.Project;

public class ProjectAccountsDto
{
    [Required]
    public ProjectItemDto Project { get; set; } = new ProjectItemDto();

    public List<Guid> Accounts { get; set; } = new List<Guid>();
}
