namespace Chat.Infrastructure.Providers.Keycloak.Models;

internal sealed record KeycloakCreateUserRequest(
    string Username,
    string Email,
    bool Enabled,
    KeycloakCredentials[] Credentials);
