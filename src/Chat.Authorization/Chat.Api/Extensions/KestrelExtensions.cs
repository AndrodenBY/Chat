using Chat.Api.Options;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Chat.Api.Extensions;

public static class KestrelExtensions
{
    public static IServiceCollection AddEndpointOptions(this IServiceCollection services)
    {
        services.AddOptions<EndpointOptions>()
            .BindConfiguration(EndpointOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public static IWebHostBuilder ConfigureChatKestrel(this IWebHostBuilder webHost)
    {
        webHost.ConfigureKestrel((context, options) =>
        {
            var endpoints = context.Configuration
                .GetSection(EndpointOptions.SectionName)
                .Get<EndpointOptions>()!;

            options.ListenAnyIP(endpoints.Rest.Port, listen =>
            {
                listen.UseHttps();
                listen.Protocols = HttpProtocols.Http1AndHttp2;
            });

            options.ListenAnyIP(endpoints.Grpc.Port, listen =>
            {
                listen.Protocols = HttpProtocols.Http2;
            });
        });

        return webHost;
    }
}
