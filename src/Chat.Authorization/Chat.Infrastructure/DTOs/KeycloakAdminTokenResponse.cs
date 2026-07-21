using System.Text.Json.Serialization;

namespace Chat.Infrastructure.DTOs;

internal record KeycloakAdminTokenResponse(
    [property: JsonPropertyName("access_token")]
    string AccessToken
);
