using System.Text.Json.Serialization;

namespace Chat.Infrastructure.DTOs;

internal record KeycloakTokenResponse(
    [property: JsonPropertyName("access_token")]
    string AccessToken,
    [property: JsonPropertyName("refresh_token")]
    string RefreshToken,
    [property: JsonPropertyName("expires_in")]
    int ExpiresIn,
    [property: JsonPropertyName("refresh_expires_in")]
    int RefreshExpiresIn
);
