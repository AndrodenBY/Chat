using Chat.Domain.Contracts;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.Interfaces;

public interface IIdentityProvider
{
    Task<TokenResponse> Login(Username username, string password,  CancellationToken cancellationToken);
    Task<TokenResponse> RefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task Logout(RefreshToken refreshToken, CancellationToken cancellationToken);
}
