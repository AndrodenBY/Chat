namespace Chat.Domain.Common;

public readonly struct Result<TValue>
{
    private readonly TValue? _value;
    
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<Error> Errors { get; }
    
    public Error PrimaryError => Errors.Count > 0 
        ? Errors[0]
        : Error.None;

    public TValue Value => IsSuccess 
        ? _value! 
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    private Result(TValue? value, bool isSuccess, IReadOnlyList<Error> errors)
    {
        IsSuccess = isSuccess;
        _value = value;
        Errors = errors;
    }

    public static Result<TValue> Success(TValue value) => new(value, true, []);
    public static Result<TValue> Failure(Error error) => new(default, false, [error]);
    public static Result<TValue> Failure(IReadOnlyList<Error> errors) => new(default, false, errors);
    
    public static implicit operator Result<TValue>(TValue value) => Success(value);
    public static implicit operator Result<TValue>(Error error) => Failure(error);
    public static implicit operator Result<TValue>(List<Error> errors) => Failure(errors);
}
