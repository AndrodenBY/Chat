namespace Chat.Domain.Common.Result;

public readonly struct Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<Error> Errors  { get; }
    
    public Error PrimaryError => Errors.Count > 0 
        ? Errors[0] 
        : Error.None;

    private Result(bool isSuccess, IReadOnlyList<Error> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }
    
    public static Result Success => new(true, []);
    public static Result Failure(Error error) => new(false, [error]);
    public static Result Failure(IReadOnlyList<Error> errors) => new(false, errors);
    
    public static implicit operator Result(Error error) => Failure(error);
    public static implicit operator Result(List<Error> errors) => Failure(errors);
    
    
}
