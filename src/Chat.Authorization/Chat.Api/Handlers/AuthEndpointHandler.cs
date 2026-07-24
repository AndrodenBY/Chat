using Chat.Api.Contracts;
using Chat.Domain.Interfaces;
using Chat.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Handlers;

public static class AuthEndpointHandler
{
    public static async Task Login(
        [FromBody] LoginParameters request,
        IIdentityProvider provider,
        CancellationToken cancellationToken)
    {
        await provider.Login(request.Username, request.Password, cancellationToken);
    }

    public static async Task RefreshToken(
        [FromBody] RefreshTokenParameters request,
        IIdentityProvider provider,
        CancellationToken cancellationToken)
    {
        var refreshToken = new RefreshToken(request.RefreshToken);
        await provider.RefreshToken(refreshToken, cancellationToken);
    }

    public static async Task Logout(
        [FromBody] LogoutParameters request,
        IIdentityProvider provider,
        CancellationToken cancellationToken)
    {
        var refreshToken = new RefreshToken(request.RefreshToken);
        await provider.Logout(refreshToken, cancellationToken);
    }
}
