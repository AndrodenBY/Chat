using System.Net;
using System.Net.Http.Json;
using Chat.Application.DTOs;
using Chat.Application.Interfaces;
using Chat.Infrastructure.DTOs;
using Chat.Infrastructure.Options;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.Services;

public class KeycloakUserManagementService(
    IManagementApiClient managementApiClient,
    IOptions<IdentityProviderOptions> identityProviderOptions) 
    : IUserManagementService
{
    private readonly IdentityProviderOptions _identityProviderOptions = identityProviderOptions.Value;
    
    public async Task<ErrorOr<UserDto>> Get(string externalId, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{_identityProviderOptions.ManagementApiEndpoint}/{externalId}");
        
        var responseResult =  await managementApiClient.SendRequest(request, cancellationToken);
        using var response = responseResult.Value;

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
                description: "Failed to retrieve user details"
            );
        }
        
        var user = await response.Content.ReadFromJsonAsync<KeycloakUserRepresentation>(cancellationToken);

        if (user is null)
        {
            return Error.Failure(
                code: "User.NullData",
                description: "Identity provider returned empty user"
            );
        }

        return new UserDto(externalId, user.Username, user.Email, true);
    }

    public async Task<ErrorOr<string>> Create(UserCreateDto createDto, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, _identityProviderOptions.ManagementApiEndpoint);
        request.Content = JsonContent.Create(createDto);

        var responseResult = await managementApiClient.SendRequest(request, cancellationToken);
        using var response = responseResult.Value;
        
        if (response.StatusCode is HttpStatusCode.Conflict)
        {
            return Error.Conflict(
                code: "User.AlreadyExists",
                description: "A user with this username already exists"
            );
        }

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure(
                code: "User.Failed",
                description: "Failed to create user"
            );
        }
        
        var locationHeader = response.Headers.Location?.ToString();
        var createdUserId = locationHeader?.Split('/').LastOrDefault();

        if (createdUserId is null)
        {
            return Error.Failure(
                code: "User.MissingId",
                description: "User created but failed to retrieve user id"
            );
        }

        return createdUserId;
    }

    public async Task<ErrorOr<Success>> Update(string externalId, UserUpdateDto updateDto, CancellationToken cancellationToken)
    {
        var payload = new KeycloakUserRepresentation(
            updateDto.Username,
            updateDto.Email,
            true
        );

        using var request = new HttpRequestMessage(HttpMethod.Put, $"{_identityProviderOptions.ManagementApiEndpoint}/{externalId}");
        request.Content = JsonContent.Create(payload);
        
        var responseResult = await managementApiClient.SendRequest(request, cancellationToken);
        using var response = responseResult.Value;

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

    public async Task<ErrorOr<Success>> Delete(string externalId, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"{_identityProviderOptions.ManagementApiEndpoint}/{externalId}");
        
        var responseResult = await managementApiClient.SendRequest(request, cancellationToken);
        using var response = responseResult.Value;

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure(
                code: "User.Failed",
                description: "Failed to update user details"
            );
        }

        return Result.Success;
    }
}
