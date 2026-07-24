using ErrorOr;

namespace Chat.Api.Extensions;

public static class ErrorOrExtensions
{
    public static IResult ToApiResult<T>(
        this ErrorOr<T> result)
    {
        return result.Match(
            value => Results.Ok(value),
            errors => errors.ToProblem());
    }

    public static IResult ToProblem(
        this List<Error> errors)
    {
        var error = errors[0];

        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return Results.Problem(
            statusCode: statusCode,
            title: error.Code,
            detail: error.Description);
    }
}
