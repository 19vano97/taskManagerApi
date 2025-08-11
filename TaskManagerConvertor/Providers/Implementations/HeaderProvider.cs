using System;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Providers.Interfaces;

namespace TaskManagerConvertor.Providers.Implementations;

public class HeaderProvider : IHeaderProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public HeaderProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IDictionary<string, string> GetHeaders()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var headers = new Dictionary<string, string>();

        if (httpContext != null)
        {
            var forwardList = new[] { Constants.Settings.Header.AUTHORIZATION,  Constants.Settings.Header.ORGANIZATION};

            foreach (var headerName in forwardList)
            {
                if (httpContext.Request.Headers.TryGetValue(headerName, out var value))
                {
                    headers[headerName] = value.ToString();
                }
            }
        }

        return headers;
    }
}
