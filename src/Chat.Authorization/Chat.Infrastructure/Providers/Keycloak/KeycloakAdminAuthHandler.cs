using Chat.Infrastructure.Options.Keycloak;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.Providers.Keycloak;

public class KeycloakAdminAuthHandler(
    KeycloakTokenService tokenService,
    IOptions<KeycloakOptions> options) : DelegatingHandler
{
    private readonly KeycloakOptions _options = options.Value;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tokenResult = await tokenService.GetAdminToken(_options.AdminClient, cancellationToken);

        if (tokenResult.IsError)
        {
            throw new InvalidOperationException("Could not acquire Keycloak admin token");
        }
        
        tokenService.AuthorizeRequest(request, tokenResult.Value);
        
        return await base.SendAsync(request, cancellationToken);
    }
}
