using System.Net;
using System.Net.Http.Json;
using Chat.Domain.Interfaces;
using Chat.Domain.Models;
using Chat.Domain.ValueObjects;
using Chat.Infrastructure.Providers.Keycloak.Models;

namespace Chat.Infrastructure.Providers.Keycloak;

public class KeycloakUserManagementService(HttpClient httpClient) : IIdentityUserProvider
{
    public async Task<IdentityUserData?> Get(ExternalId externalId, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(externalId.Value, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<KeycloakUserRepresentation>(cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("Keycloak returned an empty user payload.");
        }

        if (!Username.TryCreate(user.Username, out var username, out var usernameError))
        {
            throw new InvalidOperationException(usernameError ?? "Keycloak returned an invalid username.");
        }

        if (!Email.TryCreate(user.Email, out var email, out var emailError))
        {
            throw new InvalidOperationException(emailError ?? "Keycloak returned an invalid email.");
        }

        return new IdentityUserData(
            externalId,
            username!,
            email!,
            user.Enabled
        );
    }

    public async Task<ExternalId> Create(Username username, Email email, string password, CancellationToken cancellationToken)
    {
        var payload = new KeycloakCreateUserRequest(
                username.Value,
                email.Value,
                true,
                [
                    new KeycloakCredentials(
                        "password",
                        password,
                        false)
                ]);

        using var response = await httpClient.PostAsJsonAsync("", payload, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            throw new InvalidOperationException("A Keycloak user with this username or email already exists.");
        }

        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location;

        var userId = location?
            .AbsolutePath
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .LastOrDefault();

        return !ExternalId.TryCreate(userId, out var externalId, out var error) 
            ? throw new InvalidOperationException(error ?? "Keycloak returned an invalid user ID.")
            : externalId!;
    }

    public async Task<OperationResult> Update(ExternalId externalId, Username username, Email email, CancellationToken cancellationToken)
    {
        var payload =
            new KeycloakUpdateUserRequest(
                username.Value,
                email.Value,
                true
        );

        using var response = await httpClient.PutAsJsonAsync(externalId.Value, payload, cancellationToken);

        return response.StatusCode switch
        {
            HttpStatusCode.Conflict => new OperationResult(
                OperationStatus.Conflict,
                "A user with this username or email already exists."),

            _ when response.IsSuccessStatusCode => new OperationResult(
                OperationStatus.Success),

            _ => new OperationResult(
                OperationStatus.Failure,
                $"Keycloak update failed with status code {response.StatusCode}.")
        };
    }

    public async Task<OperationResult> Delete(ExternalId externalId, CancellationToken cancellationToken)
    {
        using var response = await httpClient.DeleteAsync(externalId.Value, cancellationToken);

        return response.StatusCode switch
        {
            HttpStatusCode.NotFound => new OperationResult(
                OperationStatus.NotFound,
                "User with this id doesn't exist."),

            _ when response.IsSuccessStatusCode => new OperationResult(
                OperationStatus.Success),

            _ => new OperationResult(
                OperationStatus.Failure,
                $"Keycloak delete failed with status code {response.StatusCode}.")
        };
    }
}
