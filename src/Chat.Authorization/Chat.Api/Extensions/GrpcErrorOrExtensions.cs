using ErrorOr;
using Grpc.Core;

namespace Chat.Api.Extensions;

public static class GrpcErrorOrExtensions
{
    public static RpcException ToRpcException(this List<Error> errors)
    {
        if (errors.Count == 0)
        {
            return new RpcException(new Status(StatusCode.Internal, "An unexpected error occurred."));
        }

        var firstError = errors[0];
        return new RpcException(new Status(firstError.Type.ToGrpcStatusCode(), firstError.Description));
    }

    private static StatusCode ToGrpcStatusCode(this ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCode.InvalidArgument,
        ErrorType.Unauthorized => StatusCode.Unauthenticated,
        ErrorType.Forbidden => StatusCode.PermissionDenied,
        ErrorType.NotFound => StatusCode.NotFound,
        ErrorType.Conflict => StatusCode.AlreadyExists,
        _ => StatusCode.Internal
    };
}
