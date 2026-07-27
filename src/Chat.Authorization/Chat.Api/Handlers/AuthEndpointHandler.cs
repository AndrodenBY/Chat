using Chat.Api.Contracts;
using Chat.Api.Extensions;
using Chat.Application.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Handlers;

public static class AuthEndpointHandler
{
    public static async Task<IResult> Login(
        [FromBody] LoginParameters request,
        IAuthenticationService authService,
        CancellationToken cancellationToken)
    {
        var result = await authService.Login(
            request.Username,
            request.Password,
            cancellationToken);

        return result.ToApiResult();
    }

    public static async Task<IResult> RefreshToken(
        [FromBody] string refreshToken,
        IAuthenticationService authService,
        CancellationToken cancellationToken)
    {
        var result = await authService.RefreshToken(refreshToken, cancellationToken);

        return result.ToApiResult();
    }

    public static async Task<IResult> Logout(
        [FromBody] string refreshToken,
        IAuthenticationService authService,
        CancellationToken cancellationToken)
    {
        var result = await authService.Logout(refreshToken, cancellationToken);

        return result.Match(
            _ => Results.NoContent(),
            errors => errors.ToProblem()
        );
    }
}
