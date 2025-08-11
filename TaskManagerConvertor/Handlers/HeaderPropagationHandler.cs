using System;
using TaskManagerConvertor.Providers.Interfaces;

namespace TaskManagerConvertor.Handlers;

public class HeaderPropagationHandler : DelegatingHandler
{
    private readonly IHeaderProvider _headerProvider;

    public HeaderPropagationHandler(IHeaderProvider headerProvider)
    {
        _headerProvider = headerProvider;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var headers = _headerProvider.GetHeaders();
        foreach (var header in headers)
        {
            if (!request.Headers.Contains(header.Key))
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}
