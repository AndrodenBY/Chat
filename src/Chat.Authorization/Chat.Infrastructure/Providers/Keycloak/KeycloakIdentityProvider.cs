using Chat.Domain.Contracts;
using Chat.Domain.Interfaces;
using Chat.Domain.ValueObjects;
using Chat.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.Providers.Keycloak;

public class KeycloakIdentityProvider(
    KeycloakTokenService tokenService, 
    IOptionsMonitor<IdentityProviderClientOptions> clientOptions
    ) : IIdentityProvider
{
    private readonly IdentityProviderClientOptions _userClientOptions = clientOptions.Get(IdentityProviderClientOptions.UserClient);  
    
    public async Task<TokenResult> Login(Username username, string password, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _userClientOptions.ClientId,
            ["client_secret"] = _userClientOptions.ClientSecret,
            ["username"] = username.Value,
            ["password"] = password
        };
        
        var result = await tokenService.ExchangeToken(parameters, cancellationToken);
        
        return result.Match(
            tokenResult => tokenResult,
            errors => throw new InvalidOperationException($"Login failed: {errors.First().Description}")
        );
    }

    public async Task<TokenResult> RefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["client_id"] = _userClientOptions.ClientId,
            ["client_secret"] = _userClientOptions.ClientSecret,
            ["refresh_token"] = refreshToken.Value
        };
        
        var result = await tokenService.ExchangeToken(parameters, cancellationToken);
        
        return result.Match(
            tokenResult => tokenResult,
            errors => throw new InvalidOperationException($"Refresh failed: {errors.First().Description}")
        );
    }

    public async Task Logout(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = _userClientOptions.ClientId,
            ["client_secret"] = _userClientOptions.ClientSecret,
            ["refresh_token"] = refreshToken.Value
        };

        var result = await tokenService.RevokeToken(parameters, cancellationToken);

        result.Switch(
            _ => { },
            errors => throw new InvalidOperationException($"Logout failed: {errors.First().Description}")
        );
    }
}
