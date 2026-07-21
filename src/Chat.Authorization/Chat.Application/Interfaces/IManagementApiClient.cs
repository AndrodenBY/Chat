using ErrorOr;

namespace Chat.Application.Interfaces;

public interface IManagementApiClient
{
    Task<ErrorOr<HttpResponseMessage>> SendRequest(HttpRequestMessage request, CancellationToken cancellationToken);
}
