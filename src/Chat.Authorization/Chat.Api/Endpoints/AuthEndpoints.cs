using Chat.Api.Handlers;
using Chat.Domain.Contracts;

namespace Chat.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth");

        group.MapPost("/login", AuthEndpointHandler.Login)
            .Produces<TokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();
        
        group.MapPost("/refresh-token", AuthEndpointHandler.RefreshToken)
            .Produces<TokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();
        
        group.MapPost("/logout", AuthEndpointHandler.Logout)
            .Produces<TokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
        
        return endpoints;
    }
}
