using Chat.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

namespace Chat.Authorization.Extensions;

public static class ScalarExtensions
{
    public static void AddScalarDocumentation(this IServiceCollection services)
    {
        services.AddOptions<ScalarOptions>()
            .Configure<IOptions<IdentityProviderOptions>, IOptionsMonitor<IdentityProviderClientOptions>>((options, identityProviderConfig, clientConfig) =>
            {
                var identityProvider = identityProviderConfig.Value;
                var userClient = clientConfig.Get(IdentityProviderClientOptions.UserClient);

                options
                    .WithTitle("Chat Auth")
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                    .AddPreferredSecuritySchemes("KeycloakAuth")
                    .AddAuthorizationCodeFlow("KeycloakAuth", flow =>
                    {
                        flow.ClientId = userClient.ClientId;
                        flow.ClientSecret = userClient.ClientSecret;
                        flow.SelectedScopes = identityProvider.SelectedScopes;
                        
                        flow.Pkce = Enum.TryParse<Pkce>(identityProvider.Pkce, true, out var parsedPkce)
                            ? parsedPkce
                            : Pkce.No;
                    });
            });
    }
}
