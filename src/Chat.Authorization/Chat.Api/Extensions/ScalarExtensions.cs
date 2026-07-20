using Chat.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

namespace Chat.Authorization.Extensions;

public static class ScalarExtensions
{
    public static void AddScalarDocumentation(this IServiceCollection services)
    {
        services.AddOptions<ScalarOptions>()
            .Configure<IOptions<KeycloakOptions>>((options, keycloakConfig) =>
            {
                var keycloakOptions = keycloakConfig.Value;

                options
                    .WithTitle("Chat Auth")
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                    .AddPreferredSecuritySchemes("KeycloakAuth")
                    .AddAuthorizationCodeFlow("KeycloakAuth", flow =>
                    {
                        flow.ClientId = keycloakOptions.ClientId;
                        flow.ClientSecret = keycloakOptions.ClientSecret;
                        flow.SelectedScopes = keycloakOptions.SelectedScopes;
                        
                        flow.Pkce = Enum.TryParse<Pkce>(keycloakOptions.Pkce, true, out var parsedPkce)
                            ? parsedPkce
                            : Pkce.No;
                    });
            });
    }
}
