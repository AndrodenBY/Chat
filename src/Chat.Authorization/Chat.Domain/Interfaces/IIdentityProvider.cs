using Chat.Domain.Contracts;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.Interfaces;

public interface IIdentityProvider
{
    Task<TokenResult> Authenticate(Username username, string password,  CancellationToken cancellationToken);
    Task<TokenResult> RefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task RevokeSession(RefreshToken refreshToken, CancellationToken cancellationToken);
}
