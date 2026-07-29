using Chat.Api.Contracts;
using Chat.Api.Extensions;
using Chat.Application.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Handlers;

public static class AuthEndpointHandler
{
    public static async Task<IResult> Login(
        [FromBody] LoginParameters request,
        IAuthenticationManager authManager,
        CancellationToken cancellationToken)
    {
        var result = await authManager.Login(
            request.Username,
            request.Password,
            cancellationToken);

        return result.ToApiResult();
    }

    public static async Task<IResult> RefreshToken(
        [FromBody] string refreshToken,
        IAuthenticationManager authManager,
        CancellationToken cancellationToken)
    {
        var result = await authManager.RefreshToken(refreshToken, cancellationToken);

        return result.ToApiResult();
    }

    public static async Task<IResult> Logout(
        [FromBody] string refreshToken,
        IAuthenticationManager authManager,
        CancellationToken cancellationToken)
    {
        var result = await authManager.Logout(refreshToken, cancellationToken);

        return result.Match(
            _ => Results.NoContent(),
            errors => errors.ToProblem()
        );
    }
}
