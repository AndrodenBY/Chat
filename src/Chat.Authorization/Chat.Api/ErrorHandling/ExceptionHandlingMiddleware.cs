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
            KeyNotFoundException ex => Error.NotFound("Resource.NotFound", ex.Message),
            ArgumentException ex => Error.Validation("Request.InvalidArgument", ex.Message),
            UnauthorizedAccessException ex => Error.Unauthorized("Access.Unauthorized", ex.Message),
            InvalidOperationException ex => Error.Conflict("Operation.Invalid", ex.Message),
            _ => Error.Unexpected("Server.Error", "An unexpected error occured")
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
