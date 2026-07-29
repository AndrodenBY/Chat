using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Chat.Api.ErrorHandling;

public class ErrorInterceptor(ILogger<ErrorInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "An unhandled infrastructure exception occurred during gRPC call {Method}.", context.Method);
            
            throw new RpcException(new Status(StatusCode.Internal, "An internal server error occurred."));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected exception occurred during gRPC call {Method}.", context.Method);

            throw new RpcException(new Status(StatusCode.Unknown, "An unexpected error occurred."));
        }
    }
}
