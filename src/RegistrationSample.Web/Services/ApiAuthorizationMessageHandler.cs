using System.Net.Http.Headers;

namespace RegistrationSample.Web.Services;

public class ApiAuthorizationMessageHandler : DelegatingHandler
{
    private readonly TokenService _tokenService;

    public ApiAuthorizationMessageHandler(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_tokenService.IsAuthenticated)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.Token);

        return base.SendAsync(request, cancellationToken);
    }
}
