using Chat.Application.DTOs;
using ErrorOr;

namespace Chat.Application.Interfaces;

public interface IUserService
{
    Task<ErrorOr<Guid>> CreateUser(UserCreateDto createDto, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> UpdateUser(UserUpdateDto updateDto, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> DeleteUser(Guid userId, CancellationToken cancellationToken);
}
