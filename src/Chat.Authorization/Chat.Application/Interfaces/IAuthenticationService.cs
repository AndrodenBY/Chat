using Chat.Domain.Contracts;
using ErrorOr;

namespace Chat.Application.Interfaces;

public interface IAuthenticationService
{
    Task<ErrorOr<TokenResult>> Login(string username, string password, CancellationToken cancellationToken);
    Task<ErrorOr<TokenResult>> RefreshToken(string refreshToken, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> Logout(string refreshToken, CancellationToken cancellationToken);
}
