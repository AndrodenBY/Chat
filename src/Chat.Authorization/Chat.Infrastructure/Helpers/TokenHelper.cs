using System.Net;
using System.Net.Http.Json;
using Chat.Application.Contracts;
using Chat.Infrastructure.DTOs;
using Chat.Infrastructure.Options;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.Helpers;

public static class TokenHelper
{
    public static async Task<ErrorOr<AuthorizationResponse>> SendTokenRequest(
        HttpClient httpClient,
        string tokenEndpoint,
        Dictionary<string, string> parameters,
        CancellationToken cancellationToken)
    {
        var requestTime = DateTimeOffset.UtcNow;

        var response = await httpClient.PostAsync(
            tokenEndpoint,
            new FormUrlEncodedContent(parameters),
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized)
            {
                return Error.Validation(
                    code: "Auth.InvalidCredentials",
                    description: "Invalid credentials or token supplied"
                );
            }

            return Error.Failure(
                code: "Auth.ProviderError",
                description: $"Identity provider returned HTTP {(int)response.StatusCode}"
            );
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken);

        if (tokenResponse is null)
        {
            return Error.Failure(
                code: "Auth.NullResponse",
                description: "Identity provider returned an empty response body"
            );
        }

        return new AuthorizationResponse
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            AccessTokenExpiresIn = requestTime.AddSeconds(tokenResponse.ExpiresIn),
            RefreshTokenExpiresIn = requestTime.AddSeconds(tokenResponse.ExpiresIn)
        };
    }
