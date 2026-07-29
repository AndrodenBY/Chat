using Chat.Domain.Contracts;
using ErrorOr;

namespace Chat.Application.Interfaces;

public interface IAuthenticationManager
{
    Task<ErrorOr<TokenResponse>> Login(string username, string password, CancellationToken cancellationToken);
    Task<ErrorOr<TokenResponse>> RefreshToken(string refreshToken, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> Logout(string refreshToken, CancellationToken cancellationToken);
}
