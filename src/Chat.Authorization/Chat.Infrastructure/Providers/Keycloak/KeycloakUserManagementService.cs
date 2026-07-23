using System.Net;
using System.Net.Http.Json;
using Chat.Application.DTOs;
using Chat.Application.Interfaces;
using Chat.Domain.ValueObjects;
using Chat.Infrastructure.DTOs;
using Chat.Infrastructure.Options;
using Chat.Infrastructure.Providers.Keycloak;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.Services;

public class KeycloakUserManagementService(
    IHttpClientFactory clientFactory,
    KeycloakTokenService tokenService,
    IOptions<IdentityProviderOptions> identityProviderOptions,
    IOptionsMonitor<IdentityProviderClientOptions> adminClientOptions) 
    : IUserManagementService
{
    private readonly HttpClient _httpClient = clientFactory.CreateClient(nameof(KeycloakUserManagementService));
    private readonly IdentityProviderOptions _identityProviderOptions = identityProviderOptions.Value;
    private readonly IdentityProviderClientOptions _adminClientOptions = adminClientOptions.Get(IdentityProviderClientOptions.AdminClient);
    
    public async Task<ErrorOr<UserDto>> Get(ExternalId externalId, CancellationToken cancellationToken)
    {
        var tokenResult = await tokenService.GetAdminToken(_adminClientOptions, cancellationToken);
        if (tokenResult.IsError)
        {
            return tokenResult.Errors;
        }
        
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{_identityProviderOptions.ManagementApiEndpoint}/{externalId.Value}");
        tokenService.AuthorizeRequest(request, tokenResult.Value);
        
        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return Error.NotFound("User.NotFound", "User with this id doesn't exist");
        }

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure("User.Failed", "Failed to retrieve user details");
        }

        var user = await response.Content
            .ReadFromJsonAsync<KeycloakUserRepresentation>(cancellationToken);

        if (user is null)
        {
            return Error.Failure("User.NullData", "Identity provider returned empty user");
        }
        
        if (!Username.TryCreate(user.Username, out var username, out var usernameError))
        {
            return Error.Validation("User.InvalidUsername", usernameError ?? "Invalid username returned from provider");
        }

        if (!Email.TryCreate(user.Email, out var email, out var emailError))
        {
            return Error.Validation("User.InvalidEmail", emailError ?? "Invalid email returned from provider");
        }

        return new UserDto(
            externalId,
            username!,
            email!,
            user.Enabled
        );
    }

    public async Task<ErrorOr<string>> Create(UserCreateDto createDto, CancellationToken cancellationToken)
    {
        if (!Username.TryCreate(createDto.Username.Value, out var username, out var usernameError))
        {
            return Error.Validation("User.InvalidUsername", usernameError ?? "Invalid username");
        }

        if (!Email.TryCreate(createDto.Email.Value, out var email, out var emailError))
        {
            return Error.Validation("User.InvalidEmail", emailError ?? "Invalid email");
        }

        if (string.IsNullOrWhiteSpace(createDto.Password))
        {
            return Error.Validation("User.InvalidPassword", "Password cannot be empty");
        }
        
        var tokenResult = await tokenService.GetAdminToken(_adminClientOptions, cancellationToken);
        if (tokenResult.IsError)
        {
            return tokenResult.Errors;
        }
        
        using var request = new HttpRequestMessage(HttpMethod.Post, _identityProviderOptions.ManagementApiEndpoint);
        tokenService.AuthorizeRequest(request, tokenResult.Value);
        request.Content = JsonContent.Create(new
        {
            username = username!.Value,
            email = email!.Value,
            enabled = true,
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = createDto.Password,
                    temporary = false
                }
            }
        });
        
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        
        if (response.StatusCode is HttpStatusCode.Conflict)
        {
            return Error.Conflict("User.AlreadyExists", "A user with this username already exists");
        }

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure("User.Failed", "Failed to create user");
        }
        
        var locationHeader = response.Headers.Location?.ToString();
        var createdUserId = locationHeader?.Split('/').LastOrDefault();

        if (!ExternalId.TryCreate(createdUserId, out var externalId, out var idError))
        {
            return Error.Failure("User.InvalidIdFromProvider", idError ?? "Provider returned an invalid user ID");
        }

        return externalId!.Value;
    }

    public async Task<ErrorOr<Success>> Update(ExternalId externalId, UserUpdateDto updateDto, CancellationToken cancellationToken)
    {
        if (!Username.TryCreate(updateDto.Username.Value, out var username, out var usernameError))
        {
            return Error.Validation("User.InvalidUsername", usernameError ?? "Invalid username");
        }

        if (!Email.TryCreate(updateDto.Email.Value, out var email, out var emailError))
        {
            return Error.Validation("User.InvalidEmail", emailError ?? "Invalid email");
        }
        
        var tokenResult = await tokenService.GetAdminToken(_adminClientOptions, cancellationToken);
        if (tokenResult.IsError)
        {
            return tokenResult.Errors;
        }

        using var request = new HttpRequestMessage(HttpMethod.Put, $"{_identityProviderOptions.ManagementApiEndpoint}/{externalId.Value}");
        tokenService.AuthorizeRequest(request, tokenResult.Value);
        request.Content = JsonContent.Create(new
        {
            username = username!.Value,
            email = email!.Value,
            enabled = true
        });
        
        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return Error.NotFound(
                code: "User.NotFound",
                description: "User with this id doesn't exist"
            );
        }

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure(
                code: "User.Failed",
                description: "Failed to update user details"
            );
        }

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> Delete(ExternalId externalId, CancellationToken cancellationToken)
    {
        var tokenResult = await tokenService.GetAdminToken(_adminClientOptions, cancellationToken);
        if (tokenResult.IsError)
        {
            return tokenResult.Errors;
        }
        
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"{_identityProviderOptions.ManagementApiEndpoint}/{externalId.Value}");
        tokenService.AuthorizeRequest(request, tokenResult.Value);
        
        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure(
                code: "User.Failed",
                description: "Failed to update user details"
            );
        }

        return Result.Success;
    }
    
    private async Task<ErrorOr<HttpRequestMessage>> CreateAuthorizedRequest(HttpMethod method, string endpoint, CancellationToken cancellationToken)
    {
        var tokenResult = await tokenService.GetAdminToken(_adminClientOptions, cancellationToken);
        if (tokenResult.IsError)
        {
            return tokenResult.Errors;
        }

        var request = new HttpRequestMessage(method, endpoint);
        tokenService.AuthorizeRequest(request, tokenResult.Value);

        return request;
    }
    
    private static ErrorOr<(Username Username, Email Email)> ValidateIdentity(string usernameValue, string emailValue)
    {
        if (!Username.TryCreate(usernameValue, out var username, out var usernameError))
        {
            return Error.Validation("User.InvalidUsername", usernameError ?? "Invalid username.");
        }

        if (!Email.TryCreate(emailValue, out var email, out var emailError))
        {
            return Error.Validation("User.InvalidEmail", emailError ?? "Invalid email.");
        }

        return (username!, email!);
    }
}
