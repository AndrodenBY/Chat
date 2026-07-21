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
        services.AddOptions<IdentityProviderOptions>()
            .BindConfiguration(IdentityProviderOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<IdentityProviderClientOptions>()
            .BindConfiguration($"{IdentityProviderOptions.SectionName}:UserClient")
            .ValidateDataAnnotations()
            .ValidateOnStart(); 
        
        services.AddOptions<IdentityProviderClientOptions>()
            .BindConfiguration($"{IdentityProviderOptions.SectionName}:AdminClient")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<IdentityProviderOptions>>((options, identityProviderConfig) =>
            {
                var identityProviderOptions = identityProviderConfig.Value;

                options.Authority = identityProviderOptions.Authority;
                options.Audience = identityProviderOptions.Audience;
                options.MetadataAddress = identityProviderOptions.MetadataAddress;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = identityProviderOptions.Issuer,
                    
                    ValidateAudience = true,
                    ValidAudience = identityProviderOptions.Audience,
                };
            });

        services.AddAuthorization();
    }
}
