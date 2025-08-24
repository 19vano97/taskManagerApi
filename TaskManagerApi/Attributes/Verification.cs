using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskManagerApi.Models;
using TaskManagerApi.Services.Interfaces.Business;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class Verification : Attribute, IAsyncActionFilter
{
    private readonly string _organizationIdKey;
    private readonly IAccountVerification _accountVerification;

    public Verification(IAccountVerification accountVerification,
                        string organizationIdKey = Constants.Headers.ORGANIZATION_ID)
    {
        _organizationIdKey = organizationIdKey;
        _accountVerification = accountVerification;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;

        if (!context.HttpContext.Request.Headers.TryGetValue(_organizationIdKey, out var organizationIdString)
               || !Guid.TryParse(organizationIdString, out Guid organizationId)
               || !Guid.TryParse(user.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
               || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId))
        { 
            context.Result = new BadRequestObjectResult("verification is failed");
        }
    }
}
