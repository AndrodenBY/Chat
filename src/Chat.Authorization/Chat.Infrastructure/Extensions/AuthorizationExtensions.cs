using Chat.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Chat.Infrastructure.Extensions;

public static class AuthorizationExtensions
{
    public static void AddKeycloakAuth(this IServiceCollection services)
    {
        services.AddOptions<KeycloakOptions>()
            .BindConfiguration(KeycloakOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<KeycloakOptions>>((options, keycloakConfig) =>
            {
                var keycloakOptions = keycloakConfig.Value;

                options.Authority = keycloakOptions.Authority;
                options.Audience = keycloakOptions.ClientId;
                options.MetadataAddress = keycloakOptions.MetadataAddress;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = keycloakOptions.Issuer
                };
            });

        services.AddAuthorization();
    }
}
