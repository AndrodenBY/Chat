namespace Chat.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapIdentityProviderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth");
        
        group.RequireAuthorization();
        
        group.MapPost("/login", )
        
        return endpoints;
        
    }
}
