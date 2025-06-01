using System;

namespace TaskManagerApi.Models.OrganizationModel;

public class OrganizationAccountsDto : OrganizationDto
{
    public List<Guid> Accounts { get; set; }
}
