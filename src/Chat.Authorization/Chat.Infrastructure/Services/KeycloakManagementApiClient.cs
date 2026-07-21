using System.Net.Http.Headers;
using Chat.Application.Interfaces;
using Chat.Infrastructure.Helpers;
using Chat.Infrastructure.Options;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.Services;

public class KeycloakManagementApiClient(
    IHttpClientFactory httpClientFactory,
    IOptions<IdentityProviderOptions> identityProviderOptions,
    IOptionsMonitor<IdentityProviderClientOptions> clientOptions) 
    : IManagementApiClient
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("IdentityProvider");
    private readonly IdentityProviderOptions _identityProviderOptions = identityProviderOptions.Value;
    private readonly IdentityProviderClientOptions _adminClientOptions = clientOptions.Get(IdentityProviderClientOptions.AdminClient);

    public async Task<ErrorOr<HttpResponseMessage>> SendRequest(HttpRequestMessage request, CancellationToken cancellationToken)
    {
         var tokenResult = await TokenHelper.SendManagementTokenRequest(
             _httpClient,
             _identityProviderOptions.TokenEndpoint,
             _adminClientOptions,
             cancellationToken
         );

         if (tokenResult.IsError)
         {
             return tokenResult.Errors;
         }

         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Value);
         
         return await _httpClient.SendAsync(request, cancellationToken);
    }
}
