using Chat.Application.Contracts;
using Chat.Application.Interfaces;
using Chat.Infrastructure.Helpers;
using Chat.Infrastructure.Options;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.Services;

public class KeycloakAuthorizationService(
    IHttpClientFactory clientFactory, 
    IOptions<IdentityProviderOptions> identityProviderOptions,
    IOptionsMonitor<IdentityProviderClientOptions> clientOptions) : IAuthorizationService
{
    private readonly HttpClient _httpClient = clientFactory.CreateClient("IdentityProvider");
    private readonly IdentityProviderOptions _identityProviderOptions = identityProviderOptions.Value;
    private readonly IdentityProviderClientOptions _userClientOptions = clientOptions.Get(IdentityProviderClientOptions.UserClient);  
    
    public async Task<ErrorOr<AuthorizationResponse>> Login(string username, string password, CancellationToken cancellationToken)
    {
        var formParameters = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _userClientOptions.ClientId,
            ["client_secret"] = _userClientOptions.ClientSecret,
            ["username"] = username,
            ["password"] = password
        };
        
        return await TokenHelper.SendTokenRequest(
            _httpClient, 
            _identityProviderOptions.TokenEndpoint, 
            formParameters, 
            cancellationToken
        );
    }

    public async Task<ErrorOr<AuthorizationResponse>> RefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["client_id"] = _userClientOptions.ClientId,
            ["client_secret"] = _userClientOptions.ClientSecret,
            ["refresh_token"] = refreshToken
        };
        
        return await TokenHelper.SendTokenRequest(
            _httpClient, 
            _identityProviderOptions.TokenEndpoint, 
            parameters, 
            cancellationToken
        );
    }

    public async Task<ErrorOr<Success>> Logout(string refreshToken, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = _userClientOptions.ClientId,
            ["client_secret"] = _userClientOptions.ClientSecret,
            ["refresh_token"] = refreshToken
        };

        var response = await _httpClient.PostAsync(
            _identityProviderOptions.LogoutEndpoint,
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
}
