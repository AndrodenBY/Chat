namespace Chat.Domain.Common;

public readonly record struct Error(string Code, string Message, ErrorType type)
{
    public static readonly Error None = new Error(string.Empty, string.Empty, ErrorType.Failure);

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);
    
    public static Error NotFound(string code, string message) => 
        new(code, message, ErrorType.NotFound);

    public static Error Conflict(string code, string message) => 
        new(code, message, ErrorType.Conflict);

    public static Error Failure(string code, string message) => 
        new(code, message, ErrorType.Failure);
}
