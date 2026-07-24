using Chat.Application.Interfaces;
using Chat.Domain.Interfaces;
using Chat.Infrastructure.Options.Keycloak;
using Chat.Infrastructure.Providers.Keycloak;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddHttpClient(nameof(KeycloakTokenService));
        services.AddHttpClient(nameof(KeycloakUserManagementService));
        
        services.AddSingleton<KeycloakTokenService>()
            .AddScoped<IIdentityProvider, KeycloakIdentityProvider>()
            .AddScoped<IIdentityUserProvider, KeycloakUserManagementService>();

        return services;
    }
}
