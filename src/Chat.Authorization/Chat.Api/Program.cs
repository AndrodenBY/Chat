using Chat.Api.Endpoints;
using Chat.Api.Middleware;
using Chat.Application;
using Chat.Infrastructure;
using Chat.Infrastructure.Options;
using Chat.Infrastructure.Options.Keycloak;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();
builder.Services.AddProblemDetails();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddOptions<ScalarOptions>()
    .Configure<IOptions<IdentityProviderOptions>, IOptions<KeycloakOptions>>(
        (options, identityProviderConfig, keycloakConfig) =>
        {
            var identityProvider = identityProviderConfig.Value;
            var keycloakOptions = keycloakConfig.Value;
            
            const string securitySchemeName = "OAuth2";

            options
                .WithTitle("Chat API Reference")
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                .AddPreferredSecuritySchemes(securitySchemeName)
                .AddAuthorizationCodeFlow(securitySchemeName, flow =>
                {
                    flow.ClientId = keycloakOptions.UserClient.ClientId;
                    flow.ClientSecret = keycloakOptions.UserClient.ClientSecret;
                    
                    flow.SelectedScopes = identityProvider.SelectedScopes;
                    
                    flow.Pkce = Enum.TryParse<Pkce>(keycloakOptions.Pkce, true, out var parsedPkce)
                        ? parsedPkce
                        : Pkce.No;
                });
        });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
    
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapUserManagementEndpoints();

app.Run();
