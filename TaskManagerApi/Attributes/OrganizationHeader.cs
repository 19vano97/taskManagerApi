using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskManagerApi.Models;

namespace TaskManagerApi.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class OrganizationHeader : Attribute, IActionFilter
{
    private readonly string _headerName;

    public OrganizationHeader(string headerName = Constants.Headers.ORGANIZATION_ID)
    {
        _headerName = headerName;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {

    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var httpContext = context.HttpContext;

        if (!httpContext.Request.Headers.TryGetValue(_headerName, out var organizationIdString)
            || string.IsNullOrWhiteSpace(organizationIdString)
            || !Guid.TryParse(organizationIdString, out Guid organizationId))
        {
            context.Result = new BadRequestObjectResult($"{_headerName} is absent or invalid");
        }
    }
}
