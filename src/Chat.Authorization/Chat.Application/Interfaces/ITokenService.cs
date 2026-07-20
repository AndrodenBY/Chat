using Chat.Application.Contracts;
using ErrorOr;

namespace Chat.Application.Interfaces;

public interface ITokenService
{
    Task<ErrorOr<string>> GetAdminToken(CancellationToken cancellationToken);
    Task<ErrorOr<TokenResult>> GetUserInfo(string username, string password, CancellationToken cancellationToken);
    Task<ErrorOr<TokenResult>> RefreshToken(string refreshToken, CancellationToken cancellationToken);
}
