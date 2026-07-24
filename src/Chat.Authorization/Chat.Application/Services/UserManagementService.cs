using Chat.Application.DTOs;
using Chat.Application.Interfaces;
using Chat.Domain.Interfaces;
using Chat.Domain.Models;
using Chat.Domain.ValueObjects;
using ErrorOr;

namespace Chat.Application.Services;

public class UserManagementService(IIdentityUserProvider identityUserProvider) : IUserManagementService
{
    public async Task<ErrorOr<UserDto>> Get(string externalId, CancellationToken cancellationToken)
    {
        var validationResult = ValidateExternalId(externalId);

        if (validationResult.IsError)
        {
            return validationResult.Errors;
        }

        var userData = await identityUserProvider.Get(
            validationResult.Value,
            cancellationToken);

        if (userData is null)
        {
            return Error.NotFound(
                "User.NotFound",
                "User with this id does not exist.");
        }

        return new UserDto(
            userData.ExternalId.Value, 
            userData.Username.Value, 
            userData.Email.Value, 
            userData.Enabled
        );
    }

    public async Task<ErrorOr<ExternalId>> Create(UserCreateDto createDto, CancellationToken cancellationToken)
    {
        var validationResult = ValidateIdentity(
            createDto.Username,
            createDto.Email);

        if (validationResult.IsError)
        {
            return validationResult.Errors;
        }

        if (string.IsNullOrWhiteSpace(createDto.Password))
        {
            return Error.Validation(
                "Auth.InvalidPassword",
                "Password cannot be empty.");
        }

        var (username, email) = validationResult.Value;

        return await identityUserProvider.Create(
            username,
            email,
            createDto.Password,
            cancellationToken);
    }

    public async Task<ErrorOr<Success>> Update(string externalId, UserUpdateDto updateDto, CancellationToken cancellationToken)
    {
        var idValidationResult = ValidateExternalId(externalId);

        if (idValidationResult.IsError)
        {
            return idValidationResult.Errors;
        }

        var validationResult = ValidateIdentity(
            updateDto.Username,
            updateDto.Email);

        if (validationResult.IsError)
        {
            return validationResult.Errors;
        }

        var (validUsername, validEmail) = validationResult.Value;

        var result = await identityUserProvider.Update(
            idValidationResult.Value,
            validUsername,
            validEmail,
            cancellationToken);

        return result.Status switch
        {
            OperationStatus.Success =>
                Result.Success,

            OperationStatus.NotFound =>
                Error.NotFound(
                    "User.NotFound",
                    result.ErrorMessage ?? "User doesn't exist."),

            OperationStatus.Conflict =>
                Error.Conflict(
                    "User.AlreadyExists",
                    result.ErrorMessage ?? "User already exists."),

            OperationStatus.Failure =>
                Error.Failure(
                    "User.Failed",
                    result.ErrorMessage ?? "Failed to update user."),

            _ =>
                Error.Failure(
                    "User.Unknown",
                    "Unknown user operation result.")
        };
    }

    public async Task<ErrorOr<Success>> Delete(string externalId, CancellationToken cancellationToken)
    {
        var idValidationResult = ValidateExternalId(externalId);
        if (idValidationResult.IsError)
        {
            return idValidationResult.Errors;
        }
        
        var result = await identityUserProvider.Delete(idValidationResult.Value, cancellationToken);

        return result.Status switch
        {
            OperationStatus.Success =>
                Result.Success,

            OperationStatus.Failure =>
                Error.Failure(
                    "User.Failed",
                    result.ErrorMessage ?? "Failed to update user."),

            _ =>
                Error.Failure(
                    "User.Unknown",
                    "Unknown user operation result.")
        };
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

    private static ErrorOr<ExternalId> ValidateExternalId(string externalId)
    {
        if (!ExternalId.TryCreate(externalId, out var validExternalId, out var externalIdError))
        {
            return Error.Validation("Auth.InvalidExternalId", externalIdError ?? "Invalid external id.");
        }
        
        return validExternalId!;
    }
}
