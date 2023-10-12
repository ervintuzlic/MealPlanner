using MealPlanner.Client.Providers;
using System.Net.Http.Headers;

namespace MealPlanner.Client.Handlers;

public class JwtTokenMessageHandler : DelegatingHandler
{
    private readonly Uri _allowedBaseAddress;
    private readonly JwtAuthenticationStateProvider _loginStateService;

    public JwtTokenMessageHandler(Uri allowedBaseAddress, JwtAuthenticationStateProvider loginStateService)
    {
        _allowedBaseAddress = allowedBaseAddress;
        _loginStateService = loginStateService;
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return SendAsync(request, cancellationToken).Result;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uri = request.RequestUri;
        var isSelfApiAccess = _allowedBaseAddress.IsBaseOf(uri);

        if (isSelfApiAccess)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _loginStateService.Token ?? string.Empty);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
