using Chat.Api.Extensions;
using ErrorOr;
using Microsoft.AspNetCore.Diagnostics;

namespace Chat.Api.ErrorHandling;

public class ExceptionHandlingMiddleware(
    ILogger<ExceptionHandlingMiddleware> logger, 
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Unhandled Exception at {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path
        );

        return await HandleExceptionResponse(exception, httpContext);
    }

    private async Task<bool> HandleExceptionResponse(Exception exception, HttpContext httpContext)
    {
        var error = exception switch
        {
            KeyNotFoundException => 
                Error.NotFound("Resource.NotFound", "The requested resource was not found."),

            ArgumentException => 
                Error.Validation("Request.InvalidArgument", "The request contains invalid data."),

            UnauthorizedAccessException => 
                Error.Unauthorized("Access.Unauthorized", "You are not authorized to perform this action."),

            InvalidOperationException => 
                Error.Conflict("Operation.Invalid", "The requested operation cannot be completed."),

            _ => Error.Unexpected("Server.Error", "An unexpected error occurred.")
        };
        
        httpContext.Response.StatusCode = error.Type.ToStatusCode();
        
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails =
            {
                Status = error.Type.ToStatusCode(),
                Title = error.Code,
                Detail = error.Description,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            }
        });
    }
}
