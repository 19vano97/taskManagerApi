using System;
using TaskManagerApi.Models.OrganizationModel;

namespace TaskManagerApi.Models.Account;

public class AccountDefaultInfo
{
    public Guid Id { get; set; }
    public List<OrganizationProjectDto>? Organizations { get; set; }
}
