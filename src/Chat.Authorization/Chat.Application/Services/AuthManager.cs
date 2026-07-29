using Chat.Application.Interfaces;
using Chat.Domain.Contracts;
using Chat.Domain.Interfaces;
using Chat.Domain.ValueObjects;
using ErrorOr;

namespace Chat.Application.Services;

public class AuthManager(IIdentityProvider identityProvider) : IAuthenticationManager
{
    public async Task<ErrorOr<TokenResponse>> Login(string username, string password, CancellationToken cancellationToken)
    {
        if (!Username.TryCreate(username, out var validUsername, out var usernameError))
        {
            return Error.Validation("User.InvalidUsername", usernameError ?? "Invalid username.");
        }
        
        return await identityProvider.Login(validUsername!, password, cancellationToken);
    }

    public async Task<ErrorOr<TokenResponse>> RefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        if (!Domain.ValueObjects.RefreshToken.TryCreate(refreshToken, out var validRefreshToken, out var refreshError))
        {
            return Error.Validation("Token.RefreshTokenInvalid", refreshError ?? "Invalid refresh token.");
        } 
        
        return await identityProvider.RefreshToken(validRefreshToken!, cancellationToken);
    }

    public async Task<ErrorOr<Success>> Logout(string refreshToken, CancellationToken cancellationToken)
    {
        if (!Domain.ValueObjects.RefreshToken.TryCreate(refreshToken, out var validRefreshToken, out var refreshError))
        {
            return Error.Validation("Token.RefreshTokenInvalid", refreshError ?? "Invalid refresh token.");
        }
        
        await identityProvider.Logout(validRefreshToken!, cancellationToken);

        return Result.Success;
    }
}
