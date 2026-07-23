using Chat.Application.DTOs;
using Chat.Domain.ValueObjects;
using ErrorOr;

namespace Chat.Application.Interfaces;

public interface IUserManagementService
{
    Task<ErrorOr<UserDto>> Get(ExternalId externalId, CancellationToken cancellationToken);
    Task<ErrorOr<string>> Create(UserCreateDto createDto, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> Update(ExternalId externalId, UserUpdateDto updateDto, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> Delete(ExternalId externalId, CancellationToken cancellationToken);
}
