using ErrorOr;

namespace Chat.Api.Extensions;

public static class ErrorOrExtensions
{
    public static IResult ToApiResult<T>(this ErrorOr<T> result)
    {
        return result.Match(
            onValue: Results.Ok,
            onError: errors => errors.ToProblem()
        );
    }

    public static IResult ToProblem(this List<Error> errors)
    {
        if (errors.Count == 0)
        {
            return Results.Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            var errorMap = errors
                .GroupBy(error => error.Code)
                .ToDictionary(
                    grouping => grouping.Key,
                    grouping => grouping.Select(error => error.Description).ToArray()
                );

            return Results.ValidationProblem(errorMap);
        }
        
        var firstError = errors[0];
        
        return Results.Problem(
            statusCode: firstError.Type.ToStatusCode(),
            title: firstError.Code,
            detail: firstError.Description
        );
    }
    
    public static int ToStatusCode(this ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
}
