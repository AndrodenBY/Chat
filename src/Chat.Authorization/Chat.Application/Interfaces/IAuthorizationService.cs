using ErrorOr;
using Chat.Application.Contracts;

namespace Chat.Application.Interfaces;

public interface IAuthorizationService
{
    Task<ErrorOr<AuthorizationResponse>> Login(string username, string password, CancellationToken cancellationToken);
    Task<ErrorOr<AuthorizationResponse>> RefreshToken(string refreshToken, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> Logout(string refreshToken, CancellationToken cancellationToken);
}
