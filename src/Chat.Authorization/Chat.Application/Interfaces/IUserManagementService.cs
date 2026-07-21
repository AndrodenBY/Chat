using Chat.Application.DTOs;
using ErrorOr;

namespace Chat.Application.Interfaces;

public interface IUserManagementService
{
    Task<ErrorOr<string>> Get(string externalId, CancellationToken cancellationToken);
    Task<ErrorOr<string>> Create(UserCreateDto createDto, CancellationToken cancellationToken);
    Task<ErrorOr<string>> Update(UserUpdateDto updateDto, CancellationToken cancellationToken);
    Task<ErrorOr<string>> Delete(string externalId, CancellationToken cancellationToken);
}
