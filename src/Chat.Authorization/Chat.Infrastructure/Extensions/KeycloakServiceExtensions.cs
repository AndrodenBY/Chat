using Chat.Domain.Interfaces;
using Chat.Infrastructure.Options.Keycloak;
using Chat.Infrastructure.Providers.Keycloak;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.Extensions;

public static class KeycloakServiceExtensions
{
    public static IServiceCollection AddKeycloakServices(this IServiceCollection services)
    {
        services.AddOptions<KeycloakOptions>()
            .BindConfiguration(KeycloakOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddMemoryCache();
        
        services.AddTransient<KeycloakAdminAuthHandler>();
        
        services.AddHttpClient<KeycloakTokenService>();
        services.AddHttpClient<IIdentityUserProvider, KeycloakUserManagementService>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

                var baseUrl = options.ManagementApiEndpoint.EndsWith('/')
                    ? options.ManagementApiEndpoint
                    : $"{options.ManagementApiEndpoint}/";

                client.BaseAddress = new Uri(baseUrl);
            })
            .AddHttpMessageHandler<KeycloakAdminAuthHandler>();
        
        services.AddScoped<IIdentityProvider, KeycloakIdentityProvider>();

        return services;
    }
}
