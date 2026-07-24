using Chat.Application.DTOs;
using Chat.Domain.ValueObjects;
using ErrorOr;

namespace Chat.Application.Interfaces;

public interface IUserManagementService
{
    Task<ErrorOr<UserDto>> Get(string externalId, CancellationToken cancellationToken);
    Task<ErrorOr<ExternalId>> Create(UserCreateDto createDto, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> Update(string externalId, UserUpdateDto updateDto, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> Delete(string externalId, CancellationToken cancellationToken);
}
