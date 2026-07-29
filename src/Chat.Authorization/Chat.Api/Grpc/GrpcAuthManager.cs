using Chat.Api.Extensions;
using Chat.Application.Interfaces;
using Chat.Grpc;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using DomainTokenResponse = Chat.Domain.Contracts.TokenResponse;
using GrpcTokenResponse = Chat.Grpc.TokenResponse;

namespace Chat.Api.Grpc;

public class GrpcAuthManager(IAuthenticationManager authManager) : AuthManager.AuthManagerBase
{
    public override async Task<GrpcTokenResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var result = await authManager.Login(
            request.Username,
            request.Password,
            context.CancellationToken
        );

        return result.Match(
            ToGrpcTokenResponse,
            errors => throw errors.ToRpcException()
        );
    }

    public override async Task<GrpcTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        var result = await authManager.RefreshToken(request.RefreshToken, context.CancellationToken);

        return result.Match(
            ToGrpcTokenResponse,
            errors => throw errors.ToRpcException()
        );
    }

    public override async Task<Empty> Logout(LogoutRequest request, ServerCallContext context)
    {
        var result = await authManager.Logout(request.RefreshToken, context.CancellationToken);

        return result.Match(
            _ => new Empty(),
            errors => throw errors.ToRpcException()
        );
    }

    private static GrpcTokenResponse ToGrpcTokenResponse(DomainTokenResponse tokenResponse) =>
        new()
        {
            AccessToken = tokenResponse.AccessToken.Value,
            RefreshToken = tokenResponse.RefreshToken.Value,
            AccessTokenExpiresAt = Timestamp.FromDateTimeOffset(tokenResponse.AccessTokenExpiresAt),
            RefreshTokenExpiresAt = Timestamp.FromDateTimeOffset(tokenResponse.RefreshTokenExpiresAt)
        };
}
