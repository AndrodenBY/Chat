using Chat.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Infrastructure;

public static class InfrastructureExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddIdentityProvider()
            .AddKeycloakServices();
    }
}
