namespace Chat.Infrastructure.Providers.Keycloak.Models;

internal sealed record KeycloakUpdateUserRequest(
    string Username,
    string Email,
    bool Enabled);
