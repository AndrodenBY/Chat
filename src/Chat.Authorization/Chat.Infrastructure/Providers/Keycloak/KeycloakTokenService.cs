using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Chat.Domain.Contracts;
using Chat.Domain.ValueObjects;
using Chat.Infrastructure.Options;
using Chat.Infrastructure.Options.Keycloak;
using Chat.Infrastructure.Providers.Keycloak.Models;
using ErrorOr;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.Providers.Keycloak;

public class KeycloakTokenService(
    HttpClient httpClient, 
    IOptions<KeycloakOptions> keycloakOptions,
    IMemoryCache cache)
{
    private readonly KeycloakOptions _keycloakOptions = keycloakOptions.Value;
    private const string AdminTokenCacheKey = "keycloak_admin_token";
    
    public async Task<ErrorOr<TokenResponse>> ExchangeToken(Dictionary<string, string> parameters, CancellationToken cancellationToken)
    {
        var requestTime = DateTimeOffset.UtcNow;

        using var response = await httpClient.PostAsync(
            _keycloakOptions.TokenEndpoint,
            new FormUrlEncodedContent(parameters),
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode is HttpStatusCode.Unauthorized)
            {
                return Error.Validation(
                    code: "Auth.InvalidCredentials",
                    description: "Invalid username or password"
                );
            }
            
            if (response.StatusCode is HttpStatusCode.BadRequest)
            {
                return Error.Validation(
                    code: "Auth.TokenExpiredOrInvalid",
                    description: "The provider gran or token is invalid or expired"
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

        if (!AccessToken.TryCreate(tokenResponse.AccessToken, out var accessToken, out var accessError))
        {
            return Error.Validation("Token.AccessTokenInvalid", accessError ?? "Invalid access token");
        }

        if (!RefreshToken.TryCreate(tokenResponse.RefreshToken, out var refreshToken, out var refreshError))
        {
            return Error.Validation("Token.RefreshTokenInvalid", refreshError ?? "Invalid refresh token");
        }

        return new TokenResponse(
            AccessToken: accessToken!,
            RefreshToken: refreshToken!,
            AccessTokenExpiresAt: requestTime.AddSeconds(tokenResponse.ExpiresIn),
            RefreshTokenExpiresAt: requestTime.AddSeconds(tokenResponse.RefreshExpiresIn)
        );
    }
    
    public async Task<ErrorOr<string>> GetAdminToken(IdentityProviderClientOptions adminClientOptions, CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(AdminTokenCacheKey, out string? cachedToken) && cachedToken is not null)
        {
            return cachedToken;
        }

        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = adminClientOptions.ClientId,
            ["client_secret"] = adminClientOptions.ClientSecret
        };
        
        using var response = await httpClient.PostAsync(
            _keycloakOptions.TokenEndpoint,
            new FormUrlEncodedContent(parameters),
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure(
                code: "Auth.AdminTokenFailed",
                description: "Could not obtain administrative token");
        }

        var json = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken);

        if (json?.AccessToken is null)
        {
            return Error.Failure(
                code: "Auth.NullResponse",
                description: "Identity provider returned an empty response body");
        }
            
        var cacheExpiry = TimeSpan.FromSeconds(json.ExpiresIn * 0.8);
        cache.Set(AdminTokenCacheKey, json.AccessToken, cacheExpiry);

        return json.AccessToken;
    }
    
    public async Task<ErrorOr<Success>> RevokeToken(Dictionary<string, string> parameters, CancellationToken cancellationToken)
    {
        using var response = await httpClient.PostAsync(
            _keycloakOptions.LogoutEndpoint,
            new FormUrlEncodedContent(parameters),
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure(
                code: "Auth.LogoutFailed",
                description: "Failed to revoke session with the identity provider"
            );
        }

        return Result.Success;
    }

    public void AuthorizeRequest(HttpRequestMessage request, string token)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
