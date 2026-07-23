namespace Chat.Infrastructure.Providers.Keycloak.Models;

internal sealed record KeycloakCredentials(
    string Type,
    string Value,
    bool Temporary);
