using System;

namespace TaskManagerApi.Models.Verification;

public class VerificationOrganizationAccountDto
{
    public bool IsVerified { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid AccountId { get; set; }
}
