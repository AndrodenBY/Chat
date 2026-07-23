using System.Net;
using System.Net.Http.Json;
using Chat.Application.DTOs;
using Chat.Application.Interfaces;
using Chat.Domain.ValueObjects;
using Chat.Infrastructure.Options.Keycloak;
using Chat.Infrastructure.Providers.Keycloak.Models;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.Providers.Keycloak;

public class KeycloakUserManagementService(
    IHttpClientFactory clientFactory,
    KeycloakTokenService tokenService,
    IOptions<KeycloakOptions> keycloakOptions) 
    : IUserManagementService
{
    private readonly HttpClient _httpClient = clientFactory.CreateClient(nameof(KeycloakUserManagementService));
    private readonly KeycloakOptions _keycloakOptions = keycloakOptions.Value;
    
    public async Task<ErrorOr<UserDto>> Get(ExternalId externalId, CancellationToken cancellationToken)
    {
        var requestResult = await CreateAuthorizedRequest(
            HttpMethod.Get,
            $"{_keycloakOptions.ManagementApiEndpoint}/{externalId.Value}",
            cancellationToken
        );

        if (requestResult.IsError)
        {
            return requestResult.Errors;
        }
        
        using var response = await _httpClient.SendAsync(requestResult.Value, cancellationToken);
        
        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return Error.NotFound("User.NotFound", "User with this id doesn't exist.");
        }

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure("User.Failed", "Failed to retrieve user details.");
        }
        
        var user = await response.Content.ReadFromJsonAsync<KeycloakUserRepresentation>(cancellationToken);
        if (user is null)
        {
            return Error.Failure("User.NullData", "Identity provider returned an empty user payload.");
        }
        
        var validationResult = ValidateIdentity(user.Username, user.Email);
        if (validationResult.IsError) return validationResult.Errors;

        var (username, email) = validationResult.Value;

        return new UserDto(
            externalId,
            username,
            email,
            user.Enabled
        );
    }

    public async Task<ErrorOr<string>> Create(UserCreateDto createDto, CancellationToken cancellationToken)
    {
        var validationResult = ValidateIdentity(createDto.Username.Value, createDto.Email.Value);
        if (validationResult.IsError)
        {
            return validationResult.Errors;
        }
        
        if (string.IsNullOrWhiteSpace(createDto.Password))
        {
            return Error.Validation("User.InvalidPassword", "Password cannot be empty");
        }
        
        var (username, email) = validationResult.Value;

        var requestResult = await CreateAuthorizedRequest(
            HttpMethod.Post,
            _keycloakOptions.ManagementApiEndpoint,
            cancellationToken
        );
        
        if (requestResult.IsError)
        {
            return requestResult.Errors;
        }

        using var request = requestResult.Value;
        request.Content = JsonContent.Create(new KeycloakCreateUserRequest(
            username.Value,
            email.Value,
            true,
            [
                new KeycloakCredentials(
                    "password", 
                    createDto.Password, 
                    false
                )
            ]
        ));
        
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
        var validationResult = ValidateIdentity(updateDto.Username.Value, updateDto.Email.Value);
        if (validationResult.IsError)
        {
            return validationResult.Errors;
        }

        var (username, email) = validationResult.Value;

        var requestResult = await CreateAuthorizedRequest(
            HttpMethod.Put, 
            $"{_keycloakOptions.ManagementApiEndpoint}/{externalId.Value}", 
            cancellationToken
        );

        if (requestResult.IsError)
        {
            return requestResult.Errors;
        }

        using var request = requestResult.Value;
        request.Content = JsonContent.Create(new KeycloakUpdateUserRequest(
            username.Value,
            email.Value,
            true
        ));

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return Error.NotFound("User.NotFound", "User with this id doesn't exist.");
        }

        if (response.StatusCode is HttpStatusCode.Conflict)
        {
            return Error.Conflict("User.AlreadyExists", "A user with this username or email already exists.");
        }

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure("User.Failed", "Failed to update user details.");
        }

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> Delete(ExternalId externalId, CancellationToken cancellationToken)
    {
        var requestResult = await CreateAuthorizedRequest(
            HttpMethod.Delete, 
            $"{_keycloakOptions.ManagementApiEndpoint}/{externalId.Value}", 
            cancellationToken
        );

        if (requestResult.IsError)
        {
            return requestResult.Errors;
        }

        using var request = requestResult.Value;
        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return Error.NotFound("User.NotFound", "User with this id doesn't exist.");
        }

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure("User.Failed", "Failed to delete user."); // Fixed copy-paste error!
        }

        return Result.Success;
    }
    
    private async Task<ErrorOr<HttpRequestMessage>> CreateAuthorizedRequest(HttpMethod method, string endpoint, CancellationToken cancellationToken)
    {
        var tokenResult = await tokenService.GetAdminToken(_keycloakOptions.AdminClient, cancellationToken);
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
