using ErrorOr;
using Chat.Application.DTOs;
using Chat.Domain.Contracts;

namespace Chat.Application.Interfaces;

public interface IAuthorizationService
{
    Task<ErrorOr<TokenResult>> Login(LoginRequest request, CancellationToken cancellationToken);
    Task<ErrorOr<TokenResult>> RefreshToken(string refreshToken, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> Logout(string refreshToken, CancellationToken cancellationToken);
}
