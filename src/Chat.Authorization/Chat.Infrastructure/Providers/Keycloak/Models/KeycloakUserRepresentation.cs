using System.Text.Json.Serialization;

namespace Chat.Infrastructure.Providers.Keycloak.Models;

internal record KeycloakUserRepresentation(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("enabled")] bool Enabled
);
